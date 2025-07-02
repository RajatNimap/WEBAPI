using System.ComponentModel.DataAnnotations;

namespace Test.Entities
{
    public class category
    {
        [Key]
        public int Id { get; set; } 
        public string Name { get; set; }
    }
}
