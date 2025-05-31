using AutoMapper;
using E_Commerce.Data;
using E_Commerce.Model;
using E_Commerce.Model.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;

namespace E_Commerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly DataContext Database;
        private readonly IMapper mapper;
        private readonly IMemoryCache _cache;
        private readonly TimeSpan cacheExpiration = TimeSpan.FromMinutes(1);
        private readonly ILogger<CategoryController> _logger;

        public CategoryController(DataContext data,IMapper mapper,IMemoryCache cache,ILogger<CategoryController> logger)
        {
            Database = data;
            this.mapper = mapper;   
            _cache = cache;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetData()
        {
            //var data = await Database.categories.Include(x => x.products)
            //.ToListAsync();

            var cacheKey = "Category";
            if(_cache.TryGetValue(cacheKey, out var result)) {
                _logger.LogInformation("this is retrive from the cache");
                return Ok( new {res= result,sources="Cache"}); 
            } 
            

                var data = await Database.categories.Select(c => new
                {
                    c.Id,
                    c.Name,
                    product = c.products.Where(x => x.Soft_delete == 0).ToList()
                }).ToListAsync();
                if (data == null)
                {

                    return NotFound();
                }
            _logger.LogInformation("This is from the Datbase");
             _cache.Set(cacheKey, data, cacheExpiration);


            return Ok(new
            {
                data1 = data,
                sources = "Database"
            });
        }
        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetDataParticular(int id)
        {
            var data = await Database.categories.Include(x => x.products.Where(p=>p.Soft_delete==0)).FirstOrDefaultAsync(x => x.Id == id);
           // var newdata = await Database.categories.Select(x => x.Id == id);
           
            if (data == null)
            {

                return NotFound(id);
            }

            return Ok(data);
        }
        [HttpGet]
        [Route("products/{id}")]
        public async Task<IActionResult> GetProductCategory(int id)
        {
            //var data = await Database.categories.AnyAsync(x => x.Id == id);

         
            var newdata =await Database.products.Where(x => x.CategoryID == id && x.Soft_delete==0).ToListAsync();

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
            var data =mapper.Map<Category>(catedto); 
            //var data = new Category { Name = catedto.Name };

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
