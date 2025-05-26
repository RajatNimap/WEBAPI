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

        public AppointmentBookingController(AppointmentImplementation implementation)
        {
           _implementation = implementation; 
        }

        [HttpPost]
        public async Task<IActionResult> AppointmentBooking(AppointmentDto appointmentDto)
        {
            if (appointmentDto == null) { 
                    return BadRequest("Please the details");    
            }
            var Data = await _implementation.AppointmentBooking(appointmentDto);

            return Ok(Data);    
        }
       
    }
}
