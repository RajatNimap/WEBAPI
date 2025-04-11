namespace WebApimanyrelation.Model.Entities
{
    public class Coupon
    {
        public int Id { get; set; } 
        public string code { get; set; }
        public  List<User> users { get; set; }  
    }
}
