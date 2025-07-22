using System.ComponentModel.DataAnnotations;

namespace Hospital_Management.Models.Entities
{
    public class Department
    {
        [Key]
        public int Id { get; set; }
        [Required]  
        public string DepartmentName { get; set; }
    }
}
