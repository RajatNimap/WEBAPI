using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hospital_Management.Models.Entities
{
    public class Appointment
    {

        [Key]
        public int Id { get; set; }
        public int DoctorId { get; set; }
        public int PatientId { get; set; }
        public int DayOfWeek { get; set; } // 0 = Sunday ... 6 = Saturday
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public bool IsAvailable { get; set; }
        public AppointmentStatus Status { get; set; }
        public DateTime Created { get; set; }
        [ForeignKey("DoctorId")]
        public Doctors Doctors { get; set; }

        [ForeignKey("PatientId")]
        public PatientsModel Patients { get; set; } 

    }

    public enum AppointmentStatus
    {
        Scheduled,
        Completed,
        Cancelled,
        Rescheduled
    }
}
