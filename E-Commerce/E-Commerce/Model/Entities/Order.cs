using System.Text.Json.Serialization;

namespace E_Commerce.Model.Entities
{
    public class Order
    {
        public int Id { get; set; } 
        public double TotalPrice { get; set; }
        public string Address { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        //Foreign key
        public int UserId { get; set; }
        [JsonIgnore]
        public User User { get; set; }
        //Foreign key for user
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    }
}
