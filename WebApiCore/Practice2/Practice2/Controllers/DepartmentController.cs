using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Practice2.Models;
using Practice2.Services;

namespace Practice2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentController : ControllerBase
    {
        private readonly IDepartment dep;
        public DepartmentController(IDepartment _dep)
        {
            dep= _dep;  
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll()
        {
                var data =  await dep.GetDepartments();
                 if(data == null)
                 {
                       return BadRequest("data is null");

                 }
            return Ok(data);
        }

        [HttpPost]  
        public async Task<IActionResult> Adddepartment(DepartmentModel depart)
        {
            var data = await dep.AddDepartment(depart);

            if (!data) {
                return BadRequest("deparement not added");
            }
            return Ok("department is Created");
           
        }
        [HttpPut]
        public async Task<IActionResult> UpdateDepartment(DepartmentModel department,int id)
        {
            var data = await dep.UpdateDepartment(department, id);
            if (!data)
            {
                return BadRequest("deparement not added");
            }
            return Ok("department is Created");
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetbyId(int id)
        {
             var data = await dep.GetDepartmentById(id);
            if (data == null)
            {
                return BadRequest("data is null");

            }
            return Ok(data);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleeteDepartement(int id)
        {
                var data = await dep.DeleteDepartment(id);
            if (!data)
            {
                return BadRequest("deparement not added");
            }
            return Ok("department is Created");

        }
        
    }
}
