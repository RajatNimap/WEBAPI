using CrudMvc.Models;
using Microsoft.AspNetCore.Mvc;

namespace CrudMvc.Sevices
{
    public interface IEmployeeInterface
    {

        Task<List<Employee>> GetEmployeedata();
        Task<Employee> CreateEmployee(EmployeeDto employee);
        Task<Employee> UpdateEmployee(int id);
        Task<bool> DeleteEmployee(int id);
    }
}
