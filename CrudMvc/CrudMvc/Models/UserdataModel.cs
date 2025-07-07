namespace CrudMvc.Models
{
   

    public class Employee
    {
        public int Id { get; set; } 
        public string Name { get; set; }    
        public string Email { get; set; }   
        public int Age {  get; set; }   
    }
    public class EmployeeDto
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public int Age { get; set; }
    }

}
