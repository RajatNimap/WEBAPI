using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Test.Entities;

namespace Test.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {


        public Datacontext db;
        public CategoryController(Datacontext db)
        {

            this.db = db;
        }


        [HttpPost]

        public async Task<IActionResult> PostDataorupdate(category category, int id = 0)
        {

            var data = await db.customers.FirstOrDefaultAsync(x => x.Id == category.Id && x.Id != 0);

            if (data == null)
            {
                var Data = new category()
                {

                    Name = category.Name,

                };
                db.AddAsync(Data);
                db.SaveChanges();

            }
            else
            {
                var Data = new category()
                {

                    Name = category.Name,

                };
                db.SaveChanges();

            }
            return Ok(data);


        }

    }
}
