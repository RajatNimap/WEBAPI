using Microsoft.EntityFrameworkCore;
using Practice2.Model.Entities;

namespace Practice2.Data
{
    public class DataContext:DbContext
    {
        public DataContext(DbContextOptions<DataContext> options):base(options)
        {

        }
        public DbSet<EmployeeModel>employeeModels { get; set; }
    }
}
