using System.ComponentModel.DataAnnotations;

namespace E_Commerce.Model.Entities
{
    public class Category
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public ICollection<Product> products { get; set; } = new List<Product>();

    }

}
