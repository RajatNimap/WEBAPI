using Hospital_Management.Interfaces.Implementation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Hospital_Management.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorSlotController : ControllerBase
    {

        private readonly DoctorSlotService _slotService;

        public DoctorSlotController(DoctorSlotService slotService)
        {
            _slotService = slotService;
        }
        
        [HttpGet("{doctorId}/slots")]
        public async Task<IActionResult> GetAvailableSlots(int doctorId, [FromQuery] DateTime date)
        {
            var slots = await _slotService.GetDoctorSlotsAsync(doctorId, date);

            if (slots.Count == 0)
                return NotFound("Doctor is not available on this day.");

            return Ok(slots);
        }
    }
}
