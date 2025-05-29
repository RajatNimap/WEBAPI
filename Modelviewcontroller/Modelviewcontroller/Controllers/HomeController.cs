using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Mvc;

namespace Modelviewcontroller.Controllers
{
    public class HomeController : Controller
    {



        [Route("~/")]
        public IActionResult Index()
        {
            ViewData["Data1"] = "Rajat";
            ViewData["Data2"] = 30;
            ViewData["Data3"] = DateTime.Now;
            string[] arr = { "Rajat","Niraj","Pandit"};
            ViewData["Data4"] = arr;
            var List=new List<string> { "India","Usa","Canada"};
            ViewData["Data5"] = List;
            return View();
        }

        [Route("/about")]
        public IActionResult About()
        {
            return View();
        }
        [Route("/contact")]
        public IActionResult Contact() { 
        
                return View();  
        }  

    }
}
