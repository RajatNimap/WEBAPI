using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PracticeTask3.Data;
using PracticeTask3.Model;
using PracticeTask3.Model.Entities;

namespace PracticeTask3.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BasicCrudController : ControllerBase
    {

        private readonly DataContext Database;  
        public BasicCrudController(DataContext database) { 
            Database = database;
        }    
        [HttpGet]
        [Route("Getstudent")]

        public async Task<IActionResult> GetData()
        {

            var Studentdata = await Database.students.ToListAsync();    
            
            if(Studentdata == null)
            {
                return NotFound("Data not found in the database");
            }
            return Ok(Studentdata);
        }

        [HttpPost]
        [Route("student")]
        public async Task<IActionResult> PostData(Studentdto stu) 
        {

            if(stu == null)
            {
                return BadRequest();
            }
            var Hashed=BCrypt.Net.BCrypt.HashPassword(stu.Password);
            var data = new Student
            {
                    Name = stu.Name,
                    Email = stu.Email,
                    Password = Hashed,
            };

            await Database.students.AddAsync(data); 
            await Database.SaveChangesAsync();      

            return Ok(data);
        }
    }
}
