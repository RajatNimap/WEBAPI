using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRUD.MODEL.Entities
{
    public class Employee
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public string? Position { get; set; }   
        public string ?Department { get; set; }  
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string? Address { get; set; }
    }
}
