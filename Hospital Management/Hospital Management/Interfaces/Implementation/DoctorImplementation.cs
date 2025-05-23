using Hospital_Management.Data;
using Hospital_Management.Interfaces.Services;
using Hospital_Management.Models.Entities;
using Hospital_Management.Models.EntitiesDto;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;

namespace Hospital_Management.Interfaces.Implementation
{
    public class DoctorImplementation : IDoctors
    {
        private readonly DataContext Database;
        public DoctorImplementation(DataContext Database) {
            this.Database = Database;
        }
        public async Task<List<Doctors>> GetDoctors()
        {
            var Data = await Database.doctors.ToListAsync();
            if (Data == null)
            {
                return null;
            }

            return Data;    
        }

        public async Task<Doctors> GetDoctorssById(int id)
        {
            var Data = await Database.doctors.FirstOrDefaultAsync(x => x.Id == id);
            if (Data == null) {
                return null;
            }
            return Data;
        }

        public async Task<Doctors> InsertDoctorsDetail(DoctorDto Doctors)
        {
            var Data = new Doctors
            {
                Name = Doctors.Name,
                Email = Doctors.Email,
                Phoneno = Doctors.Phoneno,
                Specialization = Doctors.Specialization,
                IsCreated = DateTime.UtcNow
            };

            await Database.doctors.AddAsync(Data);
            await Database.SaveChangesAsync();
            return Data;
        }

        public async Task<Doctors> UpdateDoctorsDetail(int id, DoctorDto Doctorss)
        {
            var Data = await Database.doctors.FirstOrDefaultAsync(x=>x.Id == id);
            if (Data == null) {
                return null;
            }
            Data.Name = Doctorss.Name;  
            Data.Email = Doctorss.Email;
            Data.Phoneno = Doctorss.Phoneno;
            Data.Specialization = Doctorss.Specialization;

            return Data;
        }
        public Task<bool> DeleteDoctorsDetail(int id)
        {
            throw new NotImplementedException();
        }
    }
}
