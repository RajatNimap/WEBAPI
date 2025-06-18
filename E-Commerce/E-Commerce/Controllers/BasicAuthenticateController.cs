using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using E_Commerce.Data;
using E_Commerce.Model;
using E_Commerce.Model.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.IdentityModel.Tokens;

namespace E_Commerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BasicAuthenticateController : ControllerBase
    {
        private readonly DataContext Database;
        private readonly IConfiguration _config;
        public BasicAuthenticateController(DataContext data, IConfiguration config)
        {

            Database = data;
            _config = config;
        }

        [HttpPost("JWTAuthentication")]
        public async Task<IActionResult> BasicAuthication(UserBasicAuth userauth)
        {

            if (userauth.Email == null || userauth.Password == null)
            {
                return NotFound("please enter the detail");
            }
            var data = await Database.users.FirstOrDefaultAsync(x => x.Email == userauth.Email);


            if (data == null)
            {
                return NotFound("User not found");

            }

            bool isValid = BCrypt.Net.BCrypt.Verify(userauth.Password, data.Password);
            if (isValid)
            {
                var tokenstring = GenerateWebToken(userauth);
                var RefreshToken = GenerateRefreshToken();

                var ExistingEntry = await Database.refreshTokens.Where(x => x.Email == userauth.Email && !x.IsRevoked).ToListAsync();
                foreach (var entry in ExistingEntry)
                {

                    entry.IsRevoked = true;
                }
                var TokenData = new RefreshToken
                {
                    Token = RefreshToken,
                    Email = userauth.Email,
                    ExpiryTime = DateTime.UtcNow.AddMinutes(10),
                    IsRevoked = false,  
                };
               
                await Database.refreshTokens.AddAsync(TokenData);
                Database.SaveChanges();

                return Ok(new { token = tokenstring , refresh=RefreshToken} + " User successfully login");
            }
            else
            {
                return BadRequest("Invalid Credential");
            }
        }
        [HttpPost("RefreshToken")]
        public async Task<IActionResult>GetAuthentication(RefreshTokenDto refresh)
        {
            
            var data = await Database.refreshTokens.FirstOrDefaultAsync(x => x.Token == refresh.RefreshToken && !x.IsRevoked && x.ExpiryTime > DateTime.UtcNow);
            if (data == null) { return Unauthorized("Invalid or expired token"); }

            var User = await Database.users.FirstOrDefaultAsync(x => x.Email == data.Email);
            if (User == null) { return Unauthorized("User Not found"); }

            var NewAccessToken = GenerateWebToken(new UserBasicAuth {Email=User.Email} );
            var NewRefreshToken = GenerateRefreshToken();

            var tokendata = new RefreshToken
            {
                Token = NewRefreshToken,
                Email= User.Email,  
                ExpiryTime= DateTime.UtcNow.AddMinutes(10),
                IsRevoked = false,

            };

           await Database.refreshTokens.AddAsync(tokendata);
           Database.SaveChanges(); return Ok(new {NewAccessToken,NewRefreshToken });
        }

        private String GenerateWebToken(UserBasicAuth userInfo)
        {
            var Data = Database.users.FirstOrDefault(x => x.Email == userInfo.Email);
            var securitykey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));

            var credentials = new SigningCredentials(securitykey, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
               new Claim(JwtRegisteredClaimNames.Email,userInfo.Email),
               new Claim("Age",Data.age.ToString()),
            };
            var token = new JwtSecurityToken(
            _config["Jwt:Issuer"],
            _config["Jwt:Audience"],
            claims,
            expires: DateTime.Now.AddMinutes(5),
            signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string GenerateRefreshToken()
        {
           var RandomBytes=new byte[32];
            using var Range = RandomNumberGenerator.Create();
            Range.GetBytes(RandomBytes);    
            return Convert.ToBase64String(RandomBytes);
        }


        
    }
}
