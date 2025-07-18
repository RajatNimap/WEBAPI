using Hospital_Management.Models.Entities;
using Hospital_Management.Models.EntitiesDto;

namespace Hospital_Management.Interfaces.Services
{
    public interface IDoctors
    {
        Task<List<Doctors>> GetDoctors();
        Task<Doctors> GetDoctorssById(int id);
        Task<Doctors> InsertDoctorsDetail(DoctorDto Doctors);
        Task<Doctors> UpdateDoctorsDetail(int id, DoctorDto Doctorss);
        Task<bool> DeleteDoctorsDetail(int id);
        Task<DocotorLeave> MarkdoctorLeave(DoctorLeaveDto doctorleave);
    }
}
