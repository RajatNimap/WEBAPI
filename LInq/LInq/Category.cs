using System.ComponentModel.DataAnnotations;

namespace LInq
{
    public class Category
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public ICollection<Product> products { get; set; } = new List<Product>();

    }

}
