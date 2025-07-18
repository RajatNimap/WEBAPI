using Hospital_Management.Interfaces.Services;
using Hospital_Management.Models.EntitiesDto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Hospital_Management.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MedicalRecordController : ControllerBase
    {
        private readonly IMedicalRecord medicalRecord;
        public MedicalRecordController(IMedicalRecord medical)
        {
            medical = medicalRecord;
        }

        [HttpGet]
        public async Task<IActionResult> GetMedicalRecords()
        {
            var records = await medicalRecord.GetAllMedicalRecord();
            if (records == null || !records.Any())
            {
                return NotFound("No medical records found.");
            }
            return Ok(records);
        }

        [HttpGet("/GetRecordbyId")]
        public async Task<IActionResult> GetMedicalRecordById(int id)
        {
            var record = await medicalRecord.GeMedicalRecordbyId(id);
            if (record == null)
            {
                return NotFound($"Medical record with ID {id} not found.");
            }
            return Ok(record);
        }
        [HttpGet("/pateintsId")]
        public async Task<IActionResult> GetMedicalRecordByPatientId(int patientId)
        {
            var record = await medicalRecord.GetMedicalRecordbyPatientId(patientId);
            if (record == null)
            {
                return NotFound($"Medical record for patient with ID {patientId} not found.");
            }
            return Ok(record);
        }

        [HttpPost]  
        public async Task<IActionResult> AddMedicalRecored(MedicalRecoredDto medical)
        {
                var data = await medicalRecord.AddMedicalRecord(medical);   

            if (data == null)
            {
                return BadRequest("not able to add record");
            }
           return Ok(data); 
        }
    }
}
