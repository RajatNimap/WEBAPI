using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Test.Entities;

namespace Test.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {


        public Datacontext db;
        public OrderController(Datacontext db)
        {

            this.db = db;
        }

        [HttpPost]

        public async Task<IActionResult> GiveOrders(Orders orders)
        {

            var data = new Orders
            {
                address = orders.address,
                ProductId = orders.ProductId,
                CustomerId = orders.CustomerId,

            };

            await db.AddAsync(orders);
            db.SaveChanges();
            return Ok(orders);
        }

        [HttpGet("{getmaxofproduct}")]

        public async Task<IActionResult> Maxdata()
        {

            var data = from order in db.Orders
                       join pro in db.Products 



            return Ok();   
        }


    }
}
