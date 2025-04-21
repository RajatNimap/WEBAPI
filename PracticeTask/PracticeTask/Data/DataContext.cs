using Microsoft.EntityFrameworkCore;
using PracticeTask.Model.Entities;

namespace PracticeTask.Data
{
    public class DataContext:DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { 
        
        
        }
        public DbSet<UserDetail>userDetails { get; set; }
    }
}
