using Hospital_Management.Models.Entities;
using Hospital_Management.Models.EntitiesDto;

namespace Hospital_Management.Interfaces.Services
{
    public interface IAvailability
    {
        Task <List<DoctorAvailability>> GetAvailability(int DoctorId,DateTime dateTime);

    }

    public interface IAppointmentRepo { 
    
        Task<List<Appointment>> GetBookedSlot(int DoctorId,DateTime Date);
    
    }
  
    public interface ISLotGenerator
    {
        Dictionary<String, List<TimeSlotDto>> GenerateSlotes(List<DoctorAvailability> availabilities, List<Appointment> bookings);

    }
}
