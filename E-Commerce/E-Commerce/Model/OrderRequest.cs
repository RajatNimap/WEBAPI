namespace E_Commerce.Model
{
    public class OrderRequest
    {
        public int UserId { get; set; }
        public string Address {  get; set; }    
        public  List<OrderItemRequest> Items { get; set; }
    }
    public class OrderItemRequest
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
