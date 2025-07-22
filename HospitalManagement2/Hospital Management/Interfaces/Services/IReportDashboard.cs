using System.Data;
using Hospital_Management.Models.EntitiesDto;

namespace Hospital_Management.Interfaces.Services
{
    public interface IReportDashboard
    {
        Task<DataTable> getDailyCount(DateOnly date); 
        Task<GetdailyCountdto> getPercentage(int doctorId,DateOnly date);
    }
}
