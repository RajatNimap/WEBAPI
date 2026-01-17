using Learning.Context.Entities;
using Microsoft.EntityFrameworkCore;

namespace Learning.Context
{
    public class DataContext:DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) :base(options)    
        {
            
        }
        public DbSet<User> user { get; set; }   
    }
}
