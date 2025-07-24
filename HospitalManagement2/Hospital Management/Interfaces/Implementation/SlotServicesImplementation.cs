using System;
using Hospital_Management.Data;
using Hospital_Management.Interfaces.Services;
using Hospital_Management.Models.Entities;
using Hospital_Management.Models.EntitiesDto;
using Microsoft.EntityFrameworkCore;
using static Hospital_Management.Interfaces.Implementation.AppointmentRepository;

namespace Hospital_Management.Interfaces.Implementation
{
    public class AppointmentRepository : IAppointmentRepo
    {

        private readonly DataContext Database;

        public AppointmentRepository(DataContext Database)
        {
            this.Database = Database;
        }

        public async Task<List<Appointment>> GetBookedSlot(int DoctorId, DateTime Date)
        {
            //var data = await Database.appointments.Where(x => x.DoctorId == DoctorId && x.DateofAppointment.ToDateTime(TimeOnly.MinValue) == Date.Date).ToListAsync();
            var data = await Database.appointments
    .Where(x => x.DoctorId == DoctorId && x.DateofAppointment == DateOnly.FromDateTime(Date))
    .ToListAsync();

            if (data == null)
            {
                return null;
            }
            return data;
        }
    }
    public class AvailabilityRepository : IAvailability
    {
        private readonly DataContext Database;

        public AvailabilityRepository(DataContext context)
        {
            Database = context;
        }

        public async Task<List<DoctorAvailability>> GetAvailability(int DoctorId, DateTime dateTime)
        {
            var dayofWeek = (int)dateTime.DayOfWeek;
            //var dayofWeek = ((int)dateTime.DayOfWeek == 0) ? 7 :(int)dateTime.DayOfWeek;
            var isOnLeave = await Database.docotorLeaves
            .AnyAsync(x => x.DoctorsId == DoctorId && x.Leave == DateOnly.FromDateTime(dateTime));


            var Data = await Database.doctorAvailabilities.Where(x => x.DoctorId == DoctorId && x.DayOfWeek == dayofWeek && x.IsAvailable && !isOnLeave).ToListAsync();
            if (Data == null)
            {
                return null;
            }

            return Data;
        }
       
         
    }

    public class SlotGenerator : ISLotGenerator
    {
        public Dictionary<string, List<TimeSlotDto>> GenerateSlotes(List<DoctorAvailability> availabilities, List<Appointment> bookings)
        {
            var groupedSlots = new Dictionary<string, List<TimeSlotDto>>();

            foreach (var availability in availabilities)
            {
                // MORNING SLOTS
                var current = availability.MorningStartTime;
                while (current < availability.MorningEndTime)
                {
                    var slotEnd = current.Add(TimeSpan.FromMinutes(30));
                    var isBooked = bookings.Any(b => b.StartTime == current && b.EndTime == slotEnd);

                    if (!groupedSlots.ContainsKey("Morning"))
                        groupedSlots["Morning"] = new List<TimeSlotDto>();

                    groupedSlots["Morning"].Add(new TimeSlotDto
                    {
                        StartTime = current.ToString(@"hh\:mm"),
                        EndTime = slotEnd.ToString(@"hh\:mm"),
                        Status = isBooked ? "booked" : "available"
                    });

                    current = slotEnd;
                }

                // EVENING SLOTS
                current = availability.EveningStartTime;
                while (current < availability.EveningEndTime)
                {
                    var slotEnd = current.Add(TimeSpan.FromMinutes(30));
                    var isBooked = bookings.Any(b => b.StartTime == current && b.EndTime == slotEnd);

                    if (!groupedSlots.ContainsKey("Evening"))
                        groupedSlots["Evening"] = new List<TimeSlotDto>();

                    groupedSlots["Evening"].Add(new TimeSlotDto
                    {
                        StartTime = current.ToString(@"hh\:mm"),
                        EndTime = slotEnd.ToString(@"hh\:mm"),
                        Status = isBooked ? "booked" : "available"
                    });

                    current = slotEnd;
                }
            }

            return groupedSlots;
        }
    }
}

