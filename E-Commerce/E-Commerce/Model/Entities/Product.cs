namespace E_Commerce.Model.Entities
{
    public class Product
    {
        public int Id { get; set; } 
        public required string Name { get; set; }
        public string Description { get; set; } 
        public float price { get; set; }
        public int stock_quantity {  get; set; }
        public int Soft_delete { get; set; } = 0;  
        //foreign key
        public int CategoryID { get; set; }
        public Category? Category { get; set; }    

    }
}
