using System.ComponentModel.DataAnnotations;

namespace PracticeTask.Model.Entities
{
    public class EmployeeService
    {
        [Key]
        public int Id { get; set; } 
        public string Name { get; set; }    
        public string email { get; set; }
        public string Password { get; set; }
    }
}
