using Hospital_Management.Models.Entities;

namespace Hospital_Management.Interfaces.Services
{
    public interface IPatients
    {
        Task<List<PatientsModel>> GetPatientsModelsDetails();
        Task<PatientsModel>GetPatientsById(int id);
        Task<PatientsModel> InsertPatientDetail(PatientsDto patients);
        Task<PatientsModel> UpdatePatientDetail(int id,PatientsDto patients);
        Task<bool> DeletePatientDetail(int id);
        Task<Appointment> GetAppointmentDetail(int id);
        Task<List<PatientsModel>> Searching(string value);
        Task<List<MedicalRecord>> GettingAllmedicalRecord(int patientId);
    }
}
