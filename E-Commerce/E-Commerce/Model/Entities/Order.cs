namespace E_Commerce.Model.Entities
{
    public class Order
    {
        public int Id { get; set; } 
        public double TotalPrice { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
            
        //Foreign key
        public int UserId { get; set; }
        public User User { get; set; }

        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    }
}
