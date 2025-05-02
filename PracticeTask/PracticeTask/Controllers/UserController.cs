using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PracticeTask.Data;
using PracticeTask.Model;
using PracticeTask.Model.Entities;

namespace PracticeTask.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
       
    public class UserController : ControllerBase
    {
        private readonly DataContext Database;
        public UserController(DataContext database) {
                    
               Database = database;
        }
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Getdata(int page=1,int limit=5)
        {
            var data = await Database.userDetails.Where(x=>x.SoftDelete==false)
                .Skip((page-1)*limit).Take(limit)
                .ToListAsync();

            if (data == null) { 
                return NotFound("User not found");  
            
            }
            return Ok(data);

        }

        [HttpGet]
        [Route("{id}")]
        [Authorize]

        public async Task<IActionResult> GetResId(int id)
        {

            var data= await Database.userDetails.FirstOrDefaultAsync(x => x.Id == id);

            if (data == null) {
                return NotFound("data Not found");
            }
            return Ok(data);    
        } 

        [HttpPost]  
        public async Task<IActionResult> PostData(UserDetaildto userdto)
        {

            if(userdto == null)
            {
                return BadRequest("please enter detail properly");
            }
            var IsExist = await Database.userDetails.FirstOrDefaultAsync(x=>x.Email==userdto.Email);

            if (IsExist != null) {

                return BadRequest("User already exits");

            }  
            
            string HashesdPasword = BCrypt.Net.BCrypt.HashPassword(userdto.Password);

            var data = new UserDetail
            {
                Name = userdto.Name,    
                Age =userdto.Age,
                Email = userdto.Email,
                Password = HashesdPasword,

            };
            Database.userDetails.Add(data);
            await Database.SaveChangesAsync();  
            return Ok(data);    
        }
        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> UpdateData(int id, [FromBody] UserDetaildto userdto)
        {
            var data = await Database.userDetails.FirstOrDefaultAsync(x => x.Id == id);
            if (data == null) {
                return NotFound("Data not found");
            }
            string HashesdPasword = BCrypt.Net.BCrypt.HashPassword(userdto.Password);

            data.Name = userdto.Name;   
            data.Age = userdto.Age;
            data.Email = userdto.Email;
            data.Password = HashesdPasword;
            Database.SaveChanges();
            return Ok(data);    
            
        }
        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteData(int id)
        {

            var data = await Database.userDetails.FirstOrDefaultAsync(x => x.Id == id && x.SoftDelete==false);
            if (data == null) {
                return NotFound("User not exists");
            }
            // Database.userDetails.Remove(data);  
            data.SoftDelete = true;
            Database.SaveChanges();
            return Ok("data are deleted Successfully");    
        }

    }
}
