using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System.ComponentModel.DataAnnotations;

namespace Practice2.Models
{
    public class EmployeeModel
    {

        [Key]
        public int Id { get; set; }
        [Required]  
        public string Name { get; set; }    
        public int Age { get; set; }
        public int DepartmentId { get; set; }
        public DepartmentModel department { get; set; }




    }
}
