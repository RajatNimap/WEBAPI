using PracticeTask.Model;
using PracticeTask.Model.Entities;

namespace PracticeTask.InterfacesService
{
    public interface IEmployeeCrud
    {
        List<EmployeeService> GetAllDetail();
        EmployeeService GetemployeeDetailbyId(int id);
        EmployeeService AddEmployeeDetail(EmployeDto employeDto);
        EmployeeService UpdateEmployee(int id,EmployeDto employeDto);
        bool DeleteEmployee(int id);    

    }
}
