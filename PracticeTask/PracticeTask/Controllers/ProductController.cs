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
    [Authorize(Roles = "Admin,User")]

    public class ProductController : ControllerBase
    {
        private readonly DataContext Database;
        public ProductController(DataContext database) { 
        
                Database = database;    
        }

        [HttpGet]
        [ResponseCache(Duration = 60)]
        public async Task<IActionResult> GetData(int PageNumber = 1,int PageSize=5)
        {
            var data = await Database.products.Where(x=>x.SoftDelete==false).Skip((PageNumber -1) * PageSize).Take(PageSize).ToListAsync();
          // var newdata=data.Select(x=>x.SoftDelete==false).ToList();
            
            if (data == null) { 
                return NotFound();  
            }

            return Ok(data);
        }
        [HttpPost]
        public async Task<IActionResult> PostData([FromBody] ProductDto productDto)
        {
            var data = new Product
            {
                    Name = productDto.Name,
                    Description = productDto.Description,
                    Price = productDto.Price,   
                    StockQuatity = productDto.StockQuatity, 
                    CategoryId = productDto.CategoryId,                           
            };   

            await Database.products.AddAsync(data); 
            Database.SaveChanges();
            return Ok(data);    
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDataById(int id)
        {       
            var data=await Database.products.FirstOrDefaultAsync(x=>x.Id==id && x.SoftDelete==false);
            if (data == null) {
                return NotFound();
            }
            
            return Ok(data);    
        }
        [HttpPut] 
        public async Task<IActionResult> GetupdateData(int id, [FromBody] ProductDto productDto)
        {
            var data = await Database.products.FirstOrDefaultAsync(x => x.Id == id && x.SoftDelete == false);
            if (data == null) {
                return NotFound();
            }
            data.Name  = productDto.Name;
            data.Description = productDto.Description;  
            data.Price = productDto.Price;
            data.CategoryId = productDto.CategoryId;
            await Database.SaveChangesAsync();
            return Ok(data);    
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteData(int id)
        {
            var data = await Database.products.FirstOrDefaultAsync(x => x.Id == id);
            if (data == null) {
                return NotFound();
            }
            data.SoftDelete = true;
            await Database.SaveChangesAsync();

            return Ok(data);
        }
    }
}
