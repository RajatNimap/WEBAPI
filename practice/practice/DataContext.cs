using Microsoft.EntityFrameworkCore;

namespace practice
{
    public class DataContext:DbContext
    {
        public DataContext(DbContextOptions<DataContext> options):base(options) {
        
                
        
        }

        public DbSet<emp> employee;
      
    }
}
