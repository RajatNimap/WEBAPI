using System.ComponentModel.DataAnnotations.Schema;

namespace Hospital_Management.Models.Entities
{
    public class DoctorAvailability
    {
        public int Id { get; set; }
        public int DoctorId { get; set; }
        public int DayOfWeek { get; set; } // 0 = Sunday ... 6 = Saturday
        public TimeSpan MorningStartTime { get; set; }
        public TimeSpan MorningEndTime { get; set; }

        public TimeSpan EveningStartTime { get; set; }
        public TimeSpan EveningEndTime { get; set; }
        public bool IsAvailable { get; set; }

        [ForeignKey("DoctorId")]
        public Doctors Doctors { get; set; }
    }
}
