namespace Hospital_Management.Models.EntitiesDto
{
    public class DoctorAvailibilityDTO
    {
        public int DoctorId { get; set; }
        public int DayOfWeek { get; set; } // 0 = Sunday ... 6 = Saturday
        public TimeSpan MorningStartTime { get; set; }
        public TimeSpan MorningEndTime { get; set; }

        public TimeSpan EveningStartTime { get; set; }
        public TimeSpan EveningEndTime { get; set; }
        public bool IsAvailable { get; set; }
    }
}
