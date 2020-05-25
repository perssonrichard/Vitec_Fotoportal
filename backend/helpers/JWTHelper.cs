using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Photobox.Helpers {

    public sealed class JWTHelper {

        private IConfiguration _config;

        public JWTHelper (IConfiguration config) {
            _config = config;
        }

        public string CreatePhotographerJWT (string email) {

            var claims = new [] {
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

        public string CreateBrokerJWT (string orderId) {

            var claims = new [] {

                new Claim("orderId", orderId)
            };

            var key = new SymmetricSecurityKey (Encoding.UTF8.GetBytes (_config["Jwt:Key"]));
            var creds = new SigningCredentials (key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken (_config["Jwt:Issuer"],
                _config["Jwt:Issuer"],
                claims,
                expires : DateTime.Now.AddMinutes (5256000), // expires in 10 years
                signingCredentials : creds);

            return new JwtSecurityTokenHandler ().WriteToken (token);
        }

    }
}