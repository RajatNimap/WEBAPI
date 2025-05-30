using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Mvc;
using Modelviewcontroller.Model;

namespace Modelviewcontroller.Controllers
{
    public class HomeController : Controller
    {
        [Route("~/")]
        public IActionResult Index()
        {

            ViewData["Title"] = "HomePage";
            ViewData["Data1"] = "Rajat";
            ViewData["Data2"] = 30;
            ViewData["Data3"] = DateTime.Now;
            string[] arr = { "Rajat","Niraj","Pandit"};
            ViewData["Data4"] = arr;
            var List=new List<string> { "India","Usa","Canada"};
            ViewData["Data5"] = List;

            ViewBag.data1 = "Niraj Pandit";
            ViewBag.data2 = 15;
            ViewBag.data3 = DateTime.Now;
            string[] arr1 = { "Rajat", "Niraj", "Pandit" };

            ViewBag.data4 = arr1;
            ViewBag.data5 = new List<string> { "India", "Usa", "Canada" };
            TempData["name"] = "Suraj Shah";
            TempData["Age"] = 23;
            TempData["Time"] = DateTime.UtcNow;
            TempData["list"] = new List<string> { "Abc", "BCD", "ABCD" };
            TempData.Keep();
            return View();
        }

        [Route("/about")]
        public IActionResult About()
        {
              var list = new List<Employe>{
                new Employe {EmpId=1,EmpName="Rajat",EmpDept="CS",Empsalary=200000 },
                new Employe {EmpId=2,EmpName = "Niraj",EmpDept="AI",Empsalary=250000 }
            };
            ViewBag.emp = list;
            return View();
        }
        [Route("/contact")]
        public IActionResult Contact() {
           // TempData.Keep();   
            return View();  
        }  

    }
}
