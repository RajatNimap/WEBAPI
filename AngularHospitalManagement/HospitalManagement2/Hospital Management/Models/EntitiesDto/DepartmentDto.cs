using System.ComponentModel.DataAnnotations;

namespace Hospital_Management.Models.EntitiesDto
{
    public class DepartmentDto
    {

        [Required]
        public string DepartmentName { get; set; }

    }
}
