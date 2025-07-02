using Microsoft.EntityFrameworkCore;

namespace Test2
{
    public class DataContext:DbContext
    {

        public DataContext(DbContextOptions<DataContext> options) : base(options) {
        
        }   

        public DbSet<Customer> customers { get; set; }
        public DbSet<Category>categories { get; set; }
        public DbSet<Product> products { get; set; }       
        public DbSet<Orders> orders { get; set; }       
    }
}
