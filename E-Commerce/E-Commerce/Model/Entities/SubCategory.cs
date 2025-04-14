namespace E_Commerce.Model.Entities
{
    public class SubCategory
    {
        public int Id { get; set; } 
        public required string Name { get; set; }    
        //Foreign Key
        public int CategoryId {  get; set; }    
        public Category Category { get; set; }  
        public ICollection<Product> products { get; set; } = new List<Product>();
    }
}
