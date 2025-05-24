using Hospital_Management.Data;
using Hospital_Management.Interfaces.Services;
using Hospital_Management.Models.Entities;
using Hospital_Management.Models.EntitiesDto;
using Microsoft.EntityFrameworkCore;

namespace Hospital_Management.Interfaces.Implementation
{
    public class DepartmentImplement : IDepartment
    {
        private readonly DataContext Database;
        public DepartmentImplement(DataContext database)
        {
            Database = database;
        }

        public async Task<List<Department>> GetDepartment()
        {
            var Data = await Database.department.ToListAsync();
            if (Data == null) {

                return null;
            }
            return Data;    
        }
        public async Task<Department> GetDepartmentsById(int id)
        {
            var Data = await Database.department.FirstOrDefaultAsync(x => x.Id == id);
            if (Data == null) {
                return null;
            }
            return Data;
        }
        public async Task<Department> InsertDepartmentDetail(DepartmentDto department)
        {
            var Data = new Department
            {
                DepartmentName = department.DepartmentName,
            };

            await Database.department.AddAsync(Data);
            await Database.SaveChangesAsync();
            return Data;
        }

        public async Task<Department> UpdateDepartmentDetail(int id,  DepartmentDto departmentDto)
        {
            var Data = await Database.department.FirstOrDefaultAsync(x => x.Id == id);
                Data.DepartmentName = departmentDto.DepartmentName;
                await Database.SaveChangesAsync();
                if (Data == null)
                {
                    return null;
                }
                return Data;
        }

        public async Task<bool> DeleteDepartmentDetail(int id)
        {
            var Data=await Database.department.FirstOrDefaultAsync(x=>x.Id == id);
            if (Data == null) { return false; }
            Database.department.Remove(Data);
            Database.SaveChanges();
            return true;
        }
    }

}
