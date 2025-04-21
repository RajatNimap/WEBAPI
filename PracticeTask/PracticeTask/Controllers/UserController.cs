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
        public async Task<IActionResult> Getdata()
        {
            var data = await Database.userDetails.ToListAsync();

            if (data == null) { 
                return NotFound("User not found");  
            
            }
            return Ok(data);

        }

        [HttpGet]
        [Route("{id}")]

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
            var data = new UserDetail
            {
                Name = userdto.Name,    
                age =userdto.age,
                Email = userdto.Email,
                Password = userdto.Password,

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
            data.Name = userdto.Name;   
            data.age = userdto.age;
            data.Email = userdto.Email;
            data.Password = userdto.Password;
            Database.SaveChanges();
            return Ok(data);    
            
        }
        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteData(int id)
        {

            var data = await Database.userDetails.FirstOrDefaultAsync(x => x.Id == id);
            if (data == null) {
                return NotFound();
            }
            Database.userDetails.Remove(data);  
            Database.SaveChanges();
            return Ok(data);    
        }
    }
}
