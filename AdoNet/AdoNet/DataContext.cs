using AdoNet.Model;
using Microsoft.EntityFrameworkCore;


namespace AdoNet
{
    public class DataContext:DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }
        public DbSet<Employee> employees { get; set; }
    }
}
