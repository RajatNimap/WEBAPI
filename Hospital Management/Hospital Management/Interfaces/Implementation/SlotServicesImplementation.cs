using System;
using Hospital_Management.Data;
using Hospital_Management.Interfaces.Services;
using Hospital_Management.Models.Entities;
using Hospital_Management.Models.EntitiesDto;
using Microsoft.EntityFrameworkCore;
using static Hospital_Management.Interfaces.Implementation.AppointmentRepository;

namespace Hospital_Management.Interfaces.Implementation
{

    public class DefaulSessionClassfier : Classifier
    {
        public string Classify(TimeSpan startTime)
        {
            if (startTime >= TimeSpan.FromHours(8) && startTime < TimeSpan.FromHours(12))
                return "morning";

            return (startTime >= TimeSpan.FromHours(17) && startTime < TimeSpan.FromHours(21)) ? "evening" : "";


        }

    }

    public class AppointmentRepository : IAppointmentRepo
    {

        private readonly DataContext Database;

        public AppointmentRepository(DataContext Database)
        {
            this.Database = Database;
        }

        public async Task<List<Appointment>> GetBookedSlot(int DoctorId, DateTime Date)
        {
            var data = await Database.appointments.Where(x => x.DoctorId == DoctorId && x.Created == Date.Date).ToListAsync();

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
                    var dayofWeek=(int)dateTime.DayOfWeek;
                var Data = await Database.doctorAvailabilities.Where(x => x.DoctorId == DoctorId && x.DayOfWeek == dayofWeek && x.IsAvailable).ToListAsync();
                if (Data == null) {
                    return null;
                }
                return Data;
            }

           
        }

        public class SlotGenerator : ISLotGenerator
        {
            private readonly DefaulSessionClassfier _classifier;

            public SlotGenerator(DefaulSessionClassfier classifier)
            {
                _classifier = classifier;
            }

            public Dictionary<string, List<TimeSlotDto>> GenerateSlotes(List<DoctorAvailability> availabilities, List<Appointment> bookings)
            {
                var groupedSlots = new Dictionary<string, List<TimeSlotDto>>();

                foreach (var availability in availabilities)
                {
                    var current = availability.StartTime;
                    while (current < availability.EndTime)
                    {
                        var slotEnd = current.Add(TimeSpan.FromMinutes(30));
                        var isBooked = bookings.Any(b => b.StartTime == current && b.EndTime == slotEnd);

                        var session = _classifier.Classify(current);
                        if (!groupedSlots.ContainsKey(session))
                            groupedSlots[session] = new List<TimeSlotDto>();

                        groupedSlots[session].Add(new TimeSlotDto
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
