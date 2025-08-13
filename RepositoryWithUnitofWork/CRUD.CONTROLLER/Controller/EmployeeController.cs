using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using CRUD.SERVICE;
using Microsoft.AspNetCore.Mvc;

namespace CRUD.CONTROLLER.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController: ControllerBase
    {

        private readonly Emp emp;

        public EmployeeController(Emp emp)
        {
            this.emp = emp;
        }

        [HttpGet]
        public async Task<IActionResult> GetalltheEmploye()
        {

            var data=  await emp.GetAllEmployeesAsync();   
            if(data == null)
            {
                return NotFound();
            }
            return Ok(data);    

        }
      
    }
}
