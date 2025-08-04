namespace Repositorypattern.Model
{
    public class Product
    {

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; } // Navigation property to Category   
    }

    public class Category
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public ICollection<Product> Products { get; set; } = new List<Product>();
        }

    public class Categorydto
    {
        public string Name { get; set; }
        //public ICollection<Product> Products { get; set; } = new List<Product>();
    }


}