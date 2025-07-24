using System.Data;
using Hospital_Management.Interfaces.Implementation;
using Hospital_Management.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Hospital_Management.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly IReportDashboard _dashboardService;

        public DashboardController(IReportDashboard dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboardData(DateOnly date)
        {
            var data = await _dashboardService.getDailyCount(date);
            var json = JsonConvert.SerializeObject(data, Formatting.Indented);
            if (data == null)
            {
                return NotFound("No data found for the dashboard.");
            }


            return Ok(json);
        }

        [HttpGet("dashboard/doctor")]
        public async Task<IActionResult> GetDoctorpercentage(int doctorId, DateOnly date)
        {
            var data = await _dashboardService.getPercentage(doctorId, date);
            if (data == null)
            {
                return NotFound("No data found for the doctor.");
            }
            // var json = JsonConvert.SerializeObject(data, Formatting.Indented);
            return Ok(data);
        }

        [HttpGet("dashboard/PatientFrequency/{month}")]

        public async Task<IActionResult> GetPatientFrequency(int month)
        {
            var data = await _dashboardService.PatientFrequencyDto(month);
            if (data == null)
            {
                return NotFound("No data found for the patient frequency.");
            }
            // var json = JsonConvert.SerializeObject(data, Formatting.Indented);
            return Ok(data);    

        }
    }
}
