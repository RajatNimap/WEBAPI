using Hospital_Management.Data;
using Hospital_Management.Interfaces.Services;
using Hospital_Management.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Hospital_Management.Interfaces.Implementation
{
    public class PatientsImplementation : IPatients
    {
        private readonly DataContext Database;
        public PatientsImplementation(DataContext database) {
        
                this.Database = database;
        } 
        public async Task<List<PatientsModel>> GetPatientsModelsDetails()
        {
            var PatientDetail = await Database.patients.ToListAsync();

            if (PatientDetail == null) {
                return null;
            }

            return PatientDetail;
        }
       
        public async Task<PatientsModel> GetPatientsById(int id)
        {
            var PatientDetailById = await Database.patients.FirstOrDefaultAsync(x => x.Id == id);
            if (PatientDetailById == null) { }

            if (PatientDetailById == null) { return null; }

            return PatientDetailById;   
        }

        public async Task<PatientsModel> InsertPatientDetail(PatientsDto patients)
        {
                var Data=new PatientsModel
                {
                        Name = patients.Name,   
                        Age = patients.Age,
                        Gender = patients.Gender,   
                        Address = patients.Address,
                        Disease = patients.Disease,
                };   
            if(Data==null) { return null; }
            await Database.AddAsync(Data);  
            await Database.SaveChangesAsync();
            return Data;
        }

        public async Task<PatientsModel> UpdatePatientDetail(int id, PatientsDto patients)
        {
            var Data=await Database.patients.FirstOrDefaultAsync(x=>x.Id == id);

            if (Data == null)
            {
                return null;
            }
            Data.Name = patients.Name;  
            Data.Age = patients.Age;
            Data.Gender = patients.Gender;  
            Data.Address = patients.Address;    
            Data.Disease = patients.Disease;

            await Database.SaveChangesAsync();

            return Data;


        }
        public async Task<bool> DeletePatientDetail(int id)
        {
            var Data = await Database.patients.FirstOrDefaultAsync(x => x.Id == id);
            if (Data == null) {
                return false;
            }
             Database.patients.Remove(Data);
            await Database.SaveChangesAsync();
            return true;
        }
    }
}
