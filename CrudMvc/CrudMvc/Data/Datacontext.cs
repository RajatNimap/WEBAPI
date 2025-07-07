using CrudMvc.Models;
using Microsoft.EntityFrameworkCore;

namespace CrudMvc.DataContext
{
    public class Datacontext:DbContext
    {
        public Datacontext(DbContextOptions<Datacontext>options):base(options) { 
        
        }
       public DbSet<Employee> employees { get; set; }
    }
}
