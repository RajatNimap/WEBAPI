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
    public class CategoryController : ControllerBase
    {
        private readonly DataContext Database;
        public CategoryController(DataContext data)
        {
            Database = data;
        }
        [HttpGet]
        public async Task<IActionResult> GetData()
        {
            
            var data = await Database.categories.Include(x => x.products)
                .ToListAsync();
            if (data == null)
            {
               
                return NotFound();
            }
            return Ok(data);
        }
       

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetDataParticular(int id)
        {
            var data = await Database.categories.Include(x=>x.products).FirstOrDefaultAsync(x => x.Id == id);
            if (data == null)
            {

                return NotFound(id);
            }

            return Ok(data);
        }
        [HttpGet]
        [Route("{id}/products")]
        public async Task<IActionResult> GetProductCategory(int id)
        {
            var data = await Database.categories.AnyAsync(x => x.Id == id);
            var newdata =await Database.products.Where(x => x.CategoryID == id).ToListAsync();
            return Ok(newdata);

        }
        [HttpPost]
        public async Task<IActionResult> PostData([FromBody] CategoryDto catedto)
        {

            var IsExist=Database.categories.FirstOrDefault(x=>x.Name == catedto.Name);   
            if(IsExist != null)
            {
                return Conflict(new {message="category already exits"});
            }
            var data = new Category { Name = catedto.Name };

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); 
            }
            //if (data == null) {
            //    return BadRequest();
            //}
            await Database.categories.AddAsync(data);
            Database.SaveChanges();
            return Ok(data);
        }
        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> UpdataDatabase(int id, [FromBody] CategoryDto catedto)
        {
            var data = await Database.categories.FindAsync(id);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            data.Name = catedto.Name;
            Database.SaveChanges();
            return Ok(data);    
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteData(int id)
        {
            var data=await Database.categories.FindAsync(id);
            if (data == null) { return NotFound("category not exist");  };
            Database.categories.Remove(data);
            Database.SaveChanges();
            return Ok("category are deleted");

        }
    }
}
