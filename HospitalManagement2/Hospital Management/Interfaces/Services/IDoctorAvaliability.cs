using Hospital_Management.Models.Entities;
using Hospital_Management.Models.EntitiesDto;

namespace Hospital_Management.Interfaces.Services
{
    public interface IDoctorAvaliability
    {

        Task<IEnumerable<DoctorAvailability>> GetAllAsync();
        Task<DoctorAvailability> GetByIdAsync(int doctorId);
        Task<DoctorAvailibilityDTO> CreateAsync(DoctorAvailibilityDTO availability);
        Task<DoctorAvailibilityDTO> UpdateAsync(int doctorId, DoctorAvailibilityDTO availability);
    }
}
