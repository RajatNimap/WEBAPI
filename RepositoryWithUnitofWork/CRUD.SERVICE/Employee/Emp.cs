using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRUD.DATA.Infrastructure;
using CRUD.MODEL.Entities;
using CRUD.MODEL.Model;


namespace CRUD.SERVICE
{
    public class Emp
    {
        private readonly IRepository<Employee> _repository;


        public Emp(IRepository<Employee> repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<Employee>> GetAllEmployeesAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<Employee> GetEmployeeByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task AddEmployeeAsync(EmployeeDto employee)
        {

            var data =new Employee
            {
                Name = employee.Name,
                Email = employee.Email,
                PhoneNumber = employee.PhoneNumber,
                Address = employee.Address,
                Age = employee.Age,
                Position = employee.Position,
                Department = employee.Department    
            };

            await _repository.AddAsync(data);   
        }
        public async Task UpdateEmployeeAsync(int id, EmployeeDto employee)
        {
            var existingEmployee = await _repository.GetByIdAsync(id);
            if (existingEmployee != null)
            {
                existingEmployee.Name = employee.Name;
                existingEmployee.Email = employee.Email;
                existingEmployee.PhoneNumber = employee.PhoneNumber;
                existingEmployee.Address = employee.Address;
                existingEmployee.Age = employee.Age;
                existingEmployee.Position = employee.Position;
                existingEmployee.Department = employee.Department;
                await _repository.UpdateAsync(existingEmployee);
            }
        }

        public async Task DeleteEmployeeAsync(int id)
        {
            var existingEmployee = await _repository.GetByIdAsync(id);
            await _repository.DeleteAsync(existingEmployee);
        }

    }
}
