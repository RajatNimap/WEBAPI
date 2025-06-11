using System.Text.Json.Serialization;

namespace E_Commerce.Model.Entities
{
    public class OrderItem
    {
        public int Id { get; set; } 
        public int Quantity { get; set; }
        public Double Price { get; set; }

        //Foreign key of product
        public int ProductId { get; set; }
        [JsonIgnore]
        public Product? Product { get; set; }  
        
        //Foreign key for  order
        public int OrderId { get; set; }
        [JsonIgnore]
        public Order Order { get; set; }

    }
}
