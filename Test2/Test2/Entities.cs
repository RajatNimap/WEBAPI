using System.Data.Common;

namespace Test2
{
    public class Customer{
            
        public int Id { get; set; } 
        public string Name { get; set; }    
        public string Email { get; set; } 


    }

    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Product> products { get; set; }

    }
    public class Product
    {
        public int Id { get; set; } 
        public string Name { get; set; }    
        public double Price { get; set; }   
        public int CategoryId { get; set; }
        public Category Category { get; set; }  
    }
}
