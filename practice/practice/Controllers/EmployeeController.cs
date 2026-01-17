using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace practice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly DataContext _dataContext;
        public EmployeeController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        [HttpGet]
        public ActionResult getdata()
        {

            var data = _dataContext.employee.ToList();
            return Ok(data);
        }
        [HttpPost]

        public ActionResult postdata(emp e)
        {
            var em = new emp
            {
                id = e.id,
                name = e.name,
            };
            _dataContext.employee.Add(em);
            return Ok(em);

        }



    }
}
