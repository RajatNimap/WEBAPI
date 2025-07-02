using System.ComponentModel.DataAnnotations;

namespace Test.Entities
{
    public class Orders
    {

        [Key]
        public int Id { get; set; } 
        public string address { get; set; }

        public int ProductId { get; set; }

        public int CustomerId { get; set; }
        public product product { get; set; }   
        public customer customer { get; set; }  

    }
}
