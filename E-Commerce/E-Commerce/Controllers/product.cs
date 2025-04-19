using E_Commerce.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using E_Commerce.Model.Entities;
using E_Commerce.Model;
using Microsoft.VisualBasic;

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
            data = data.Where(x=>x.Soft_delete==0)
                .Skip((page - 1) * pagesize)
                .Take(pagesize)
                .ToList();           
            return Ok(data);    
        }
        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> Getdata(int id)
        {
            var data=await Database.products.FirstOrDefaultAsync(x=>x.Id == id && x.Soft_delete==0);  
            
            
            if (data == null)
            {
                //return NotFound("data not found");
                throw new Exception("Data not Found");
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

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
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

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            await Database.SaveChangesAsync();
            return Ok(data);
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteData(int id)
        {
            var data=await Database.products.FindAsync(id);
            if(data == null)
            {
                return NotFound("Product not exist");
            }

            if (data.Soft_delete == 1)
            {
                return NotFound("Product not exist");
            }
            data.Soft_delete = 1;
            //Database.products.Remove(data); 
            Database.SaveChanges(); 
            return Ok("Product are deleted");
        }
       
    }
}
