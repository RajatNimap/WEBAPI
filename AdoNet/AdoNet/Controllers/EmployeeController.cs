using AdoNet.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using AdoNet.Model;

namespace AdoNet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployee _employee;
        private DataContext _db;
        private readonly IConfiguration config;
        public EmployeeController(IEmployee employee,DataContext db, IConfiguration config)
        {
            _employee = employee;
            _db = db;
            this.config = config;
        }
        [HttpGet]
        public async Task<IActionResult> Getemployees()
        {
            // var data = await _employee.Getemployees();
            //return Ok(data);
            var employe = await _db.employees.FromSqlRaw("select * from employees").ToListAsync();
            return Ok(employe);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> Getemployeebyid(int id)
        {
            var data = await _employee.Getemployeebyid(id);
            return Ok(data);
        }
        [HttpPost]
        public async Task<IActionResult> Addemployee(EmployeeModel employee)
        {

            using (SqlConnection con = new SqlConnection(config.GetConnectionString("DefaultConnection")))
            {
                
            }

            var data = await _employee.Addemployee(employee);
            return Ok(data);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Updateemployee(int id, EmployeeModel employee)
        {
            var data = await _employee.Updateemployee(id, employee);
            return Ok(data);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Deleteemployee(int id)
        {
            var data = await _employee.Deleteemployee(id);
            return Ok(data);
        }
    }
}
