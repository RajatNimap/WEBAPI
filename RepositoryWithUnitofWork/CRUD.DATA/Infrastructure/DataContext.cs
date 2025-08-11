using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRUD.MODEL.Entities;
using Microsoft.EntityFrameworkCore;

namespace CRUD.DATA.Infrastructure
{
    public class DataContext : DbContext
    {

        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }
        public DbSet<Employee> employees { get; set; }
      

    }
}
