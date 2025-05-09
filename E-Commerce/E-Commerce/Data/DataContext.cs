using E_Commerce.Model.Entities;
using Microsoft.EntityFrameworkCore;
namespace E_Commerce.Data
{
    public class DataContext:DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { 
       
        }
        public DbSet<User>users{ get; set; }
        public DbSet<Category>categories{ get; set; }   
        public DbSet<Product> products{ get; set; } 
        public DbSet<OrderItem> orderitems{ get; set; } 
        public DbSet<Order> orders{ get; set; }
        public DbSet<RefreshToken> refreshTokens{ get; set; }   

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>().HasQueryFilter(p => p.Soft_delete == 0);
            modelBuilder.Entity<User>().HasQueryFilter(User => User.Soft_delete == 0);
        }
    }
}
