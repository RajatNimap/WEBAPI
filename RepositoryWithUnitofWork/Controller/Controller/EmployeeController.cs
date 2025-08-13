using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using CRUD.MODEL.Model;
using CRUD.SERVICE;
using Microsoft.AspNetCore.Mvc;

namespace CRUDCONTROLLER.Controller
{

    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {

        public readonly Emp _services;
        public EmployeeController(Emp services)
        {
            _services = services;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllEmployees()
        {
            var employees = await _services.GetAllEmployeesAsync();
            return Ok(employees);
        }

        [HttpGet("{id}")]

        public async Task<IActionResult> GetEmployeeById(int id)
        {
            var employee = await _services.GetEmployeeByIdAsync(id);
            if (employee == null)
            {
                return NotFound();
            }
            return Ok(employee);
        }
        [HttpPost]
        public async Task<IActionResult> AddEmployee([FromBody] EmployeeDto employee)
        {
            if (employee == null)
            {
                return BadRequest("Employee data is null");
            }
            var createdEmployee = await _services.AddEmployeeAsync(employee);
            return CreatedAtAction(nameof(GetEmployeeById), new { id = createdEmployee.Id }, createdEmployee);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEmployee(int id, [FromBody] EmployeeDto employee)
        {
            if (employee == null)
            {
                return BadRequest("Employee data is null");
            }
            var result = await _services.UpdateEmployeeAsync(id, employee);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var result = await _services.DeleteEmployeeAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
    
}
