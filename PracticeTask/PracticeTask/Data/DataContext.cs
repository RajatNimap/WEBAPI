using Microsoft.EntityFrameworkCore;
using PracticeTask.Model.Entities;

namespace PracticeTask.Data
{
    public class DataContext:DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { 
        
        
        }
        public DbSet<UserDetail>userDetails { get; set; }
        public DbSet<Category>categories { get; set; }  
        public DbSet<Product> products { get; set; }      
        public DbSet<OrderItems> orderItems { get; set; }   
        public DbSet<Orders> orders { get; set; }
        public DbSet<EmployeeService> employeeServices { get; set; }   
        public DbSet<RefreshToken> refreshTokens { get; set; }  

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>().HasQueryFilter(x => !x.SoftDelete);  
           
        }

    }
   
}
