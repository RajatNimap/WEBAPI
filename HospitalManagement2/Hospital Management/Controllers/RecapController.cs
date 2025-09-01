using Hospital_Management.Interfaces.Implementation;
using Hospital_Management.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hospital_Management.Controllers
{
    [Route("[Controller]")]
    public class RecapController : Controller
    {
        private readonly PatientsImplementation _patients;
        public RecapController(PatientsImplementation patients) {
            _patients = patients;
        }

        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        [Authorize(Roles = "admin")]

        [Route("Patients")]
        public IActionResult Patients()
        {
            return View();  
        }

        [Route("RegisterPatient")]
        public IActionResult RegisterPatient()
        {
            return View();
        }


        public async Task<IActionResult> AddPatients(PatientsDto patients)
        {
            var data = await _patients.InsertPatientDetail(patients);
            if (data == null) { return BadRequest("Something Went Wrong"); }    
            return Ok();
   
        }

        [HttpGet("GetPatients")]
        public async Task<IActionResult> GetPatients()
        {
            var data = await _patients.GetPatientsModelsDetails();
            if (data == null) { return BadRequest("Something Went Wrong"); }
            return View(data);  
        }
        [HttpGet("PatientsViewById/{id}")]  
        public async Task<IActionResult> PatientsViewById(int id)
        {
            var data = await _patients.GetPatientsById(id);
            if (data == null) { return BadRequest("Something Went Wrong"); }
            return View(data);
        }

        [HttpPost("Edit/{id}")]
        public async Task<IActionResult> Edit(int id, PatientsDto patients)
        {
            var data = await _patients.UpdatePatientDetail(id, patients);
            if (data == null) { return BadRequest("Something Went Wrong"); }
            return Ok();
        }
        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var data = await _patients.GetPatientsById(id);
            if (data == null) { return BadRequest("Something Went Wrong"); }
            return View(data);
        }

        [HttpPost("Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            var data = await _patients.DeletePatientDetail(id);
            if (data == false) {


                return Json(new { success = false, message = "Patient not found successfully." });
            }

            return Json(new { success = true, message = "Patient deleted successfully." });
        }
    }
}
