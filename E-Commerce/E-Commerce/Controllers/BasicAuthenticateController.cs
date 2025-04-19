using E_Commerce.Data;
using E_Commerce.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace E_Commerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BasicAuthenticateController : ControllerBase
    {
        private readonly DataContext Database;
        public BasicAuthenticateController(DataContext data) { 
            Database = data;
        }

        [HttpPost("Authentication")]
        public async Task<IActionResult> BasicAuthication(UserBasicAuth userauth)
        {
            if (userauth.Email == null || userauth.Password == null) {
                return NotFound("please enter the detail");
            }
            var data = await Database.users.FirstOrDefaultAsync(x=>x.Email == userauth.Email);

            if(data ==null)
            {
                return NotFound("User not found");
            }
            bool isValid=BCrypt.Net.BCrypt.Verify(userauth.Password, data.Password);    
            if (isValid) {

                return Ok("User successfully login");
            }
            else
            {
                return BadRequest("Invalid Credential");
            }
        }
    }
}
