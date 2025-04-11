namespace WebApimanyrelation.Model.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public  Address Address {get; set;}
        public int AddressId { get; set;}    
        public List<Product> Products { get; set; }
        public List<Coupon> Coupons { get; set; }

    }
}
