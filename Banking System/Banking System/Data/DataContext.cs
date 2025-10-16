using Microsoft.EntityFrameworkCore;

namespace Banking_System.Data
{
    public class DataContext:DbContext
    {
        public DataContext(DbContextOptions<DataContext> options):base(options)
        {
            
        }
    }
}
