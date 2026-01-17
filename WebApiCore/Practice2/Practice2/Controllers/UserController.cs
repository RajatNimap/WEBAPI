using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Practice2.Data;
using Practice2.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime.Intrinsics.Arm;
using System.Security.Claims;
using System.Text;

namespace Practice2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        public readonly DataContext _db;
        private readonly IConfiguration configuration;
        public UserController(DataContext db,IConfiguration config)
        {
            _db = db;
            configuration = config;
        }

        [HttpPost]
        public async Task<IActionResult> addUser(UserModel user)
        {
            var data = new UserModel
            {
                UserName = user.UserName,
                Name = user.Name,
                Password = user.Password,
                Role = user.Role,

            };
            await _db.userModels.AddAsync(data);
            _db.SaveChanges();
            return Ok("user created");

        }
        [HttpPost("Login")]

        public async Task<IActionResult> login(string userName, string password) {

            var verify = await _db.userModels.FirstOrDefaultAsync(x => x.Name == userName);

            if (verify == null) {
                return BadRequest("not verify");
            }
            var token = jwtToken(verify);

            return Ok(token);
        }

        private string jwtToken(UserModel user)
        {
            var data = configuration["JWT:Key"];
            var symetricKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Key"]));
            var credential = new SigningCredentials(symetricKey, SecurityAlgorithms.HmacSha256);

            var Claimes = new[]
            {
                new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Name,user.Name),
                new Claim(ClaimTypes.Role,user.Role)

            };
            var Token = new JwtSecurityToken(
                        issuer: configuration["JWT:Issuer"],
                        audience: configuration["JWT:Audience"],
                        claims: Claimes,
                        expires: DateTime.UtcNow.AddDays(2),
                        signingCredentials: credential

             );

            return new JwtSecurityTokenHandler().WriteToken(Token);

        } 

    }

}
