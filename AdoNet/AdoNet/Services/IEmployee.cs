using AdoNet.Model;

namespace AdoNet.Services
{
    public interface IEmployee
    {
        public Task<List<Employee>> Getemployees();
        public Task<Employee> Getemployeebyid(int id);
        public Task<Employee> Addemployee(EmployeeModel employee);
        public Task<Employee> Updateemployee(int id, EmployeeModel employee);
        public Task<bool> Deleteemployee(int id);   


    }
}
