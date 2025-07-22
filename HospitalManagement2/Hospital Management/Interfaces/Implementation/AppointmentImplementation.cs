using System;
using Hospital_Management.Data;
using Hospital_Management.Interfaces.Services;
using Hospital_Management.Models.Entities;
using Hospital_Management.Models.EntitiesDto;
using Microsoft.EntityFrameworkCore;

namespace Hospital_Management.Interfaces.Implementation
{
    public class AppointmentImplementation :IAppointmentBooking
    {
        private readonly DataContext Database;
        private readonly EmailService _emailService;

        public AppointmentImplementation( DataContext database, EmailService emailService)
        {
             Database = database;
            _emailService = emailService;   
        }

        public async Task<Appointment> AppointmentBooking(AppointmentDto availability)
        {

            if (availability.StartTime.TotalMinutes % 30 != 0 || availability.DateofAppointment < DateOnly.FromDateTime(DateTime.Now) )
            {
                return null; // Not a valid 30-minute slot
            }
            var now =DateTime.Now;
            var Timeonly = TimeOnly.FromTimeSpan(availability.StartTime);
            var appointmentDateTIme=availability.DateofAppointment.ToDateTime(Timeonly);  
            if(appointmentDateTIme < now)
            {
                return null; // Cannot book an appointment in the past
            }

            //var CheckData = Database.appointments.Any(x => x.StartTime == availability.StartTime && x.Created.Date < availability.Created.Date)
            var isOnLeave = await Database.docotorLeaves
            .AnyAsync(x => x.DoctorsId == availability.DoctorId && x.Leave == DateOnly.FromDateTime(availability.DateofAppointment.ToDateTime(TimeOnly.MinValue)));


            var Check0 = (int)availability.DateofAppointment.DayOfWeek;
            var Check1 = await Database.doctorAvailabilities.Where(x => x.DoctorId == availability.DoctorId && x.DayOfWeek == Check0 && x.IsAvailable && !isOnLeave).FirstOrDefaultAsync();

            if (Check1 == null)
            {
                return null;
            }
            var isSlotBooked = await Database.appointments.AnyAsync(x =>
      x.DoctorId == availability.DoctorId &&
      x.DateofAppointment == availability.DateofAppointment &&
      x.StartTime == availability.StartTime);

            if (isSlotBooked)
            {
                return null;
            }

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
            var doctname = await Database.doctors.FirstOrDefaultAsync(x => x.Id == availability.DoctorId);
            var patintname = await Database.patients.FirstOrDefaultAsync(x => x.Id == availability.PatientId);


            string subject = $"Appointment Confirmation on {availability.DateofAppointment}";
            await _emailService.SendEmailAsync(
      "reetaroashan@gmail.com",
      subject,
      $@"
        Mr/Ms/Mrs {patintname.Name},<br><br>
        Your appointment has been <b>confirmed</b> with Dr. {doctname.Name}.<br><br>
        <b>Date</b>: {availability.DateofAppointment}<br>
        <b>Time</b>: {availability.StartTime}<br><br>
        Please arrive 10 minutes early. If you have any questions, feel free to contact us.<br><br>
        Thank you,<br>
        <b>Hospital Management</b>
    "
  );


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
        public async Task<bool> CancelAppointment(int appointmentId)
        {
            var appointment = await Database.appointments.FindAsync(appointmentId);
            if (appointment == null || appointment.StatusId == AppointmentStatus.Cancelled)
                return false;

            appointment.StatusId = AppointmentStatus.Cancelled;
            await Database.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RescheduleAppointment(int appointmentId, AppointmentDto newData)
        {
            var appointment = await Database.appointments.FindAsync(appointmentId);
            if (appointment == null || appointment.StatusId == AppointmentStatus.Cancelled)
                return false;

            // Check if new slot is valid
            if (newData.StartTime.TotalMinutes % 30 != 0) return false;

            var isSlotBooked = await Database.appointments.AnyAsync(x =>
                x.DoctorId == newData.DoctorId &&
                x.DateofAppointment == newData.DateofAppointment &&
                x.StartTime == newData.StartTime &&
                x.Id != appointmentId);

            if (isSlotBooked) return false;

            // Update values
            appointment.StartTime = newData.StartTime;
            appointment.EndTime = newData.StartTime + TimeSpan.FromMinutes(30);
            appointment.DateofAppointment = newData.DateofAppointment;
            appointment.StatusId = AppointmentStatus.Rescheduled;
           // appointment.Session = newData.Session;

            await Database.SaveChangesAsync();
            return true;
        }

    }
}

