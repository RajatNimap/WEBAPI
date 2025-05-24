using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hospital_Management.Models.Entities
{
    public class MedicalRecord
    {
        [Key]
        public int Id { get; set; } 
        public int PatientsModelID { get; set; }

        [ForeignKey("PatientsModelID")]
        public PatientsModel PatientsModel { get; set; } 
        public int DoctorId { get; set; }

        [ForeignKey("Doctors")]
        public Doctors Doctors { get; set; }    
        public int AppointmentId { get; set; }

        [ForeignKey("AppointmentId")]
        public Appointment Appointment { get; set; }    
        public string Vist_Notes { get; set; }
        public string Prescription { get; set; }
        public string Follow_up { get; set; }
        public DateTime CreatedAt { get; set; }  
    }
}
