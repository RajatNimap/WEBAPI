


using Learning.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Learning.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly DataContext _context;

        public UserController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUser()
        {
            var data = await _context.user.ToListAsync();
            return Ok(data);

        }
        
        [HttpPost]  
        public async Task<IActionResult>

    }
}
