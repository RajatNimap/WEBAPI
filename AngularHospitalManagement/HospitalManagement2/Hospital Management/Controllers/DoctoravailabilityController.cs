using Hospital_Management.Interfaces.Services;
using Hospital_Management.Models.Entities;
using Hospital_Management.Models.EntitiesDto;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hospital_Management.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorAvailabilityController : ControllerBase
    {
        private readonly IDoctorAvaliability _doctorAvailabilityService;

        public DoctorAvailabilityController(IDoctorAvaliability doctorAvailabilityService)
        {
            _doctorAvailabilityService = doctorAvailabilityService;
        }

        // GET: api/DoctorAvailability
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DoctorAvailability>>> GetAll()
        {
            var result = await _doctorAvailabilityService.GetAllAsync();
            return Ok(result);
        }

        // GET: api/DoctorAvailability/{doctorId}
        [HttpGet("{doctorId}")]
        public async Task<ActionResult<DoctorAvailability>> GetById(int doctorId)
        {
            var result = await _doctorAvailabilityService.GetByIdAsync(doctorId);
            if (result == null)
                return NotFound("Doctor availability not found.");

            return Ok(result);
        }

        // POST: api/DoctorAvailability
        [HttpPost]
        public async Task<ActionResult<DoctorAvailibilityDTO>> Create([FromBody] DoctorAvailibilityDTO dto)
        {
            if (dto == null)
                return BadRequest("Invalid data.");

            var result = await _doctorAvailabilityService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { doctorId = result.DoctorId }, result);
        }

        // PUT: api/DoctorAvailability/{doctorId}
        [HttpPut("{doctorId}")]
        public async Task<ActionResult<DoctorAvailibilityDTO>> Update(int doctorId, [FromBody] DoctorAvailibilityDTO dto)
        {
            var result = await _doctorAvailabilityService.UpdateAsync(doctorId, dto);
            if (result == null)
                return NotFound("Doctor availability not found.");

            return Ok(result);
        }
    }
}
