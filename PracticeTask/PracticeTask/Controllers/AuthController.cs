using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PracticeTask.Data;
using PracticeTask.Model;

namespace PracticeTask.Controllers
{
    [Route("api/[controller]")]
    [ApiController]


    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly DataContext Database;
        public AuthController(DataContext database, IConfiguration configuration)
        {
            Database = database;
            _configuration = configuration;
        }
        [HttpPost("LoginAuth")]
        public async Task<IActionResult> LoginAuth([FromBody]LoginReqDto logindto)
        {
            var data=await Database.userDetails.FirstOrDefaultAsync(x=>x.Email == logindto.Email);
            if (data == null) {
                return NotFound("user Not found");
            }
            bool isValid=BCrypt.Net.BCrypt.Verify( logindto.Password, data.Password);
            
            if (isValid)
            {
                var token = GenerateJwtToken(logindto);
                return Ok(token+" User Login Successfully");
            }
            else
            {
                return BadRequest("Invalid crenditials");
            }

        }

        private string GenerateJwtToken([FromBody]LoginReqDto logindto)
        {
            var securitykey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

            var SigningKey= new SigningCredentials(securitykey,SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Email, logindto.Email),
            };

            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims,
                expires: DateTime.UtcNow.AddMinutes(15),
                signingCredentials:SigningKey
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        private string GenerateToken([FromBody] LoginReqDto Login)
        {
            var securitykey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Issuer"]));
            var credential = new SigningCredentials(securitykey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Email,Login.Email),
            };

            var token = new JwtSecurityToken(
             _configuration["Jwt:Issuer"],
             _configuration["Jwt:Audience"],
             claims,
             signingCredentials:credential
                
             );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
