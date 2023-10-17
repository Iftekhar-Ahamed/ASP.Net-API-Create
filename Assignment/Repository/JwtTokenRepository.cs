using Assignment.Helper;
using Assignment.IRepository;
using System.Threading.Tasks;
using System;
using Assignment.DTO;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Linq;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using Assignment.DbContexts;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using System.Security.Principal;

namespace Assignment.Repository
{
    public class JwtTokenRepository:IJwtToken
    {
        IConfiguration _configuration;
        public JwtTokenRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        
        public string GenerateToken(UserDto _userData)
        {
            var claims = new[] {
                        new Claim(JwtRegisteredClaimNames.Sub, _configuration["Jwt:Subject"]),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                        new Claim("UserId", _userData.UserId.ToString()),
                        //new Claim("DisplayName", "Iftekhar"),
                        //new Claim("UserName", "Ïftekhar Ahamed Siddiquee"),
                        //new Claim("Email", "iftekhar@ibos.io")
                    };


            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims,
                expires: DateTime.UtcNow.AddMinutes(2),
                signingCredentials: signIn);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        public bool CheckTimeExpire(ClaimsIdentity identity)
        {
            if (identity != null)
            {
                // Find the 'exp' claim (expiration time)
                var expirationClaim = identity.FindFirst("exp");

                if (expirationClaim != null && long.TryParse(expirationClaim.Value, out var expirationTime))
                {
                    // Convert the Unix timestamp to a DateTime
                    var expirationDateTime = DateTimeOffset.FromUnixTimeSeconds(expirationTime).DateTime;

                    // Get the current time
                    var currentTime = DateTime.UtcNow;

                    if (currentTime < expirationDateTime)
                    {
                        // JWT is still valid
                        return true;
                    }
                    else
                    {
                        // JWT has expired
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }

            return false;
        }
    }

}
