using E_Commerce.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class Orderitem : ControllerBase
    {
        private readonly DataContext Database;

        public Orderitem(DataContext database)
        {
            Database = database;
        }
        [HttpGet] 
        public async Task<IActionResult> GetData()
        {
            var data = await Database.orderitems.ToListAsync();
            return Ok(data);    
        }
        [HttpPost]  


    }
}
