using E_Commerce.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using E_Commerce.Model.Entities;
using E_Commerce.Model;

namespace E_Commerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class product : ControllerBase
    {
        private readonly DataContext Database;

        public product(DataContext Datbase)
        {
            Database = Datbase; 
        }
        [HttpGet]
        public async Task<IActionResult> Getdata(int page=1,int pagesize=5)
        {
            var data = await Database.products.ToListAsync();
            if (data == null) { 
                return NotFound();  
            }
            data = data.Skip((page - 1) * pagesize).Take(pagesize).ToList();
            return Ok(data);    
        }
        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> Getdata(int id)
        {
            var data=await Database.products.FirstOrDefaultAsync(x=>x.Id == id);  
                
            if (data == null) {
                return NotFound();
            }
            return Ok(data);
        }
        [HttpPost]  
        public async Task<IActionResult> PostData([FromBody]ProductDto productdto)
        {
            var data = new Product
            {
                Name = productdto.Name,
                Description = productdto.Description,   
                price=productdto.price,
                stock_quantity=productdto.stock_quantity, 
                CategoryID=productdto.CategoryID,
            };
            await Database.products.AddAsync(data);
            Database.SaveChanges();
            return Ok(data);    
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> UpdateData(int id, [FromBody]ProductDto productdto)
        {
            var data = await Database.products.FindAsync(id);
            data.Name = productdto.Name;
            data.price = productdto.price;
            data.Description = productdto.Description;
            data.stock_quantity = productdto.stock_quantity;
            data.CategoryID = productdto.CategoryID;
            await Database.SaveChangesAsync();
            return Ok(data);
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteData(int id)
        {
            var data=await Database.products.FindAsync(id);
            Database.products.Remove(data); 
            Database.SaveChanges(); 
            return Ok(data+" data are deleted");
        }
    }
}
