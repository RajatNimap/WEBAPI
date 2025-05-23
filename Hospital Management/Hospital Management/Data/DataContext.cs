using Hospital_Management.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace Hospital_Management.Data
{
    public class DataContext:DbContext
    {
        public DataContext(DbContextOptions<DataContext>options):base(options)
        {

        }
        public DbSet<Register> registers { get; set; }
        public DbSet<RefreshToken>refreshTokens { get; set; }   
        public DbSet<PatientsModel> patients { get; set; }  
        public DbSet<Department> department { get; set; }   
        public DbSet<Doctors> doctors { get; set; }


    }
}
