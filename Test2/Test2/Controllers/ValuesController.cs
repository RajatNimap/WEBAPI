using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Test2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly DataContext _dataContext;
        public ValuesController(DataContext dataContext) { 
                _dataContext = dataContext;
        
        }

        [HttpPost]

        public async Task<IActionResult> addorupdate(Customer customer)
        {

           var finding=await _dataContext.customers.FirstOrDefaultAsync(x=>x.Email==customer.Email);
            

            if (finding == null) {
                var data = new Customer
                {
                    Name = customer.Name,   
                    Email = customer.Email,

                };

               await _dataContext.customers.AddAsync(data);
                _dataContext.SaveChanges();
            }else{
                var data = new Customer
                {
                    Name = customer.Name,
                    Email = customer.Email,

                };
                _dataContext.SaveChanges();
            }
            return Ok();

        }

        [HttpPost("/category")]
        public async Task<IActionResult> addorupdate2(Category category)
        {

            var finding = await _dataContext.categories.FirstOrDefaultAsync(x => x.Id == category.Id);


            if (finding == null)
            {
                var data = new Category
                {
                    Name = category.Name,
                    

                };

                await _dataContext.categories.AddAsync(data);
                _dataContext.SaveChanges();
            }
            else
            {
                var data = new Category
                {
                    Name = category.Name,
                   

                };
                _dataContext.SaveChanges();
            }
            return Ok();

        }
        [HttpPost("/product")]
        public async Task<IActionResult> addorupdate3(Product product)
        {

            var finding = await _dataContext.products.FirstOrDefaultAsync(x => x.Id == product.Id);


            if (finding == null)
            {
                var data = new Product
                {
                    Name = product.Name,    
                    Price = product.Price,  
                    Category = product.Category,
         


                };

                await _dataContext.products.AddAsync(data);
                _dataContext.SaveChanges();
            }
            else
            {
                var data = new Product
                {
                    Name = product.Name,
                    Price = product.Price,
                    Category = product.Category,



                };
                _dataContext.SaveChanges();
            }
            return Ok();
        }
        [HttpPost("/Orders")]

        public async Task<IActionResult> Orders(Orders orders)
        {
            var data = new Orders
            {
                Id = orders.Id,
                Address = orders.Address,
                ProductId = orders.ProductId,

            };

            _dataContext.orders.AddAsync(data);

            return Ok();    
        }





    }
}
