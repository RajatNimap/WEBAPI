using Hospital_Management.Interfaces.Implementation;
using Hospital_Management.Models.EntitiesDto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;

namespace Hospital_Management.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentBookingController : ControllerBase
    {

        private readonly AppointmentImplementation _implementation;
        private readonly EmailService _emailService;

        public AppointmentBookingController(AppointmentImplementation implementation, EmailService emailService )
        {
            _implementation = implementation;
            _emailService = emailService;
        }

        [HttpPost]
        public async Task<IActionResult> AppointmentBooking(AppointmentDto appointmentDto)
        {
            if (appointmentDto == null) { 
                    return BadRequest("Please the details");    
            }
            var Data = await _implementation.AppointmentBooking(appointmentDto);
            if (Data == null) {
                return BadRequest("Invalid");
            }

            return Ok(Data);    
        }
        [HttpPut("cancel/{id}")]
        public async Task<IActionResult> CancelAppointment(int id)
        {
            var result = await _implementation.CancelAppointment(id);
            if (!result) return NotFound("Appointment not found or already cancelled");
            return Ok("Appointment cancelled successfully.");
        }

        [HttpPut("reschedule/{id}")]
        public async Task<IActionResult> RescheduleAppointment(int id, [FromBody] AppointmentDto updatedAppointment)
        {
            var result = await _implementation.RescheduleAppointment(id, updatedAppointment);
            if (!result) return BadRequest("Invalid rescheduling");
            return Ok("Appointment rescheduled successfully.");
        }


    }
}
