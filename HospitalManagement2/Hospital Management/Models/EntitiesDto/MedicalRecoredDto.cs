using Hospital_Management.Models.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Hospital_Management.Models.EntitiesDto
{
    public class MedicalRecoredDto
    {


 
        public int AppointmentId { get; set; }
        public string ?Vist_Notes { get; set; }
        public string ?Prescription { get; set; }
        public string ?Follow_up { get; set; }
    
    }
}
