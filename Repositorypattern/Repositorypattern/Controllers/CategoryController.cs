using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Repositorypattern.Repository;
using Repositorypattern.Model;
namespace Repositorypattern.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    
    public class CategoryController : ControllerBase
    {
        private readonly IRepository<Category> _repository;
        public CategoryController(IRepository<Category> repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCategories()
        {
            var categories = await _repository.GetAllAsync();
            return Ok(categories);
        }
        [HttpPost]
        public async Task<IActionResult> AddingCategory(Categorydto cat)
        {
            if (cat == null) {
                return BadRequest();
            }
            var data  = new Category { Name = cat.Name };   
            var createdCategory = await _repository.AddAsync(data);
            
            return CreatedAtAction(nameof(GetAllCategories), new { id = createdCategory.Id }, createdCategory);

        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            var category = await _repository.GetELementById(id);
            if (category == null)
            {
                return NotFound();
            }
            return Ok(category);
        }

    }
}
