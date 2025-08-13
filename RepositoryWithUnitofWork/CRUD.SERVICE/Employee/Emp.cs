using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRUD.DATA.Infrastructure;
using CRUD.DATA.Unitowork;
using CRUD.MODEL.Entities;
using CRUD.MODEL.Model;

namespace CRUD.SERVICE
{
    public class Emp
    {

        private readonly IUnitofWork _unitOfWork;   
        public Emp(IUnitofWork unitofWork)
        {
            _unitOfWork = unitofWork;
        }

        public async Task<IEnumerable<Employee>> GetAllEmployeesAsync()
        {
            return await _unitOfWork.employe.GetAllAsync();
        }

        public async Task<Employee> GetEmployeeByIdAsync(int id)
        {
            return await _unitOfWork.employe.GetByIdAsync(id);
        }

        public async Task<Employee> AddEmployeeAsync(EmployeeDto employee)
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

            await _unitOfWork.employe.AddAsync(data);
            await _unitOfWork.SaveCommitChanges();
            return data;
        }
        public async Task<bool> UpdateEmployeeAsync(int id, EmployeeDto employee)
        {
            var existingEmployee = await _unitOfWork.employe.GetByIdAsync(id);
            if(existingEmployee == null)
            {
                return false; // Employee not found
            }   
      
                existingEmployee.Name = employee.Name;
                existingEmployee.Email = employee.Email;
                existingEmployee.PhoneNumber = employee.PhoneNumber;
                existingEmployee.Address = employee.Address;
                existingEmployee.Age = employee.Age;
                existingEmployee.Position = employee.Position;
                existingEmployee.Department = employee.Department;
               
            
            await _unitOfWork.employe.UpdateAsync(existingEmployee);
            await _unitOfWork.SaveCommitChanges();
            return true;
        }

        public async Task<bool> DeleteEmployeeAsync(int id)
        {
            var existingEmployee = await _unitOfWork.employe.GetByIdAsync(id);
            if (existingEmployee == null)
            {
                return false; // Employee not found
            }   

            await _unitOfWork.employe.DeleteAsync(existingEmployee);
            await _unitOfWork.SaveCommitChanges();  
            return true; 
        }

    }
}
