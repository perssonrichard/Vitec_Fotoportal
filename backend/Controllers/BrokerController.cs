using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Photobox.CustomExceptions;
using Photobox.DB;
using Photobox.Helpers;
using Photobox.Models;

namespace Photobox.Controllers {

    [ApiController]
    [Route ("api/[controller]")]

    public class BrokerController : ControllerBase {
        private const string AUTHENTICATION_VALUE = "Vml0ZWNmb3RvcG9ydGFsOmFtSzlzY0VnSnpNd1Bqckdack03bUNIY1llWEdsbmxDdEIyQ3JweHU0YTlIa25Eektn";
        private IConfiguration _config;
        private PhotoboxDB _database;
        private JWTHelper _jwtHelper;

        public BrokerController (IConfiguration config, PhotoboxDB database) {
            _config = config;
            _database = database;
            _jwtHelper = new JWTHelper (config);
        }

        [HttpGet ("createorder")]
        public async Task<IActionResult> CreateOrder ([FromQuery] Order order) {

            try {
                HttpContext.Request.Headers.TryGetValue ("Authorization", out var basicAuthString);

                if (string.IsNullOrEmpty (basicAuthString)) {
                    return StatusCode (StatusCodes.Status400BadRequest, new { message = "Request must contain an Authorization header." });
                }

                // den här funkar när jag kör postman anrop med Zm90b3BvcnRhbDpvS29rcDEyM2shb3BrQURuZHc3NDI0YTE= i Authorization header
                // men får 500 när jag kör anrop från next, därför är den utkommenterad
                // if (!validBasicAuth (basicAuthString)) {
                //     return StatusCode (StatusCodes.Status400BadRequest, new { message = "Request contains invalid value in Authorization header." });
                // }

                DateTime thisDay = DateTime.Now;
                order.regDate = thisDay;
                order = await OrderController.CompleteOrderDetails (order);

                if (order == null) {
                    return StatusCode (StatusCodes.Status500InternalServerError, new { message = "Hub request didn't succeed." });
                }

                _database.CreateOrder (order);

                string tokenString = _jwtHelper.CreateBrokerJWT (order.orderId);
                IActionResult response = Redirect (_config["SiteBaseUrl"] + "/maklare?token=" + tokenString);
                return response;

            } catch (OrderAlreadyExistException err) {
                Console.WriteLine (err);
                return StatusCode (StatusCodes.Status409Conflict, new { message = "Order already exists." });
            } catch (Exception err) {
                Console.WriteLine (err);
                return StatusCode (StatusCodes.Status500InternalServerError, new { message = "Internal server error." });
            }
        }

        [HttpGet, Authorize]
        public ActionResult<Order> GetOrder () {
            try {
                var orderIdFromToken = HttpContext.User.FindFirst ("orderId").Value;
                Order order = _database.GetOrderById (orderIdFromToken);
                return order;

            } catch (ArgumentNullException) {
                return StatusCode (StatusCodes.Status400BadRequest, new { message = "Request must contain an email." });
            } catch (Exception err) {
                Console.WriteLine (err);
                return StatusCode (StatusCodes.Status500InternalServerError, new { message = "Internal server error." });
            }
        }

        [HttpPut ("order"), Authorize]
        public async Task<IActionResult> UpdateOrder ([FromBody] UpdatedOrder order) {
            try {

                HttpContext.Request.Headers.TryGetValue ("Authorization", out var token);

                var orderId = HttpContext.User.FindFirst ("orderId").Value;
                Order orderInDb = _database.GetOrderById (orderId);

                if (order == null) {
                    return StatusCode (StatusCodes.Status400BadRequest, new { message = "Requested order does not exist." });
                }

                if (orderInDb.status == StatusType.Cancelled) {
                    return StatusCode (StatusCodes.Status400BadRequest, new { message = "Order is already cancelled. You can't edit details of an order after it is cancelled." });
                }

                if (orderInDb.status == StatusType.Created) {
                    if (order.status == StatusType.InProgress) {
                        orderInDb.photographerEmail = order.photographerEmail;
                        orderInDb.description = order.description;
                        orderInDb.status = order.status;
                        _database.UpdateOrder (orderInDb);
                        string updatedMsg = $"Order {order.status} with {order.photographerEmail} assigned as Photographer.";
                        // Calling hub
                        bool orderstatusChangedOnHub = await OrderController.ChangeOrderstatusOnHub (order, updatedMsg, token, _config);
                        if (orderstatusChangedOnHub) {
                            return StatusCode (StatusCodes.Status200OK, new { message = updatedMsg });
                        } else {
                            return StatusCode (StatusCodes.Status400BadRequest, new { message = "Hub request did not succeed." });
                        }
                    }
                }

                if (order.status == StatusType.Cancelled) {
                    _database.UpdateOrderStatus (orderId, order.status);
                    string cancelledMsg = "Order 'Cancelled'.";
                    // Calling hub
                    bool orderstatusChangedOnHub = await OrderController.ChangeOrderstatusOnHub (order, cancelledMsg, token, _config);
                    if (orderstatusChangedOnHub) {
                        return StatusCode (StatusCodes.Status200OK, new { message = cancelledMsg });
                    } else {
                        return StatusCode (StatusCodes.Status400BadRequest, new { message = "Hub request did not succeed." });
                    }
                }

                IActionResult badRequest = BadRequest (new { message = "Select a valid status of the order." });
                return badRequest;

            } catch (ArgumentNullException) {
                return StatusCode (StatusCodes.Status400BadRequest, new { message = "Put request must contain an orderId." });
            } catch (OrderDoesNotExistException) {
                return StatusCode (StatusCodes.Status400BadRequest, new { message = "The provided orderId does not exist." });
            } catch (Exception err) {
                Console.WriteLine (err);
                return StatusCode (500);
            }
        }

        // TODO felhantering i denna
        [HttpPost ("estateinfo"), Authorize]
        public async Task<IActionResult> GetEstateInfo ([FromBody] UpdatedOrder order) {
            try {
                var result = await BrokerController.GetEstateInfoFromNext (order);
                var response = Ok (new { result });
                return response;

            } catch (Exception err) {
                Console.WriteLine (err);
                return null;
            }
        }

        // TODO felhantering i denna
        // api/broker/brokerinfo
        [HttpPost ("brokerinfo"), Authorize]
        public async Task<IActionResult> GetBrokerInfo ([FromBody] BrokerInfo brokerInfo) {
            try {
                var result = await BrokerController.GetBrokerInfoFromNext (brokerInfo);
                var response = Ok (new { result });
                return response;

            } catch (Exception err) {
                Console.WriteLine (err);
                return null;
            }
        }

        // TODO felhantering i denna
        [HttpPost ("departmentinfo"), Authorize]
        public async Task<IActionResult> GetDepartmentInfo ([FromBody] DepartmentInfo departmentInfo) {
            try {
                var result = await BrokerController.GetDepartmentInfoFromNext (departmentInfo);
                var response = Ok (new { result });
                return response;

            } catch (Exception err) {
                Console.WriteLine (err);
                return null;
            }
        }

        // api/broker/photographer/{email} returns a photographerobject
        [HttpGet ("photographer/{email}"), Authorize]
        public IActionResult GetUser (string email) {
            try {
                Photographer photographer = _database.GetPhotographerByEmail (email);
                IActionResult response = Ok (new {
                    photographer
                });
                return response;

            } catch (Exception err) {
                Console.WriteLine (err.ToString ());
                return StatusCode (500);
            }
        }

        private bool validBasicAuth (string basicAuthString) {
            string decodedUsername = DecodeBase64UserName (basicAuthString);
            string decodedPassword = DecodeBase64Pwd (basicAuthString);
            string base64Username = (string)_config["NextBase64Credentials:Username"];
            string base64Password = (string)_config["NextBase64Credentials:Password"];
            return (decodedUsername == base64Username && decodedPassword == base64Password);
        }

        private string DecodeBase64UserName (string basicAuthString) {
            string result = Base64Decode (basicAuthString);
            string[] splittedArray = result.Split (":");
            return splittedArray[0];
        }
        private string DecodeBase64Pwd (string basicAuthString) {
            string result = Base64Decode (basicAuthString);
            string[] splittedArray = result.Split (":");
            return splittedArray[1];
        }
        private static string Base64Decode (string base64EncodedData) {
            var base64EncodedBytes = System.Convert.FromBase64String (base64EncodedData);
            return System.Text.Encoding.UTF8.GetString (base64EncodedBytes);
        }

        private static async Task<Newtonsoft.Json.Linq.JObject> GetEstateInfoFromNext (UpdatedOrder order) {
            try {
                HttpClient client = new HttpClient ();

                // TODO stringconstants for hub-urls
                client.BaseAddress = new Uri ("https://hubtest.megler.vitec.net/");
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue ("Basic", AUTHENTICATION_VALUE);

                string uri = order.installationId + "/Estates/" + order.estateId;

                dynamic result = await client.GetStringAsync (uri);
                result = JsonConvert.DeserializeObject<object> (result);
                return result;
            } catch (Exception err) {
                Console.WriteLine (err);
                return null;
            }

        }

        private static async Task<Newtonsoft.Json.Linq.JObject> GetBrokerInfoFromNext (BrokerInfo brokerInfo) {
            try {
                HttpClient client = new HttpClient ();

                // TODO put all hub url in a string constantfile
                client.BaseAddress = new Uri ("https://hubtest.megler.vitec.net/");
                // client.BaseAddress = new Uri ("https://hub.megler.vitec.net/");

                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue ("Basic", "Vml0ZWNmb3RvcG9ydGFsOmFtSzlzY0VnSnpNd1Bqckdack03bUNIY1llWEdsbmxDdEIyQ3JweHU0YTlIa25Eektn");

                string uri = brokerInfo.installationId + "/Employees/" + brokerInfo.employeeId;

                dynamic result = await client.GetStringAsync (uri);
                result = JsonConvert.DeserializeObject<object> (result);

                return result;
            } catch (Exception err) {
                Console.WriteLine (err);
                return null;
            }
        }

        private static async Task<Newtonsoft.Json.Linq.JObject> GetDepartmentInfoFromNext (DepartmentInfo departmentInfo) {
            try {
                HttpClient client = new HttpClient ();

                // TODO put all hub url in a string constantfile
                client.BaseAddress = new Uri ("https://hubtest.megler.vitec.net/");
                // client.BaseAddress = new Uri ("https://hub.megler.vitec.net/");

                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue ("Basic", "Vml0ZWNmb3RvcG9ydGFsOmFtSzlzY0VnSnpNd1Bqckdack03bUNIY1llWEdsbmxDdEIyQ3JweHU0YTlIa25Eektn");

                string uri = departmentInfo.installationId + "/Departments/" + departmentInfo.departmentId;

                dynamic result = await client.GetStringAsync (uri);
                result = JsonConvert.DeserializeObject<object> (result);

                return result;
            } catch (Exception err) {
                Console.WriteLine (err);
                return null;
            }
        }

    }
}