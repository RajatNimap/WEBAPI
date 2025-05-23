using System.ComponentModel.DataAnnotations;

namespace Hospital_Management.Models.Entities
{
    public class Doctors
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }    
        public string Email { get; set; }
        public long Phoneno { get; set; }
        public string Specialization { get; set; }
        public bool IsActive { get; set; }  = true;
        public DateTime IsCreated { get; set; }

        //Foreign Key
        public int DepartmentId { get; set; }
        public Department Department { get; set; }
    }
}
