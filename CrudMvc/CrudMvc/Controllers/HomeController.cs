using CrudMvc.Models;
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

        public async Task<IActionResult> Index()
        {
            var data=await employeeInterface.GetEmployeedata();
            ViewBag.Data = data;
      
            return View();
        }
     
        public async Task<IActionResult> Addemployee(EmployeeDto emp)
        {
            await employeeInterface.CreateEmployee(emp);
            return Ok(new { message = "employee added successully" });            
        }
        public IActionResult newpage()
        { 
            return View();
        }
        public IActionResult Privacy()
        {
            return View();
        }   
    }
}
