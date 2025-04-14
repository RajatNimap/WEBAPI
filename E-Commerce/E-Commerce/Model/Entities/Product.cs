namespace E_Commerce.Model.Entities
{
    public class Product
    {
        public int Id { get; set; } 
        public required string Name { get; set; }
        public string Description { get; set; } 
        public float price { get; set; }
        public int stock_quantity {  get; set; }    
        
       //foreign key
        public int SubCategoryID { get; set; }
        public SubCategory subCategory { get; set; }    

    }
}
