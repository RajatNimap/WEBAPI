using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PracticeTask.Data;
using PracticeTask.Model;
using PracticeTask.Model.Entities;

namespace PracticeTask.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderItemsController : ControllerBase
    {
        private readonly DataContext Database;

        public OrderItemsController(DataContext database) {

            Database = database;
        }

        [HttpPost("OrderPlaced")]

        public async Task<IActionResult> OrderPlacing(OrderRequestDto OrderReq)
        {
            if (OrderReq == null)
            {
                return BadRequest("please enter valid request");
            }

            var data =await Database.userDetails.FirstOrDefaultAsync(x => x.Id == OrderReq.UserId);
            if (data == null) {
                return BadRequest("User not found for this order");
            }
            var Orderdata = new Orders
            {
                UserDetailId = OrderReq.UserId,   
                ordertime=DateTime.UtcNow,
                Address =OrderReq.Address,
            };

            //Database.orders.Add(Orderdata); 

            decimal TotalPrice = 0;
           
            var Orderitemslist = new List<OrderItems>();
            Database.orders.Add(Orderdata);
            Database.SaveChanges();
            foreach (var item in OrderReq.orders) {

                var productdata = await Database.products.FirstOrDefaultAsync(x => x.Id == item.ProductId && x.SoftDelete == false);
                if (productdata == null) { 
                        return NotFound();  
                }
                if(item.ProductQunatity < 1)
                {
                    return BadRequest("Quantity at least 1");
                }

                if(item.ProductQunatity > productdata.StockQuatity)
                {
                    return BadRequest("Not enough Qunatity");
                }

                productdata.StockQuatity = productdata.StockQuatity-item.ProductQunatity;
                TotalPrice += Math.Round(productdata.Price * item.ProductQunatity, 2);


                Orderitemslist.Add( new OrderItems
                {
                        Price=productdata.Price,    
                        Quantity=item.ProductQunatity,
                        Productid= item.ProductId,  
                        Ordersid=Orderdata.Id,
                });
            }
            Orderdata.TotalPrice = TotalPrice;

           

            Database.orderItems.AddRange(Orderitemslist);
            Database.SaveChanges();
            

            return Ok(new
            {
                OrderId=Orderdata.Id,
                TotalPrice =Orderdata.TotalPrice,  
                data =Orderdata.ordertime
            });    
           
            
            
        }
    }
}
