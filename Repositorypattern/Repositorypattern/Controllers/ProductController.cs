using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Repositorypattern.Repository;
using Repositorypattern.Model;

namespace Repositorypattern.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {

        private readonly IRepository<Product> _repository;
        public ProductController(IRepository<Product> repository)
        {
            _repository = repository;
        }


        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            var products = await _repository.GetAllAsync();
            return Ok(products);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            var product = await _repository.GetELementById(id);
            if (product == null)
            {
                return NotFound();
            }
            return Ok(product);
        }
        [HttpPost]
        public async Task<IActionResult> AddProduct(ProductDto product)
        {
            if (product == null)
            {
                return BadRequest("Product cannot be null");
            }
            var newProduct = new Product
            {
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                CategoryId = product.CategoryId
            };
            var createdProduct = await _repository.AddAsync(newProduct);
            return CreatedAtAction(nameof(GetProductById), new { id = createdProduct.Id }, createdProduct);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, ProductDto productdto)
        {
            
            var existingProduct = await _repository.GetELementById(id);
            if (existingProduct == null)
            {
                return NotFound();
            }
                
            var products= new Product
            {
                Name = productdto.Name,
                Description = productdto.Description,
                Price = productdto.Price,
                CategoryId = productdto.CategoryId
            };

            await _repository.Update(products);
            return NoContent();
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteRepo(int id)
        {

            var existingProduct = await _repository.GetELementById(id);
            if (existingProduct == null) {
                return null;
            }
            await _repository.DeleteAsync(existingProduct);
            return NoContent();
        }
        [HttpGet("count")]
        public async Task<IActionResult> GetCount()
        {
            var count = await _repository.GetAllAsync();
            if (count == null)
            {
                return NotFound();
            }
            return Ok(count.Count());
        }   
    }
}
