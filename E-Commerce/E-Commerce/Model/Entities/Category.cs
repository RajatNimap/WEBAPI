namespace E_Commerce.Model.Entities
{
    public class Category
    {
        public int Id { get; set; } 
        public required string Name { get; set; }
        public ICollection<SubCategory> SubCategories { get; set; }=new List<SubCategory>();    
    }

}
