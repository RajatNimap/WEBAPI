using Hospital_Management.Interfaces.Implementation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hospital_Management.Controllers
{
    [Route("[Controller]")]
    public class RecapController : Controller
    {
        private readonly PatientsImplementation patients;
        public RecapController(PatientsImplementation patients) {
            this.patients = patients;
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


    }
}
