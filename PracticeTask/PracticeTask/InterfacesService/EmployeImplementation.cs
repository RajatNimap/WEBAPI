using Microsoft.EntityFrameworkCore.Storage;
using PracticeTask.Data;
using PracticeTask.Model;
using PracticeTask.Model.Entities;

namespace PracticeTask.InterfacesService
{
    public class EmployeImplementation : IEmployeeCrud
    {
        private readonly DataContext Database;
        public EmployeImplementation(DataContext database)
        {
           Database = database;   
        }
       
        public EmployeeService AddEmployeeDetail(EmployeDto employeDto)
        {

            if(employeDto == null)
            {
                return null;
            }
            string HashedPassword=BCrypt.Net.BCrypt.HashPassword(employeDto.Password);  
            var data = new EmployeeService
            {
                Name = employeDto.Name, 
                email =employeDto.email,
                Password=HashedPassword
            };

            Database.employeeServices.Add(data);    
            Database.SaveChanges();
            return data;                
        }

        public bool DeleteEmployee(int id)
        {
            if (id == null)
            {
                return false;
            }
            try
            {
                var data = Database.employeeServices.FirstOrDefault(x => x.Id == id);
                Database.employeeServices.Remove(data);
                Database.SaveChanges();
            }
            catch (Exception ex) { 
                    return false;
            }
            return true;
        }

        public List<EmployeeService> GetAllDetail()
        {
            var data = Database.employeeServices.ToList();
            if(data == null)
            {
                return null;    
            }
            return data;    
        }
        public EmployeeService GetemployeeDetailbyId(int id)
        {
            var data=Database.employeeServices.FirstOrDefault(x => x.Id == id); 
            if(data == null)
            {
                return null;
            }
            return data;
        }

        public EmployeeService UpdateEmployee(int id, EmployeDto emp)
        {
            if (id == null)
            {
                return null;
            }
            string HashedPassword = BCrypt.Net.BCrypt.HashPassword(emp.Password);
            var data =Database.employeeServices.FirstOrDefault(x=>x.Id==id);
            data.Name   = emp.Name;
            data.email = emp.email;
            data.Password = HashedPassword;
            Database.SaveChanges();
            return data;
        }
    }
}
