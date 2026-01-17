using Microsoft.EntityFrameworkCore;
using Practice2.Models;

namespace Practice2.Data
{
    public class DataContext :DbContext
    {
        public DataContext(DbContextOptions<DataContext> options):base (options)
        {
            
        }
        public DbSet<EmployeeModel> employee { get; set; }
        public DbSet<DepartmentModel> department { get; set; }

        public DbSet<UserModel> userModels { get; set; }
    }
}
