using System.ComponentModel.DataAnnotations;

namespace Hospital_Management.Models.Entities
{
    public class PatientsModel
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public string Gender { get; set; }
        public string Address { get; set; }
        public string Disease { get; set; }
        public string DiseaseHistory { get; set; }
        public DateTime Createdate { get; set; }
        public ICollection<Appointment> Appointment { get; set; } = new List<Appointment>();

    }

    
}
