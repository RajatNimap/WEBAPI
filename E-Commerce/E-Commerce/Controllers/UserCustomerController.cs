using System.Runtime.CompilerServices;
using E_Commerce.Data;
using E_Commerce.Model;
using E_Commerce.Model.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserCntrl : ControllerBase
    {
        private readonly DataContext Database;
        public UserCntrl(DataContext _Data1)
        {
            Database = _Data1;
        }
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetData(int page=1,int pagesize = 10)
        {
            var data = await Database.users.ToListAsync();   
            if (data == null) {
                return NotFound("data not found"); 
            }

            data=data.Where(x=>x.Soft_delete==0)
                .Skip((page-1)*pagesize).Take(pagesize).ToList();
            return Ok(data);    
        }
        [HttpGet]
        [Route("{id}")]
        [Authorize]

        public async Task<IActionResult> GetDataParticular(int id)
        {
            try
            {
                var data = await Database.users.FirstOrDefaultAsync(x => x.Id == id && x.Soft_delete == 0);
                if (data == null)
                {
                    return NotFound();
                }

                return Ok(data);
            }
            catch (Exception ex) {
                return StatusCode(500,"An error occured while fetching the data");
            }
        }
        [HttpGet("orders/{id}")]
        public async Task<IActionResult> OrderItem(int id)
        {
            
            var data = await Database.orders.Include(x => x.OrderItems).Where(x=>x.UserId==id).ToListAsync();
            return Ok(data);    
        }
        [AllowAnonymous]
        [HttpPost]

        public async Task<IActionResult> PostData([FromBody]UserDto newDto)
        {
            try
            {
                var IsExist = Database.users.FirstOrDefault(x => x.Email == newDto.Email || x.Phone == newDto.Phone);
                if (IsExist != null)
                {
                    return Conflict(new { message = "User already exits" });
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                string HasedPassword = BCrypt.Net.BCrypt.HashPassword(newDto.Password);

                var data = new User
                {
                    Name = newDto.Name,
                    Email = newDto.Email,
                    Phone = newDto.Phone,
                    age = newDto.age,
                    Password = HasedPassword,
                };

                await Database.users.AddAsync(data);
                Database.SaveChanges();
                return Ok(data);

            }
            catch (Exception ex) {

                return StatusCode(500, "An error occured while feteching the data");
            }
        }
        [HttpPut]
        [Route("{id}")]
        [Authorize]
        public async Task<IActionResult>UpdateData(int id, [FromBody]UserDto newDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                string HashedPassword=BCrypt.Net.BCrypt.HashPassword(newDto.Password);  
                
                var data = await Database.users.FirstOrDefaultAsync(x=>x.Id==id && x.Soft_delete==0);
                if (data == null) { return NotFound(); }
                data.Name = newDto.Name;
                data.Email = newDto.Email;
                data.Phone = newDto.Phone;
                data.age = newDto.age;

               
                data.Password = HashedPassword;


                Database.SaveChanges();
                return Ok(data);
            }
            catch(Exception ex)
            {
                return StatusCode(500, "An error occured while fetching the data");
            }
        }
        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> DeleteData(int id)
        {
            try
            {
                var data = await Database.users.FindAsync(id);
                if (data == null) { return NotFound("User not existed"); }
                //Database.users.Remove(data); 
                if (data.Soft_delete == 1)
                {
                    return NotFound("User not existed");
                }

                data.Soft_delete = 1;

                Database.SaveChanges();
                return Ok(" This data are deleted ");
            }
            catch (Exception ex) {
                return StatusCode(500, "An error occure while feteching the data");
            }
        }

    }
}
