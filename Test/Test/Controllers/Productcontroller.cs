using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Test.Entities;

namespace Test.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Productcontroller : ControllerBase
    {
        public Datacontext db;

        public Productcontroller(Datacontext db)
        {
            this.db = db;
        }



        [HttpGet]

        public async Task<IActionResult> Getdata()
        {

            var data = db.Products.ToListAsync();

            return Ok(data);
        }


        [HttpPost]

        public async Task<IActionResult> PostDataorupdate(product product, int id = 0)
        {

            var data = await db.customers.FirstOrDefaultAsync(x => x.Id == product.Id && x.Id != 0);

            if (data == null)
            {
                var Data = new product()
                {

                    Name = product.Name,
                    Description = product.Description,  
                    price=product.price
                    

                };
                db.AddAsync(Data);
                db.SaveChanges();

            }
            else
            {
                var Data = new product()
                {

                    Name = product.Name,
                    Description = product.Description,
                    price = product.price

                };
                db.SaveChanges();

            }
            return Ok(data);


        }

    }
}
