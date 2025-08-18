using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Hospital_Management.Data;
using Hospital_Management.Interfaces.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.IdentityModel.Tokens;

namespace Hospital_Management.Interfaces.Implementation
{

    public class TokenImplementation : IGenerateToken
    {
        private readonly IConfiguration _config;
        private readonly DataContext Database;

        public TokenImplementation(IConfiguration config, DataContext database)
        {
            _config = config;   
            Database = database;    
        }
        public string GeneratejwtRefreshToken()
        {
            var RandomBytes=new Byte[32];   
            using var range=RandomNumberGenerator.Create();
            range.GetBytes(RandomBytes);
            return Convert.ToBase64String(RandomBytes);
        }

        public string GeneratejwtToken(string email)
        {
            var SecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:Key"]));

            var Credential = new SigningCredentials(SecurityKey,SecurityAlgorithms.HmacSha256);
            var Roles=Database.registers.FirstOrDefault(x=>x.Email == email);
            if (Roles == null) {

                return null;
            }

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Email,email),
                new Claim(ClaimTypes.Role,Roles.Role)

            };
            var token = new JwtSecurityToken(_config["JWT:Issuer"],
                _config["JWT:Audiance"],
                claims,
                expires: DateTime.Now.AddMinutes(10),
                signingCredentials: Credential

            );
            return new JwtSecurityTokenHandler().WriteToken(token);
            
        }
    }
}
