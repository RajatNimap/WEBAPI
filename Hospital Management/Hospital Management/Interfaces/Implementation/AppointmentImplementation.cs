using Hospital_Management.Data;
using Hospital_Management.Interfaces.Services;
using Hospital_Management.Migrations;
using Hospital_Management.Models.Entities;
using Hospital_Management.Models.EntitiesDto;

namespace Hospital_Management.Interfaces.Implementation
{
    public class AppointmentImplementation :IAppointmentBooking
    {
        private readonly DataContext Database;

        public AppointmentImplementation(DataContext Database)
        {
            this.Database = Database;

        }

        public async Task<Appointment> AppointmentBooking(AppointmentDto availability)
        {
            //var CheckData = Database.appointments.Any(x => x.StartTime == availability.StartTime && x.Created.Date < availability.Created.Date);


            
                var EndTime = availability.StartTime + TimeSpan.FromMinutes(30);
                var Data = new Appointment
                {
                    DoctorId = availability.DoctorId,
                    PatientId = availability.PatientId,
                    StartTime = availability.StartTime,
                    EndTime = EndTime,
                    StatusId = availability.Status,
                    DateofAppointment = availability.DateofAppointment,
                    Created = availability.Created,
                };

                await Database.appointments.AddAsync(Data);
                await Database.SaveChangesAsync();
            


            return new Appointment
            {
                DoctorId = availability.DoctorId,
                PatientId = availability.PatientId,
                StartTime = availability.StartTime,
                EndTime = EndTime,
                StatusId = availability.Status,
                Created = availability.Created,
            };

        }


    }
}

