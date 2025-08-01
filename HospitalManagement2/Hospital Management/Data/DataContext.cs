﻿using Hospital_Management.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace Hospital_Management.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }
        public DbSet<Register> registers { get; set; }
        public DbSet<RefreshToken> refreshTokens { get; set; }
        public DbSet<PatientsModel> patients { get; set; }
        public DbSet<Department> department { get; set; }
        public DbSet<Doctors> doctors { get; set; }
        public DbSet<Appointment> appointments { get; set; }
        //  public DbSet<MedicalRecord> medicalRecords { get; set; }    
        public DbSet<DoctorAvailability> doctorAvailabilities { get; set; }
        public DbSet<DocotorLeave> docotorLeaves { get; set; }
        public DbSet<MedicalRecord> medicalRecords { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<MedicalRecord>()
                .Ignore("PatientModelId");  
        }
    }
}