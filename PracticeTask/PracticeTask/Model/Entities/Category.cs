namespace PracticeTask.Model.Entities
{
    public class Category
    {

        public int Id { get; set; } 
        public string Name { get; set; }    
       // One to many relationship
       public virtual List <Product> Products { get; set; } =new List<Product>();
    }
}
