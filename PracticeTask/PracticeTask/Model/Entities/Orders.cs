using Microsoft.EntityFrameworkCore;

namespace PracticeTask.Model.Entities
{
    public class Orders
    {
        public int Id { get; set; }
        [Precision(8,2)]
        public decimal TotalPrice { get; set; }
        public string Address { get; set; } 
        public DateTime ordertime { get; set; }=DateTime.UtcNow;

        public ICollection <OrderItems> orderItems { get; set; } = new List<OrderItems>();

        // Forign key

        public int UserDetailId { get; set; }
        public UserDetail? UserDetail { get; set; }  
    }
}
