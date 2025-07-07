using CrudMvc.DataContext;
using CrudMvc.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace CrudMvc.Sevices
{
    public class ServicesImplementaion : IEmployeeInterface
    {
        private readonly Datacontext Datbase;
        public ServicesImplementaion(Datacontext database) {

                     Datbase = database;
        }
        public async Task<EmployeeDto> CreateEmployee(EmployeeDto employee)
        {

            var data = new EmployeeDto
            {
                Name = employee.Name,   
                Email = employee.Email, 
                Age = employee.Age
            };
            await Datbase.AddAsync(data);
            Datbase.SaveChanges();
            return data;
        }

        public async Task<bool> DeleteEmployee(int id)
        {
                var data = await Datbase.employees.FirstOrDefaultAsync(x => x.Id == id);   
                 Datbase.Remove(data);
            await Datbase.SaveChangesAsync();
            return true;

        }

        public async Task<List<Employee>> GetEmployeedata()
        {
            var data = await Datbase.employees.ToListAsync();
            return data;

        }

        public async Task<Employee> UpdateEmployee(int id)
        {
            var data= await Datbase.employees.FirstOrDefaultAsync(x=>x.Id==id);

            var data1 = new EmployeeDto
            {
                Name = data.Name,
                Email = data.Email,
                Age = data.Age
            };
            await Datbase.SaveChangesAsync();
            return data;
        }
    }
}
