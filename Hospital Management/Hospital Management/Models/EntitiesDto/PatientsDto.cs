using System.ComponentModel.DataAnnotations;

namespace Hospital_Management.Models.Entities
{
    public class PatientsDto
    {
        public string Name { get; set; }
        [Required]
        public int Age { get; set; }
        [Required]
        public string Gender { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        public string Disease { get; set; }
    }
}
