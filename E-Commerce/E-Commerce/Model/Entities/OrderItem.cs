namespace E_Commerce.Model.Entities
{
    public class OrderItem
    {
        public int Id { get; set; } 
        public int Quantity { get; set; }
        public Double TotalPrice { get; set; }

        //Foreign key of product
        public int ProductId { get; set; }  
        public Product Product { get; set; }  

        //Foreign key for user
        public int UserId {  get; set; }    
        public User User { get; set; }


    }
}
