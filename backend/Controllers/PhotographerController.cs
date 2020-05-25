using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Photobox.DB;
using Photobox.Helpers;
using Photobox.Models;

namespace Photobox.Controllers
{
    [ApiController]
    [Route ("/api/[controller]")]
    public class PhotographerController : ControllerBase
    {
        private IConfiguration _config;
        private PhotoboxDB _database;
        private PasswordHelper _pwHelper;

        public PhotographerController (IConfiguration config, PhotoboxDB database)
        {
            _config = config;
            _database = database;
            _pwHelper = new PasswordHelper ();
        }

        // api/user returns all photographers
        // TODO Authorize endast admin får hämta alla
        [HttpGet, Authorize]
        public IActionResult GetAllUsers ()
        {
            try
            {
                List<Photographer> photographerList = _database.GetAllPhotographers ();

                IActionResult response = Ok (new
                {
                    message = "Response is ok",
                        data = photographerList
                });
                return response;

            }
            catch (Exception err)
            {
                IActionResult response = StatusCode (500, err);
                return response;
            }
        }

        // api/photographer/{email} returns a photographerobject
        [HttpGet ("{email}"), Authorize]
        public IActionResult GetUser (string email)
        {
            try
            {
                Photographer photographer = _database.GetPhotographerByEmail (email);
                IActionResult response = Ok (new
                {
                    photographer
                });
                return response;

            }
            catch (Exception err)
            {
                Console.WriteLine (err.ToString ());
                return StatusCode (500);
            }
        }

        // api/user/ returns a photographerobject
        [HttpPut ("{email}"), Authorize]
        public IActionResult EditUser (string email, [FromBody] Photographer photographer)
        {
            try
            {
                Photographer user = _database.GetPhotographerByEmail (email);
                if (user != null)
                {
                    user.firstName = photographer.firstName;
                    user.lastName = photographer.lastName;
                    user.cellPhoneNumber = photographer.cellPhoneNumber;
                    user.address = photographer.address;
                    user.company = photographer.company;
                    user.orgNr = photographer.orgNr;
                    user.city = photographer.city;
                    user.postalCode = photographer.postalCode;
                    user.postalCodeArea = photographer.postalCodeArea;
                    user.available = photographer.available;

                    _database.UpdatePhotographer (user);

                    IActionResult response = Ok (new { message = "Photographer was updated", user });
                    return response;
                }
                else
                {
                    IActionResult response = NotFound (new { message = "User was not found by email" });
                    return response;
                }
            }
            catch (Exception err)
            {
                Console.WriteLine (err.ToString ());
                return StatusCode (500);
            }

        }

        [HttpPut ("editPassword"), Authorize]
        public IActionResult EditUserPassword ([FromBody] UpdatedPassword photographer)
        {
            try
            {
                string email = photographer.email;

                Photographer dbUser = _database.GetPhotographerByEmail (email);
                if (dbUser != null)
                {
                    // Compare old password
                    if (_pwHelper.VerifyHashedPassword (dbUser.hashedPassword, photographer.oldPassword) == PasswordVerificationResult.Success)
                    {
                        string hash = _pwHelper.HashPassword (photographer.newPassword);
                        _database.UpdatePhotographerPassword (email, hash);

                        return Ok (new { message = "Password updated" });
                    }
                    else
                    {
                        return Unauthorized (new { message = "Password provided is wrong" });
                    }
                }
                else
                {
                    return NotFound (new { message = "User email not found" });
                }
            }
            catch (Exception err)
            {
                Console.WriteLine (err.ToString ());
                return StatusCode (500);
            }
        }

        // api/user/{email} deletes a user
        [HttpDelete ("{email}"), Authorize]
        public IActionResult DeleteUser (string email)
        {
            try
            {
                _database.DeletePhotographerByEmail (email);
                IActionResult response = Ok (new { message = "Response is ok" });
                return response;

            }
            catch (Exception err)
            {
                Console.WriteLine (err.ToString ());
                return StatusCode (500);
            }
        }

        [HttpGet ("brokeremployeeId"), Authorize]
        public async Task<string> GetEmployeeId ([FromBody] UpdatedOrder order)
        {
            try
            {
                HttpClient client = new HttpClient ();

                client.BaseAddress = new Uri ("https://hubtest.megler.vitec.net/");
                // client.BaseAddress = new Uri ("https://hub.megler.vitec.net/");

                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue ("Basic", "Vml0ZWNmb3RvcG9ydGFsOmFtSzlzY0VnSnpNd1Bqckdack03bUNIY1llWEdsbmxDdEIyQ3JweHU0YTlIa25Eektn");

                string uri = order.installationId + "/Estates/" + order.estateId;

                dynamic result = await client.GetStringAsync (uri);
                result = JsonConvert.DeserializeObject<object> (result);
                Console.WriteLine (result.brokersIdWithRoles[0].employeeId);

                return result.brokersIdWithRoles[0].employeeId;
            }
            catch (Exception err)
            {
                Console.WriteLine (err);
                return null;
            }
        }

        // TODO felhantering
        // , Authorize
        // api/photographer/brokerinfo
        [HttpPost ("brokerinfo"), Authorize]
        public async Task<IActionResult> GetBrokerInfo ([FromBody] BrokerInfo brokerInfo)
        {
            try
            {
                var result = await PhotographerController.GetBrokerInfoFromNext (brokerInfo);
                var response = Ok (new { result });
                return response;
            }
            catch (Exception err)
            {
                Console.WriteLine (err);
                return null;
            }
        }

        // TODO felhantering
        // , Authorize
        [HttpPost ("departmentinfo"), Authorize]
        public async Task<IActionResult> GetDepartmentInfo ([FromBody] DepartmentInfo departmentInfo)
        {
            try
            {
                var result = await PhotographerController.GetDepartmentInfoFromNext (departmentInfo);
                var response = Ok (new { result });
                return response;
            }
            catch (Exception err)
            {
                Console.WriteLine (err);
                return null;
            }
        }

        private static async Task<Newtonsoft.Json.Linq.JObject> GetBrokerInfoFromNext (BrokerInfo brokerInfo)
        {
            try
            {
                HttpClient client = new HttpClient ();

                // TODO stringconstants for hub-urls
                client.BaseAddress = new Uri ("https://hubtest.megler.vitec.net/");
                // client.BaseAddress = new Uri ("https://hub.megler.vitec.net/");

                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue ("Basic", "Vml0ZWNmb3RvcG9ydGFsOmFtSzlzY0VnSnpNd1Bqckdack03bUNIY1llWEdsbmxDdEIyQ3JweHU0YTlIa25Eektn");

                string uri = brokerInfo.installationId + "/Employees/" + brokerInfo.employeeId;

                dynamic result = await client.GetStringAsync (uri);
                result = JsonConvert.DeserializeObject<object> (result);

                return result;
            }
            catch (Exception err)
            {
                Console.WriteLine (err);
                return null;
            }
        }

        private static async Task<Newtonsoft.Json.Linq.JObject> GetDepartmentInfoFromNext (DepartmentInfo departmentInfo)
        {
            try
            {
                HttpClient client = new HttpClient ();

                // TODO stringconstants for hub-urls
                client.BaseAddress = new Uri ("https://hubtest.megler.vitec.net/");
                // client.BaseAddress = new Uri ("https://hub.megler.vitec.net/");

                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue ("Basic", "Vml0ZWNmb3RvcG9ydGFsOmFtSzlzY0VnSnpNd1Bqckdack03bUNIY1llWEdsbmxDdEIyQ3JweHU0YTlIa25Eektn");

                string uri = departmentInfo.installationId + "/Departments/" + departmentInfo.departmentId;

                dynamic result = await client.GetStringAsync (uri);
                result = JsonConvert.DeserializeObject<object> (result);

                return result;
            }
            catch (Exception err)
            {
                Console.WriteLine (err);
                return null;
            }

        }
    }
}