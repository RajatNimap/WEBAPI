using Microsoft.EntityFrameworkCore;
using WebApimanyrelation.Model.Entities;
namespace WebApimanyrelation.Data
{
    public class DataContext:DbContext
    {

        public DataContext(DbContextOptions<DataContext> options):base(options) { 
              
        }
        public DbSet<Address>addresses { get; set; }   
        public DbSet<User> users { get; set; }  
        public DbSet<Product> products { get; set; }    
        public DbSet<UserCoupon> userCoupons { get; set; }
        public DbSet<Coupon> coupons { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Seed base tables
            modelBuilder.Entity<Address>().HasData(new Address { Id = 1, streetname = "Kamatghar bhiwandi" });

            modelBuilder.Entity<User>().HasData(
                new User { Id = 1, Name = "Rajat", AddressId = 1 }
            );

            modelBuilder.Entity<Product>().HasData(
                new Product { Id = 1, Title = "apple", Price = 50, userId = 1 }
            );

            modelBuilder.Entity<Coupon>().HasData(
                new Coupon { Id = 1, code = "save20" }
            );

            // Configure the many-to-many relationship
            modelBuilder.Entity<UserCoupon>()
                .HasKey(uc => new { uc.UserId, uc.CouponId });

            modelBuilder.Entity<UserCoupon>()
                .HasOne(uc => uc.User)
                .WithMany()
                .HasForeignKey(uc => uc.UserId);

            modelBuilder.Entity<UserCoupon>()
                .HasOne(uc => uc.Coupon)
                .WithMany()
                .HasForeignKey(uc => uc.CouponId);

            modelBuilder.Entity<User>()
                .HasMany(u => u.Coupons)
                .WithMany(c => c.users)
                .UsingEntity<UserCoupon>();

            // Now seed the join table
            modelBuilder.Entity<UserCoupon>().HasData(
                new UserCoupon { UserId = 1, CouponId = 1 }
            );
        }

    }
}
