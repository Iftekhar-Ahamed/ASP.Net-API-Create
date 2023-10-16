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

namespace Assignment.Repository
{
    public class JwtTokenRepository:IJwtToken
    {
        private string _secretKey;
        public JwtTokenRepository(string secrateKey)
        {
            _secretKey=secrateKey;
        }
        
        public string GenerateToken(UserDto user)
        {

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_secretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                new Claim(ClaimTypes.NameIdentifier, "1")
            }),
                Expires = DateTime.Now.AddMinutes(10),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            };
            
            try
            {
                var token = tokenHandler.CreateToken(tokenDescriptor);
                return tokenHandler.WriteToken(token);
            }
            catch (Exception ex)
            {

            }
            return string.Empty;
        }
    }

}
