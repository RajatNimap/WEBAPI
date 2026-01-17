using Practice2.Models;

namespace Practice2.Services
{
    public interface IDepartment
    {

       public  Task<List<DepartmentModel>> GetDepartments();
       public Task<DepartmentModel> GetDepartmentById(int id);
      
       public  Task<bool> AddDepartment(DepartmentModel department);
        public  Task<bool> UpdateDepartment(DepartmentModel department ,int id);    
        public  Task<bool> DeleteDepartment(int id);

    }
}
