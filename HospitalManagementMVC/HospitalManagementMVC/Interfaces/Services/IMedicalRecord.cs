using Hospital_Management.Models.EntitiesDto;

using Hospital_Management.Models.Entities;

namespace Hospital_Management.Interfaces.Services
{
    public interface IMedicalRecord
    {
        Task<List<MedicalRecord>> GetAllMedicalRecord();
        Task<MedicalRecord> GeMedicalRecordbyId(int Id); 
        //Task<MedicalRecord> GetMedicalRecordbyPatientId(int PatientId); 
        Task<MedicalRecord> AddMedicalRecord(MedicalRecoredDto record);  
    }
}
