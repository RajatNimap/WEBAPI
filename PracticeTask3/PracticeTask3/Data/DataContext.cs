using Microsoft.EntityFrameworkCore;
using PracticeTask3.Model.Entities;

namespace PracticeTask3.Data
{
    public class DataContext:DbContext
    {

        public DataContext(DbContextOptions<DataContext> options) : base(options) { 
        
        }

        public DbSet<Student> students { get; set; }
    }
}
