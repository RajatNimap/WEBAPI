namespace Hospital_Management.Models.EntitiesDto
{
    public class TimeSlotDto
    {
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string Status { get; set; } // "available" or "booked"
    }
}
