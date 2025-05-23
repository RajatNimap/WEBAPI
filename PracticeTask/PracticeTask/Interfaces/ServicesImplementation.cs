
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.IdentityModel.Tokens;
using PracticeTask.Data;
using PracticeTask.Model.Entities;

namespace PracticeTask.Interfaces
{
    public class AuthService : IAuthService
    {
        private readonly DataContext Database;  
        public AuthService(DataContext Database) {
                this.Database = Database;   
        }    
        public async Task<bool> ValidateUser(string username, string password)
        {
            var data=await Database.userDetails.FirstOrDefaultAsync(x=>x.Email == username);
            if (data == null) {
                return false;
            }
            var IsValid=BCrypt.Net.BCrypt.Verify(password,data.Password);
            if (!IsValid) { 
                    return false;
            }
            return true;    
        }
    }
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _config;
        private readonly DataContext Database;

        public TokenService(IConfiguration config,DataContext database)
        {
            _config = config;   
             Database = database;
        }
       
        public string GenerateAccessToken(string email)
        {
            var credential = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var signingkey= new SigningCredentials(credential,SecurityAlgorithms.HmacSha256);
            var role =  Database.userDetails.FirstOrDefault(x => x.Email == email);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Email, email),
                new Claim(ClaimTypes.Role,role.Roles)
            };
            var token = new JwtSecurityToken(
                _config["Jwt:Issuer"],
                _config["Jwt:Audience"],
                claims,
               // expires: DateTime.UtcNow.AddMinutes(10),  
                expires: DateTime.UtcNow.AddMinutes(int.Parse(_config["Jwt:expiry"])),
                signingCredentials:signingkey

            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
                var randombytes=new byte[32];
                using var rng=RandomNumberGenerator.Create();
                rng.GetBytes(randombytes);  
                return Convert.ToBase64String(randombytes);
        }
    }

    public class RefreshOtherService : IRefeshTokenService
    {
        private readonly IConfiguration _config;
        private readonly DataContext Database;
        public RefreshOtherService(IConfiguration config,DataContext Database) {
            _config = config; 
            this.Database = Database;
        }

        public async Task<string> GetEmailToken(string token)
        {
                var data=await Database.refreshTokens.FirstOrDefaultAsync(x => x.Token == token);
                if(data == null)
                {
                     return null;
                }
                return data.Email;  
        }

        public async Task InvalidateRefreshToken(string token)
        {
                var data=await Database.refreshTokens.FirstOrDefaultAsync(x=>x.Token == token);
                if(data !=null)
                {
                       data.IsExpired = true;
                       await Database.SaveChangesAsync();
                }
        }

        public async Task StoreToken(string email, string token)
        {
            var data = new RefreshToken
            {
                Email = email,
                Token = token,
                ExpiryDate = DateTime.UtcNow.AddMinutes(10),
                IsExpired = false
            };
            await Database.refreshTokens.AddAsync(data);
            await Database.SaveChangesAsync();  
        }

        public async Task<bool> ValidateRefreshToken(string token)
        {
           var data=await Database.refreshTokens.FirstOrDefaultAsync(x=>x.Token==token);
             if(data != null && !data.IsExpired && data.ExpiryDate > DateTime.UtcNow)
             {
                return true; 
             }

             return false;
        }
    }

}
