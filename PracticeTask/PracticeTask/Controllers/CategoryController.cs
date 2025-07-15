using System.Net.Security;
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
    public class CategoryController : ControllerBase
    {
        private readonly DataContext Database;
        public CategoryController(DataContext data) { Database = data; }


        [HttpGet]
        public async Task<IActionResult> GetData()
        {
            var data = await Database.categories.Include(x=>x.Products).ToListAsync();

            if (data == null) {
                return NotFound("Category not found");
            }

            return Ok(data);
        }
        [HttpGet("{id}")]

        public async Task<IActionResult> GetDataByid(int id)
        {
            var data = await Database.categories.Include(x=>x.Products).FirstOrDefaultAsync(x => x.Id == id);
            if (data == null) {
                return NotFound("Data are not found");
            }

            return Ok(data);
        }

        [HttpPost]
        public async Task<IActionResult> PostData([FromBody]Categorydto cate)
        {
            var newdata = await Database.categories.FirstOrDefaultAsync(x => x.Name == cate.Name);
            if (newdata != null) {
                    return BadRequest("category already exists");
            }
            var data = new Category
            {
                Name = cate.Name
            };

            await Database.categories.AddAsync(data);
            Database.SaveChanges();
            return Ok(data);

        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateData(int id,[FromBody] Category dto)
        {
              var data = await Database.categories.FirstOrDefaultAsync(x=>x.Id == id);
              if (data == null) {
                return NotFound("Category  is not found");
              }
              data.Name = dto.Name;
              await Database.SaveChangesAsync();    
              return Ok(data);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteData(int id)
        {
            var data = await Database.categories.FirstOrDefaultAsync(x => x.Id == id);
            if (data == null) {
                return NotFound("category not found");
            }
            Database.categories.Remove(data);
            await Database.SaveChangesAsync();
            return Ok(data);
        }
        [HttpGet("test")]

        public async Task<IActionResult> Testing()
        {

            //Example of Enumerable and IQuearble
            //IQueryable<Category> data = Database.categories.Where(x => x.Id > 1);
            //IEnumerable<Category> data1 = Database.categories.ToList();

            //Here is the lazy and eager Loading 
            // THis is by default
          //  var data1 = Database.categories.ToList();

            // this is the eager loading
           // var data = Database.categories.Include(x => x.Products).ToList();
            // Now we are heading to the Lazy Loading
            // for the lazy loading first install proxies and make the navigation property virual


            List<Category> data2= Database.categories.ToList();
            var data3 = data2.Select(x => new
            {
                x.Id,
                x.Name,
                Products = x.Products.Select(p => p.Name).ToList()
            });
            return Ok(data3);        

                
        }
    }
}
