using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Photobox.DB;
using Photobox.Helpers;
using Photobox.Models;

namespace Photobox.Controllers
{

    [ApiController]
    [Route ("[controller]")]
    public class LoginController : ControllerBase
    {

        private IConfiguration _config;
        private PhotoboxDB _database;
        private PasswordHelper _pwHelper;
        private JWTHelper _jwtHelper;

        public LoginController (IConfiguration config, PhotoboxDB database)
        {
            _config = config;
            _database = database;
            _pwHelper = new PasswordHelper ();
            _jwtHelper = new JWTHelper (config);
        }

        [HttpGet, Authorize]
        public string Get ()
        {
            var currentUser = HttpContext.User;
            var userId = HttpContext.User.FindFirst (ClaimTypes.Email).Value;
            return userId;
        }

        [HttpPost]
        public IActionResult Post ([FromBody] UserCredentials credentials)
        {
            try
            {

                var user = _database.GetPhotographerByEmail (credentials.email);

                if (user == null)
                {
                    return StatusCode (401);
                }

                // Check if password is valid
                if (_pwHelper.VerifyHashedPassword (user.hashedPassword, credentials.password) == PasswordVerificationResult.Success)
                {
                    string tokenString = _jwtHelper.CreatePhotographerJWT (credentials.email);

                    return Ok (new { token = tokenString });;
                }
                else
                {
                    return Unauthorized (new { message = "Password provided is wrong" });
                }
            }
            catch (ArgumentNullException)
            {
                return StatusCode (StatusCodes.Status400BadRequest, new { message = "Request must contain a password." });
            }
            catch (Exception err)
            {
                Console.WriteLine (err);
                return StatusCode (StatusCodes.Status500InternalServerError, new { message = err.ToString () });
            }
        }

        private string CreateJWT (string email)
        {

            var claims = new []
            {
                new Claim (JwtRegisteredClaimNames.Email, email)
            };

            var key = new SymmetricSecurityKey (Encoding.UTF8.GetBytes (_config["Jwt:Key"]));
            var creds = new SigningCredentials (key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken (_config["Jwt:Issuer"],
                _config["Jwt:Issuer"],
                claims,
                expires : DateTime.Now.AddMinutes (30),
                signingCredentials : creds);

            return new JwtSecurityTokenHandler ().WriteToken (token);
        }

    }

}