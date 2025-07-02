using System.ComponentModel.DataAnnotations;

namespace Test2
{
    public class Orders
    {
        [Key]
        public int Id { get; set; }
        public string Address { get; set; } 
        public int ProductId { get; set; }      
        public Product Product { get; set; }
    

    }
}
