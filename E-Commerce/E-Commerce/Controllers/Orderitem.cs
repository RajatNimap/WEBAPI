//using System.Diagnostics.Contracts;
//using E_Commerce.Data;
//using E_Commerce.Model;
//using E_Commerce.Model.Entities;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;

//namespace E_Commerce.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]

//    public class Orderitem : ControllerBase
//    {
//        private readonly DataContext Database;

//        public Orderitem(DataContext database)
//        {
//            Database = database;
//        }
//        [HttpGet]
//        public async Task<IActionResult> GetData()
//        {
//            var data = await Database.orderitems.Select(x => new OrderitemDto
//            {
//                Id = x.Id,
//                Quantity = x.Quantity,
//                TotalPrice = x.TotalPrice,
//                ProductId = x.ProductId,
//                OrderId= x.OrderId,
//            }).ToListAsync();
//            if (data == null) { 
//                    return NotFound();  
//            }

//            return Ok(data);
//        }
//        [HttpGet]
//        [Route("{id}")]
//        public async Task<IActionResult> GetDataparticular(int id)
//        {
//            var data = await Database.orderitems.FindAsync(id);

//            if (data == null) {
//                return NotFound();
//            }
//            var Itemdata = new OrderitemDto
//            {
//                Id = data.Id,
//                Quantity = data.Quantity,
//                TotalPrice = data.TotalPrice,
//                ProductId = data.ProductId,
//                OrderId = data.OrderId,

//            };
           
//            return Ok(Itemdata);
//        }
//        [HttpPost]
//        public async Task<IActionResult> PostData([FromBody]OrderitemDto item)
//        {
//            var data = new OrderItem
//            {
//                Quantity = item.Quantity,
//                TotalPrice = item.TotalPrice,
//                ProductId = item.ProductId,
//                OrderId = item.OrderId,
//            };
//           await Database.orderitems.AddAsync(data);
//             Database.SaveChanges();
//            return Ok("SuccessFully data added");
//        }
//        [HttpPut]
//        [Route("{id}")]
//        public async Task<IActionResult> UpdateData(int id,OrderitemDto orderitemDto)
//        {
//            var data= await Database.orderitems.FindAsync(id);
//            data.Quantity = orderitemDto.Quantity;  
//            data.TotalPrice = orderitemDto.TotalPrice;
//            data.ProductId = orderitemDto.ProductId;
//            data.OrderId = orderitemDto.OrderId;
//            await Database.SaveChangesAsync();
//            return Ok(data);
//        }
//        [HttpDelete]
//        public async Task<IActionResult> DeleteData(int id)
//        {
//            var data = await Database.orderitems.FindAsync(id);

//            if (data == null)
//            {
//                return NotFound();  
//            }
//            Database.orderitems.Remove(data);
//            return Ok("data is deleted");
//        }        
//    }
//}
