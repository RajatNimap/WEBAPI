using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PracticeTask.InterfacesService;
using PracticeTask.Model;

namespace PracticeTask.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeController : ControllerBase
    {
        private readonly IEmployeeCrud Empdata;

        public EmployeController(IEmployeeCrud employeeCrud) { 
          
            
            Empdata = employeeCrud;   
        }
        [HttpGet]
        public IActionResult Getalldata()
        {
            var data =  Empdata.GetAllDetail();
            if (data == null) { 
                return NotFound("employee not found");
            }
            return Ok(data);    
        }
        [HttpGet]
        [Route("{id}")]
        public IActionResult Getdetail(int id) {
            var data = Empdata.GetemployeeDetailbyId(id);
            if (data == null) {
                return NotFound("employee not found");
            }

            return Ok(data);
        }

        [HttpPost]
        public IActionResult Postdata([FromBody]EmployeDto emp) { 
               
            Empdata.AddEmployeeDetail(emp);
            return Ok(Empdata); 
        }
        [HttpPut]
        [Route("{id}")]
        public IActionResult UpdateData(int id,[FromBody]EmployeDto emp)
        {

            Empdata.UpdateEmployee(id, emp);
            return Ok(Empdata); 
        }

        [HttpDelete]
        public IActionResult Delete(int id) { 
        
            Empdata.DeleteEmployee(id);
            return Ok(Empdata);
        }
    }
}
