using Hospital_Management.Interfaces.Implementation;
using Hospital_Management.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
        [Authorize]
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
        [Authorize]
        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetPatientsById(int id)
        {
            var Data=await patientIm.GetPatientsById(id);
            if (Data == null) {
                return NotFound("Data is Not found");
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
        public async Task<IActionResult> PatientsUpdate(int id,[FromBody]  PatientsDto patientsDto)
        {
            var Data = await patientIm.UpdatePatientDetail(id,patientsDto);

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
    }
}
