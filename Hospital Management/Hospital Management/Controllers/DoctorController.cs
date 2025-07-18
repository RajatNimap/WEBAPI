using Hospital_Management.Interfaces.Implementation;
using Hospital_Management.Models.EntitiesDto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Identity.Client;

namespace Hospital_Management.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorController : ControllerBase
    {
        public readonly DoctorImplementation DoctorIm;
        public DoctorController(DoctorImplementation DoctorIm)
        {
            this.DoctorIm = DoctorIm;
        }

        [HttpGet]
        [Authorize(Roles = "admin,receptionist")]
        public async Task<IActionResult> GetData()
        {
            var Data = await DoctorIm.GetDoctors();
            if (Data == null) {
                return NotFound("Data Not found");
            }
            return Ok(Data);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDatabyId(int id)
        {
            var Data = await DoctorIm.GetDoctorssById(id);
            if (Data == null) { return NotFound("Data Not Found"); }
            return Ok(Data);
        }
        [HttpPost]
        public async Task<IActionResult> PostData(DoctorDto doctorDto)
        {
            if (doctorDto == null) {
                return BadRequest("Please Enter the Valid Detail");
            }
            var Data = await DoctorIm.InsertDoctorsDetail(doctorDto);
            return Ok(Data);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateData(int id, DoctorDto doctorDto)
        {

            var Data = await DoctorIm.UpdateDoctorsDetail(id, doctorDto);
            if (Data == null) {

                return NotFound("Data Not Found");
            }
            return Ok(Data);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRecord(int id)
        {
            var Data = await DoctorIm.DeleteDoctorsDetail(id);
            if (Data == null)
            {

                return NotFound("Doctor not exist");
            }
            return Ok(Data);
        }
        [HttpPost("Leave")]
        public async Task<IActionResult> PostLeave([FromBody] DoctorLeaveDto leaveDto)
        {
            if (leaveDto == null)
            {
                return BadRequest("Please Enter the Valid Detail");
            }
            var Data = await DoctorIm.MarkdoctorLeave(leaveDto);    
            if (Data == null)
            {
                return BadRequest("Please Enter the Valid Detail");
            }
            return Ok(Data);
        }
    }
}
