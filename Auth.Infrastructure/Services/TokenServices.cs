using Auth.Application.Repository;
using Auth.Domain.Model;
using Auth.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Auth.Infrastructure.Services
{
    public class TokenServices : IToken
    {
        ApplicationDbContext db;
        IConfiguration configuration;
        public TokenServices(ApplicationDbContext db, IConfiguration configuration)
        {
            this.db = db;
            this.configuration = configuration;
        }
        public string GenerateToken(Login details)
        {
            var cfg = configuration.GetSection("JwtSettings");
            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(cfg["SecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var userId = db.User.Where(ur => ur.UserName == details.UserName || ur.Email == details.UserName).Select(x => x.UserId).FirstOrDefault();

            var roleName = db.UserRole
                .Include(ur => ur.User)
                .Include(ur => ur.Role)
                .Where(ur => ur.User.UserName == details.UserName || ur.User.Email == details.UserName) // if 'token' holds the username
                .Select(ur => ur.Role)
                .FirstOrDefault();
            if (roleName == null)
            {
                return null; // Or throw an exception if appropriate
            }


            var claims = new List<Claim>
                {
                new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
                new Claim(ClaimTypes.Role, roleName.RoleName), // assuming RoleName is the name
                new Claim("CustomData", details.UserName)
            };

            // Create the JWT token
            var token = new JwtSecurityToken(
                issuer: cfg["Issuer"],
                audience: cfg["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
