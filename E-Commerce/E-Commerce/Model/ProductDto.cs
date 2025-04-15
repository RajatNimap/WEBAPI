namespace E_Commerce.Model
{
    public class ProductDto
    {
        public required string Name { get; set; }
        public string Description { get; set; }
        public float price { get; set; }
        public int stock_quantity { get; set; }

        //foreign key
        public int CategoryID { get; set; }
    }
}
