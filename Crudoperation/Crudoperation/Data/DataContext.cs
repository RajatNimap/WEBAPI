using Crudoperation.Model;
using Microsoft.EntityFrameworkCore;

namespace Crudoperation.Data
{
    public class DataContext:DbContext
    {
        public DataContext(DbContextOptions<DataContext> options):base(options) {
        
        
        }
       public  DbSet<Emp>emps { get; set; }
    }
}
