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

            var user =await Database.users.FindAsync(orderRequest.UserId);
            if (user == null) {

                return BadRequest("User Not Found");
            }

            var order = new Order
            {
                UserId=orderRequest.UserId, 
                OrderDate = DateTime.Now,
            };
            Database.orders.Add(order);
            await Database.SaveChangesAsync();
            var orderItems=new List<OrderItem>();
            double TotalPrice = 0;
            foreach (var item in orderRequest.Items) { 
                var product =await Database.products.FindAsync(item.ProductId);
                if (product == null) {
                    return BadRequest("Product Not found");
                }
                if(product.stock_quantity < item.Quantity)
                {
                    return BadRequest("not enough of stock for product");
                }
                product.stock_quantity -= item.Quantity;
                TotalPrice += product.price * item.Quantity;

                orderItems.Add(new OrderItem
                {
                    ProductId=item.ProductId,
                    Quantity=item.Quantity,
                    Price=product.price,
                    OrderId = order.Id
                });
            }
            order.TotalPrice = TotalPrice;
           

            Database.orderitems.AddRange(orderItems);
            await Database.SaveChangesAsync();

            return Ok(new 
            {
                OrderId = order.Id,
                TotalPrice=order.TotalPrice,
                OrderDate=order.OrderDate,

            });

        }
        [HttpGet]
        public async Task<IActionResult> GetData()
        {
            var data = Database.orders.Include(a=>a.OrderItems).ToList();
            return Ok(data);
        }
    }
}
