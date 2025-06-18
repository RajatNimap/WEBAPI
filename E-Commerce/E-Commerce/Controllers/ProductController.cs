using E_Commerce.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using E_Commerce.Model.Entities;
using E_Commerce.Model;
using Microsoft.VisualBasic;
using System.Collections.Immutable;
using E_Commerce.Filters;

namespace E_Commerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly DataContext Database;

        public ProductController(DataContext Datbase)
        {
            Database = Datbase; 
        }
        [HttpGet]
        // [AgeauthorizationFilter(5)]
        // [TypeFilter(typeof(ResourceFilter))]
        [ServiceFilter(typeof(ResultFIlter))]


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
        [TypeFilter(typeof(ResourceFilter))]
        public async Task<IActionResult> Getdata(int id)
        {   
            var data=await Database.products.FirstOrDefaultAsync(x=>x.Id == id);  
            
            if (data == null)
            {
                return NotFound("data not found");
                //throw new Exception("Data not Found");
            }
            return Ok(data);
        }
        [HttpGet("filter")]
        public async Task<IActionResult> Getfilterdata([FromQuery] string ProductFilter, [FromQuery] float? minprice, [FromQuery] float? maxprice)
        {
                  
            var query = Database.products.AsQueryable();
            if (!string.IsNullOrEmpty(ProductFilter))
            {
                query = query.Where(x => x.Name.ToLower() == ProductFilter.ToLower());
            }

            if (minprice.HasValue)
            {
                query = query.Where(x => x.price > minprice);
            }
            if (maxprice.HasValue) { 
                    query=query.Where(x => x.price < maxprice); 
            }
            query = query.Where(x => x.Soft_delete == 0);
            var data= await query.ToListAsync();  

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
                stock_quantity=productdto.stock_quantity  > 0 ? productdto.stock_quantity:1, 
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
        public async Task<IActionResult> UpdateData(int id, [FromBody] ProductDto productdto)
        {
            try
            {
                var data = await Database.products.FirstOrDefaultAsync(x => x.Id == id && x.Soft_delete == 0);

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
            catch (Exception ex)
            {

                return BadRequest("data not found");
            }
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
