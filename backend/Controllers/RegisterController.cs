using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Photobox.CustomExceptions;
using Photobox.DB;
using Photobox.Helpers;
using Photobox.Models;

namespace Photobox.Controllers
{
    [ApiController]
    [Route ("api/[controller]")]
    public class RegisterController : ControllerBase
    {
        private IConfiguration _config;
        private PhotoboxDB _database;
        private PasswordHelper _pwHelper;
        public RegisterController (IConfiguration config, PhotoboxDB database)
        {
            _config = config;
            _database = database;
            _pwHelper = new PasswordHelper ();
        }

        [HttpGet]
        public IActionResult GetRegisterResponse (string value)
        {
            Console.WriteLine (value);

            IActionResult response;
            response = Ok (new { message = "This is ok" });
            return response;
        }

        [HttpPost]
        public IActionResult RegisterUser ([FromBody] Photographer user)
        {
            try
            {
                string pw = user.hashedPassword;
                user.hashedPassword = _pwHelper.HashPassword (pw);
                user.orderIdList = new List<string> ();
                user.available = true;
                System.Console.WriteLine (user);
                _database.RegisterPhotographer (user);
                IActionResult response = Ok (new { message = "User created" });
                return response;

            }
            catch (UserAlreadyExistException)
            {
                return StatusCode (409);
            }
            catch (Exception err)
            {
                Console.WriteLine (err.ToString ());
                return StatusCode (500);
            }
        }
    }
}