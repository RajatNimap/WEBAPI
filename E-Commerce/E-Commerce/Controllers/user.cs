using E_Commerce.Data;
using E_Commerce.Model;
using E_Commerce.Model.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class User : ControllerBase
    {
        private readonly DataContext Database;

        public User(DataContext _Data1)
        {
            Database = _Data1;

        }
        [HttpGet]
        public async Task<IActionResult> GetData()
        {
            var data =  await Database.users.ToListAsync();

            if (data == null) {
                return NotFound(); 
             }
            return Ok(data);    
        }
        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetDataParticular(int id)
        {
            var data =await Database.users.FirstOrDefaultAsync(x => x.Id == id);
            if (data == null) { 
                return NotFound();
            }
            return Ok(data);
        }
        [HttpPost]
        public async Task<IActionResult> PostData([FromBody]UserDto newDto)
        {
            var data = new Model.Entities.User
            {   Name = newDto.Name,
                Email = newDto.Email,
                Phone = newDto.Phone,   
                age= newDto.age,
                Password= newDto.Password,
            };
             await Database.users.AddAsync(data);
             Database.SaveChanges(); 
             return Ok(data);
        }
        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult>UpdateData(int id, [FromBody]UserDto newDto)
        {
            var data = await Database.users.FindAsync(id);
            if (data == null) { return NotFound(); }
            data.Name = newDto.Name;
            data.Email = newDto.Email;
            data.Phone = newDto.Phone;  
            data.age = newDto.age;  
            data.Password = newDto.Password;    
            Database.SaveChanges();
            return Ok(data);    
        }

        [HttpDelete]    
        public async Task<IActionResult> DeleteData(int id)
        {
            var data = await Database.users.FindAsync(id);
            if (data == null) { return NotFound(); }
            Database.users.Remove(data); ;
            Database.SaveChanges();
            return Ok(data+" This data are deleted ");
        }

    }
}
