using Hospital_Management.Models.Entities;

namespace Hospital_Management.Models.EntitiesDto
{
    public class DoctorDto
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public long Phoneno { get; set; }
        public string Specialization { get; set; }
        public bool IsActive { get; set; } = true;
        public int DepartmentId { get; set; }
        public DateTime IsCreated { get; set; }

       
    }
}
