
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer;
using MvcLearning.Model.Entities;

namespace MvcLearning.Data
{
    public class DataContext:DbContext
    {
           public DataContext(DbContextOptions<DataContext> options) : base(options) { 
        
           }

        public DbSet<TodoModel>todoModels { get; set; }
    }
}
