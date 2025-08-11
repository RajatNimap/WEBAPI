using Hospital_Management.Data;
using Hospital_Management.Interfaces.Implementation;
using Hospital_Management.Models.Entities;
using Hospital_Management.Models.EntitiesDto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Hospital_Management.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentController : ControllerBase
    {
        private readonly DepartmentImplement DepartmenIm;
        public DepartmentController(DepartmentImplement department) {
            DepartmenIm = department;
        }

        [HttpGet]
      //  [Authorize (Roles = "admin,receptionist")]
        public async Task<IActionResult> GetDepartment()
        {
            var Data = await DepartmenIm.GetDepartment();
            if (Data == null) {
                return NotFound("data not found");
            }
            return Ok(Data);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDepartmentById(int id)
        {
            var Data = await DepartmenIm.GetDepartmentsById(id);
            if (Data == null) {

                return NotFound("Data Not Found");
            }
            return Ok(Data);
        }
        [HttpPost]
        public async Task<IActionResult> CreateDepartment([FromBody] DepartmentDto departmentDto)
        {
            if (departmentDto == null) { return BadRequest("Please Enter the detail"); }

            var Data=await DepartmenIm.InsertDepartmentDetail(departmentDto);

            return Ok(Data);    
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDepartment(int id, [FromBody] DepartmentDto departmentDto)
        {
            var Data = await DepartmenIm.UpdateDepartmentDetail(id, departmentDto);
            if (Data == null) {

                return NotFound("DataNotfound");

            }   
            return Ok(Data);    
        }
        [HttpDelete]

        public async Task<IActionResult> DeleteDepartment(int id)
            {

                var Data = await DepartmenIm.DeleteDepartmentDetail(id);

                if (Data == null)
                {
                    return NotFound("Department are Not Exist");
                }
                return Ok("Department are Deleted");

        }
    }
}
