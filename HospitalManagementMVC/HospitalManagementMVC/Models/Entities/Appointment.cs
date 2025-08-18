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
        //public  Session Session { get; set; }   
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public AppointmentStatus StatusId { get; set; }
        public DateOnly DateofAppointment { get; set; }
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

    public enum Session {
            Morning,
            Evening
    }

}
