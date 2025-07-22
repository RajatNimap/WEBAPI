using Hospital_Management.Interfaces.Implementation;
using Hospital_Management.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage;
using Newtonsoft.Json;

namespace Hospital_Management.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientsController : ControllerBase
    {
        private readonly PatientsImplementation patientIm;

        public PatientsController(PatientsImplementation patientIm)
        {
            this.patientIm = patientIm;
        }
        [Authorize(Roles = "admin")]
        [HttpGet("Patients")]
        public async Task<IActionResult> GetPatientsList()
        {
            var Data = await patientIm.GetPatientsModelsDetails();
            if (Data == null)
            {
                return NotFound("Data is Not found");

            }
            return Ok(Data);
        }

        [Authorize(Roles = "receptionist")]
        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetPatientsById(int id)
        {
            var Data = await patientIm.GetPatientsById(id);
            if (Data == null) {
                return NotFound("Data is Not found");
            }

            return Ok(Data);
        }

        [HttpGet]
        [Route("app/{id}")]
        public async Task<IActionResult> GetAppointmentDetail(int id)
        {
            var Data = await patientIm.GetAppointmentDetail(id);
            if (Data == null) {
                return BadRequest("Data Not Found");
            }
            return Ok(Data);
        }


        [HttpPost]
        public async Task<IActionResult> PatientsPost([FromBody] PatientsDto patientsDto)
        {
            var Data = await patientIm.InsertPatientDetail(patientsDto);
            if (Data == null) {

                return BadRequest("Plese Enter the Details");

            }
            return Ok(Data);
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> PatientsUpdate(int id, [FromBody] PatientsDto patientsDto)
        {
            var Data = await patientIm.UpdatePatientDetail(id, patientsDto);

            if (Data == null) {
                return NotFound("DataNotfound");
            }
            return Ok(Data);
        }
        [HttpDelete]
        public async Task<IActionResult> RemovePatients(int id)
        {
            var Data = patientIm.DeletePatientDetail(id);
            if (Data == null)
            {
                return NotFound("Data Not found");
            }
            return Ok("Record are deleted");
        }

        [HttpGet("search/{value}")]
        public async Task<IActionResult> SearchPatients(string value)
        {
            var Data = await patientIm.Searching(value);
            if (Data == null || Data.Count == 0)
            {
                return NotFound("No patients found with the given search criteria.");
            }
            return Ok(Data);
        }
        [HttpGet("medical-records/{patientId}")]
        public async Task<IActionResult> GetMedicalRecords(int patientId)
        {
            var Data = await patientIm.GettingAllmedicalRecord(patientId);
            if (Data == null || Data.Count == 0)
            {
                return NotFound("No medical records found for the given patient.");
            }
            return Ok(Data);
        }

        [HttpGet("AlldataofPatients")]
        public async Task<IActionResult> Getalldatafrompaitnet(int patientId)
        {
            var data = await patientIm.GetAllRecordofPatientLinked(patientId);
            var json =JsonConvert.SerializeObject(data,Formatting.Indented);    
            if (data == null)
            {
                return NotFound("No  records found for the given patient.");

            }
            return Ok(json);    
        }
    }
}
