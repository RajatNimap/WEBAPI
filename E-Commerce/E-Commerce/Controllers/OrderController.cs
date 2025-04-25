using E_Commerce.Data;
using E_Commerce.Model;
using E_Commerce.Model.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace E_Commerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class OrderController : ControllerBase
    {
        private readonly DataContext Database;

        public OrderController(DataContext Data) {
            Database = Data;
        }

        [HttpPost("placeorder")]
        public async Task<IActionResult> OrderReq(OrderRequest orderRequest)
        {
            if (orderRequest == null || orderRequest.Items == null)
            {
                return BadRequest("order request in invalied");
            }

            var user = await Database.users.FirstOrDefaultAsync(x=>x.Id==orderRequest.UserId && x.Soft_delete==0);
            if (user == null) {

                return BadRequest("User Not Found");
            }

            var order = new Order
            {
                UserId = orderRequest.UserId,
                Address=orderRequest.Address,   
                OrderDate = DateTime.Now,
            };

            Database.orders.Add(order);
            await Database.SaveChangesAsync();
            var orderItems = new List<OrderItem>();
            double TotalPrice = 0;

            foreach (var item in orderRequest.Items) {
                var product = await Database.products.FirstOrDefaultAsync(x=>x.Id==item.ProductId && x.Soft_delete==0);
                if (product == null) {
                    return BadRequest("Product Not found");
                }
                if(item.Quantity < 1)
                {
                    return BadRequest("stock quantity at least 1");
                }
                if (product.stock_quantity < item.Quantity)
                {
                    return BadRequest("not enough of stock for product");
                }
                product.stock_quantity -= item.Quantity;
                TotalPrice += Math.Round(product.price * item.Quantity, 2);

                orderItems.Add(new OrderItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Price = Math.Round(product.price, 2),
                    OrderId = order.Id
                });
            }
            order.TotalPrice = TotalPrice;


            Database.orderitems.AddRange(orderItems);
            await Database.SaveChangesAsync();

            return Ok(new
            {
                OrderId = order.Id,
                TotalPrice = order.TotalPrice,
                OrderDate = order.OrderDate,
            });

        }
        [HttpGet]
        public async Task<IActionResult> GetData()
        {
            var data = await Database.orders.Include(a => a.OrderItems).ToListAsync();
            return Ok(data);
        }
        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetData(int id)
        {
            var data = await Database.orders.Include(x => x.OrderItems).FirstOrDefaultAsync(x => x.Id == id);
            if (data == null)
            {
                return NotFound("Order not found");
            }
            return Ok(data);
        }
        [HttpGet]
        [Route("orderItems/{id}")]
        public async Task<IActionResult> GetOrdeItemData(int id)
        {
           // var data = await Database.orders.AnyAsync(x=>x.Id==id);
           //if(data == null)
           // {
           //     return NotFound("Order item not found");
           // }
            var newdata= await Database.orderitems.Where(x=>x.OrderId==id).ToListAsync();
            if (newdata == null)
            {
                return NotFound("Order item not found");
            }
            return Ok(newdata);

        }

       
    }
}
