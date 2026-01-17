using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Practice2.Models
{
    public class DepartmentModel
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name  { get; set; }
        [JsonIgnore]
        public List<EmployeeModel> employees { get; set; }  = new List<EmployeeModel>();
    }
}
