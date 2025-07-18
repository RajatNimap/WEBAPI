using Hospital_Management.Data;
using Hospital_Management.Interfaces.Services;
using Hospital_Management.Models.Entities;
using Hospital_Management.Models.EntitiesDto;
using Microsoft.EntityFrameworkCore;

namespace Hospital_Management.Interfaces.Implementation
{
    public class MedicalRecordImplementation : IMedicalRecord
    {
        public readonly DataContext Database;
        public MedicalRecordImplementation(DataContext data)
        {
            Database = data;
        }
        public async Task<MedicalRecord> AddMedicalRecord(MedicalRecoredDto record)
        {
            var data = new MedicalRecord
            {
                PatientsModelID = record.PatientsModelID,
                AppointmentId = record.AppointmentId,
                Prescription = record.Prescription,
                Follow_up = record.Follow_up,
                Vist_Notes = record.Vist_Notes
            };
            Database.Add(data);
            Database.SaveChanges(); 
            return data;
             
        }

        public async Task<MedicalRecord> GeMedicalRecordbyId(int Id)
        {
            var data= await Database.medicalRecords.FirstOrDefaultAsync(x => x.Id == Id);
            return data;

        }

        public async Task<List<MedicalRecord>> GetAllMedicalRecord()
        {
            var data = await Database.medicalRecords.ToListAsync();
            return data;
        }

     
        public async Task<MedicalRecord> GetMedicalRecordbyPatientId(int PatientId)
        {
            var data = await Database.medicalRecords.FirstOrDefaultAsync(x=>x.PatientsModelID == PatientId);

            return data;
            
        }
    }
}
