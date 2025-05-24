using Hospital_Management.Interfaces.Services;
using Hospital_Management.Models.EntitiesDto;

namespace Hospital_Management.Interfaces.Implementation
{
    public class DoctorSlotService
    {
        private readonly SlotGenerator _slotGenerator;
        private readonly AvailabilityRepository _availabilityRepository;
        private readonly AppointmentRepository _appointmentRepository;

        public DoctorSlotService(
            SlotGenerator slotGenerator,
            AvailabilityRepository availabilityRepository,
            AppointmentRepository appointmentRepository)
        {
            _slotGenerator = slotGenerator;
            _availabilityRepository = availabilityRepository;
            _appointmentRepository = appointmentRepository;
        }

        public async Task<Dictionary<string, List<TimeSlotDto>>> GetDoctorSlotsAsync(int doctorId, DateTime date)
        {
            var availabilities = await _availabilityRepository.GetAvailability(doctorId, date);
            var bookings = await _appointmentRepository.GetBookedSlot(doctorId, date);
            return _slotGenerator.GenerateSlotes(availabilities, bookings);
        }
    }
}
