using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
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
    public class OrderController : ControllerBase {

        private IConfiguration _config;
        private PhotoboxDB _database;
        private JWTHelper _jwtHelper;

        public OrderController (IConfiguration config, PhotoboxDB database) {
            _config = config;
            _database = database;
            _jwtHelper = new JWTHelper (config);
        }

        [HttpGet, Authorize]
        public ActionResult<Order[]> GetPhotographerOrders () {
            try {
                var currentUser = HttpContext.User;
                var userId = HttpContext.User.FindFirst (ClaimTypes.Email).Value;

                List<Order> orderList = _database.GetPhotographerOrders (userId);

                Order[] orders = orderList.ToArray ();

                return orders;

            } catch (ArgumentNullException) {
                return StatusCode (StatusCodes.Status400BadRequest, new { message = "Request must contain an email." });
            } catch (Exception err) {
                Console.WriteLine (err);
                return StatusCode (StatusCodes.Status500InternalServerError, new { message = "Internal server error." });
            }
        }

        [HttpPut ("status"), Authorize]
        public async Task<IActionResult> DeliverOrder ([FromBody] UpdatedOrder order) {
            try {
                HttpContext.Request.Headers.TryGetValue ("Authorization", out var token);

                var currentPhotographerEmail = HttpContext.User;
                Order orderInDb = _database.GetOrderById (order.orderId);

                if (!OrderBelongsToPhotographer (currentPhotographerEmail, orderInDb.photographerEmail)) {
                    return StatusCode (StatusCodes.Status400BadRequest, new { message = "Requested order does not belong to the provided photographer." });
                }

                if (orderInDb.status == StatusType.Delivered) {
                    return StatusCode (StatusCodes.Status400BadRequest, new { message = "The order is already delivered." });
                }

                if (order.status != StatusType.Delivered) {
                    return StatusCode (StatusCodes.Status400BadRequest, new { message = "A photographer can only change the status of an order to delivered." });
                }

                _database.UpdateOrderStatus (order.orderId, order.status);
                
                string deliverMsg = $"Order {order.status} by {order.photographerEmail}.";
                bool orderstatusChangedOnHub = await ChangeOrderstatusOnHub (order, deliverMsg, token, _config);
                if (orderstatusChangedOnHub) {
                    return StatusCode (StatusCodes.Status200OK, new { message = deliverMsg });
                } else {
                    return StatusCode (StatusCodes.Status400BadRequest, new { message = "Hub request did not succeed." });
                }

            } catch (ArgumentNullException) {
                return StatusCode (StatusCodes.Status400BadRequest, new { message = "Put request must contain an orderId." });
            } catch (OrderDoesNotExistException) {
                return StatusCode (StatusCodes.Status400BadRequest, new { message = "The provided orderId does not exist." });
            } catch (Exception err) {
                Console.WriteLine (err);
                return StatusCode (500);
            }
        }

        // enbart till för att skapa en order i test
        // och returnera en broker JWT
        [HttpPost, Authorize]
        public IActionResult PostOrder ([FromBody] Order order) {
            try {
                DateTime thisDay = DateTime.Now;
                order.regDate = thisDay;

                _database.CreateOrder (order);

                string tokenString = _jwtHelper.CreateBrokerJWT (order.orderId);

                IActionResult response = Ok (new { token = tokenString, message = "Order created" });
                return response;
            } catch (OrderAlreadyExistException) {
                return StatusCode (StatusCodes.Status409Conflict, new { message = "Order already exists." });
            } catch (Exception err) {
                Console.WriteLine (err);
                return StatusCode (StatusCodes.Status500InternalServerError, new { message = "Internal server error." });
            }
        }

        public static async Task<Order> CompleteOrderDetails (Order incompleteOrder) {
            try {
                HttpClient client = new HttpClient ();

                client.BaseAddress = new Uri ("https://hubtest.megler.vitec.net/");
                // client.BaseAddress = new Uri ("https://hub.megler.vitec.net/");

                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue ("Basic", "Vml0ZWNmb3RvcG9ydGFsOmFtSzlzY0VnSnpNd1Bqckdack03bUNIY1llWEdsbmxDdEIyQ3JweHU0YTlIa25Eektn");

                string uri = incompleteOrder.installationId + "/Estates/" + incompleteOrder.estateId;

                dynamic result = await client.GetStringAsync (uri);
                result = JsonConvert.DeserializeObject<object> (result);

                Order completeOrder = incompleteOrder;
                completeOrder.address = result.address.streetAdress;
                completeOrder.postalCode = result.address.zipCode;
                completeOrder.city = result.address.city;

                return completeOrder;

            } catch (Exception err) {
                Console.WriteLine (err);
                return null;
            }
        }

        // TODO
        // Move this function into separate helper function along with all Next calls,
        // that can be used for both Norwegian and Swedish API
        // then we don't have to use it as static and pass config as parameter (bad practice)
        public static async Task<bool> ChangeOrderstatusOnHub (UpdatedOrder order, string msg, string token, IConfiguration _config) {
            HttpClient client = new HttpClient ();

            client.BaseAddress = new Uri ("https://hubtest.megler.vitec.net/");
            // client.BaseAddress = new Uri ("https://hub.megler.vitec.net/");

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue ("Basic", "Vml0ZWNmb3RvcG9ydGFsOmFtSzlzY0VnSnpNd1Bqckdack03bUNIY1llWEdsbmxDdEIyQ3JweHU0YTlIa25Eektn");

            string uri = order.installationId + "/Orders/" + order.orderId + "/Status?message=" + msg + "&status=" + order.status + "&url=" + _config["SiteBaseUrl"] + "/maklare";
          
            dynamic response = await client.PostAsync (uri, null);
            if (response.StatusCode == HttpStatusCode.NoContent) {
                return true;
            }
            return false;
        }

        private bool OrderBelongsToPhotographer (System.Security.Claims.ClaimsPrincipal currentUser, string photographer) {
            return currentUser.HasClaim (c => c.Type == ClaimTypes.Email && c.Value == photographer);
        }

    }

}