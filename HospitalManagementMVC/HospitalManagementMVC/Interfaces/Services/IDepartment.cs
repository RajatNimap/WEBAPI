using Hospital_Management.Models.Entities;
using Hospital_Management.Models.EntitiesDto;

namespace Hospital_Management.Interfaces.Services
{
    public interface IDepartment
    {
        Task<List<Department>> GetDepartment();
        Task<Department> GetDepartmentsById(int id);
        Task<Department> InsertDepartmentDetail(DepartmentDto department);
        Task<Department> UpdateDepartmentDetail(int id, DepartmentDto departments);
        Task<bool> DeleteDepartmentDetail(int id);
    }
}
