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
    public class subcategory : ControllerBase
    {
        private readonly DataContext Database;
        public subcategory(DataContext data) {

            Database = data;
        }

        [HttpGet]
        public async Task<IActionResult> Getdata()
        {
            var data = await Database.subcategories.Include(x=>x.products).ToListAsync();
            return Ok(data);
        }
        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetParticularData(int id)
        {
            var data = await Database.subcategories.FirstOrDefaultAsync(x => x.Id == id);
            if (data == null)
            {
                return NotFound();
            }
            return Ok(data);

        }
        [HttpPost]
        public async Task<IActionResult> PostData([FromBody] SubCategoryDto subcatdto)
        {
            var data = new SubCategory { Name = subcatdto.Name, CategoryId = subcatdto.CategoryId };
            await Database.subcategories.AddAsync(data);
            Database.SaveChanges();
            return Ok(data);

        }
        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> UpdateData(int id,[FromBody] SubCategoryDto subcatdto)
        {
            var data = await Database.subcategories.FindAsync(id);
            data.Name= subcatdto.Name;  
            data.CategoryId=subcatdto.CategoryId;
            await Database.SaveChangesAsync();  
            return Ok(data);    
        }
        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteData(int id)
        {
            var data= await Database.subcategories.FindAsync(id);
            Database.subcategories.Remove(data);
            await Database.SaveChangesAsync();
            return Ok(data+" data deleted succesfully");    
        }

    }
}
