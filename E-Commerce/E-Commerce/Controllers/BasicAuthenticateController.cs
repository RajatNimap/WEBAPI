using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using E_Commerce.Data;
using E_Commerce.Model;
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
                return Ok(new { token = tokenstring } + " User successfully login");
            }
            else
            {
                return BadRequest("Invalid Credential");
            }
        }

        private String GenerateWebToken(UserBasicAuth userInfo)
        {
            var securitykey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));

            var credentials = new SigningCredentials(securitykey, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
               new Claim(JwtRegisteredClaimNames.Email,userInfo.Email)
            };
            var token = new JwtSecurityToken(
            _config["Jwt:Issuer"],
            _config["Jwt:Audience"],
            claims,
            expires: DateTime.Now.AddMinutes(120),
            signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        //[HttpPost("BasicAuthenticataion")]
        //public async Task<IActionResult> BasiscAuthenticate(UserBasicAuth userInfo)
        //{
        //    var data =await Database.users.FirstOrDefaultAsync(x=>x.Email==userInfo.Email);
        //    if (data == null) {
        //        return NotFound("User Not found");
        //    }
        //    bool isValid =BCrypt.Net.BCrypt.Verify(data.Password, userInfo.Password);

        //    if (!isValid) {

        //        return NotFound("Invalid Credential");
        //    }
        //    return Ok("User login Succefully");

        //}
    }
}
