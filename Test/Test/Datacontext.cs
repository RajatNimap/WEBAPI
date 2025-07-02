using Microsoft.EntityFrameworkCore;
using Test.Entities;

namespace Test
{
    public class Datacontext:DbContext
    {

        public Datacontext(DbContextOptions<Datacontext> options) : base(options)
        {

        }

        public DbSet<product> Products { get; set; } 
        public DbSet<category> Categories { get;  set; }
        public DbSet<customer> customers { get; set; }

        public DbSet<Orders> Orders { get; set; }   
    }
}

