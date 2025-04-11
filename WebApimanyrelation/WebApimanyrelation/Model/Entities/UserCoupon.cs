namespace WebApimanyrelation.Model.Entities
{
    public class UserCoupon
    {
        public int UserId { get; set; }
        public User User { get; set; }

        public int CouponId { get; set; }
        public Coupon Coupon { get; set; }
    }

}