using CrudMvc.Sevices;
using Microsoft.AspNetCore.Mvc;

namespace CrudMvc.Controllers
{
    public class HomeController : Controller
    {
        private readonly IEmployeeInterface employeeInterface;
        public HomeController(IEmployeeInterface employeeInterface)
        {
            this.employeeInterface = employeeInterface;
        }

        public IActionResult Index()
        {
            var data=employeeInterface.GetEmployeedata();
            ViewBag.Data = data;
            return View();
        }
    }
}
