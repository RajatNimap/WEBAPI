using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Practice2.Model.Entities;

namespace Practice2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActionController : ControllerBase
    {
        [HttpGet]
        [Route("Emp")]
        public string GetData()
        {
            return "Hello World";
        }
        [HttpGet]
        [Route("Empdetail")]
        public EmployeeModel empdetail() { 
            
            return new EmployeeModel() { Id = 1, Name = "rajat" }; 
        }

    }
}
