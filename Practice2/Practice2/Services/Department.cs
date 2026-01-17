
using Microsoft.EntityFrameworkCore;
using Practice2.Data;
using Practice2.Models;
using System.Threading.Tasks;

namespace Practice2.Services
{
    public class Department : IDepartment
    {
        private readonly DataContext _db;
        public Department(DataContext db)
        {
            _db = db;  

        }

        public async Task<bool> AddDepartment(DepartmentModel department)
        {
            var data = new DepartmentModel
            {
                Name = department.Name, 

            };

            _db.department.Add(data);

           int res= await _db.SaveChangesAsync();
            
           if(res > 0)
            {
                return true;
            }
            return false;
        }



        public async Task<bool> DeleteDepartment(int id)
        {
            var data = await _db.department.FirstOrDefaultAsync(x => x.Id == id);
            _db.department.Remove(data);

            int res = await _db.SaveChangesAsync();
            if (res > 0)
            {
                return true;
            }
            return false;
        }

        public async Task<DepartmentModel> GetDepartmentById(int id)
        {
             var data = await _db.department.FirstOrDefaultAsync(x=>x.Id==id);
              return data;
        }

       public async Task<List<DepartmentModel>> GetDepartments()
        {
            var data = await _db.department.ToListAsync();
            return data;
        }

        public async Task<bool> UpdateDepartment(DepartmentModel department,int id)
        {

            var data = await _db.department.FirstOrDefaultAsync(x=>x.Id == id);
            if(data != null)
            {
                return false;
            }

            data.Name =department.Name;
            
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
