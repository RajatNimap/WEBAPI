using Crudoperation.Data;
using Crudoperation.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Crudoperation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

  
    public class BasicCrud : ControllerBase
    {
        private readonly DataContext Database;
        public BasicCrud(DataContext database) { 
                Database = database;
        }
        [HttpGet]
        public async Task<IActionResult> GetData()
        {
            var data =await Database.emps.ToListAsync();
            return Ok(data);    
        } 
    }
}
