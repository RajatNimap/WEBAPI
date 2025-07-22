using System.ComponentModel.DataAnnotations.Schema;

namespace Hospital_Management.Models.Entities
{
    public class DocotorLeave
    {

        public int Id { get; set; } 
        public int DoctorsId { get; set; }
        [ForeignKey("DoctorsId")]
        public Doctors Doctors { get; set; }
        public DateOnly Leave { get; set; }
        public string Reason { get; set; }
    }
}
