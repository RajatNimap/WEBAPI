using System.ComponentModel.DataAnnotations;

namespace Test.Entities
{
    public class customer
    {

        [Key]
        public int Id { get; set; } 
        public string Name { get; set; }    

        public string email { get; set; }
    }
}
