using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hospital_Management.Models.Entities
{
    public class MedicalRecord
    {
        [Key]
        public int Id { get; set; } 
      
        public int AppointmentId { get; set; }

        [ForeignKey("AppointmentId")]
        public Appointment Appointment { get; set; }    
        public string Vist_Notes { get; set; }
        public string Prescription { get; set; }
        public string Follow_up { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;  
    }
}
