using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Test.Entities;

namespace Test.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class cutomercontroller : ControllerBase
    {

        public Datacontext db;
        public cutomercontroller(Datacontext db)
        {

            this.db = db;
        }

        [HttpGet]

        public async Task<IActionResult> Getdata()
        {

            var data = db.customers.ToListAsync();

            return Ok(data);
        }


        [HttpPost]
        
        public async Task<IActionResult> PostDataorupdate(customer customer,int id =0)
        {

            var data= await db.customers.FirstOrDefaultAsync(x=>x.Id==customer.Id && x.Id !=0);

            if (data == null)
            {
                var Data = new customer()
                {

                    Name = customer.Name,
                    email = customer.email,

                };
                db.AddAsync(Data);
                db.SaveChanges();

            }
            else
            {
                var Data = new customer()
                {

                    Name = customer.Name,
                    email = customer.email,

                };
                db.SaveChanges();

            }
            return Ok(data);        
            

        }
        //[HttpDelete("{id}")]
        //public async Task<IActionResult> Deletedata(int id)
        //{
        //    var data=await db.customers.FirstOrDefaultAsync(x=>x.Id == id);   
            
        //       db.RemoveAsync(data);    
        //}

    }
}
