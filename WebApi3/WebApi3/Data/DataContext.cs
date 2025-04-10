using Microsoft.EntityFrameworkCore;
using WebApi3.Model.Entities;
namespace WebApi3.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options):base(options) {
        
        }
        public DbSet<Superhero> superheros { get; set; }
    }
}
