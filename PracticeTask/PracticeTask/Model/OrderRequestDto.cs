namespace PracticeTask.Model
{
    public class OrderRequestDto
    {
        public int UserId { get; set; }
        public string Address { get; set; }
        public ICollection<ProductOrder> orders { get; set; }=new List<ProductOrder>();
    }

    public class ProductOrder()
    {
            public int ProductId {  get; set; } 
            public int ProductQunatity {  get; set; }

    }
}
