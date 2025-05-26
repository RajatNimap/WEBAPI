using Hospital_Management.Models.Entities;

namespace Hospital_Management.Models.EntitiesDto
{
    public class AppointmentDto
    {
        public int DoctorId { get; set; }
        public int PatientId { get; set; }
        public Session Session { get; set; }
        public TimeSpan StartTime { get; set; }
        //public TimeSpan EndTime { get; set; } 
        public DateTime DateofAppointment { get; set;}
        public AppointmentStatus Status { get; set; }
        public DateTime Created { get; set; }= DateTime.Now;    
    }
}
