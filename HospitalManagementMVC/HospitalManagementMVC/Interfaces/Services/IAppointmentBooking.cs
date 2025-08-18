using Hospital_Management.Models.Entities;
using Hospital_Management.Models.EntitiesDto;

namespace Hospital_Management.Interfaces.Services
{
    public interface IAppointmentBooking
    {
        Task<Appointment> AppointmentBooking(AppointmentDto availability);
        Task<bool> RescheduleAppointment(int appointmentId, AppointmentDto newData);
        Task<bool> CancelAppointment(int appointmentId);

    }
}
