using Microsoft.AspNetCore.Mvc;

namespace WebApiCore.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {

        [HttpGet]
        public  IActionResult Display()
        {
            return Ok("Hello My Name is Rajat Pandit");
        }


        [HttpGet("search")]
        public IActionResult PostData([ModelBinder(typeof(ModelBinding))] string[] name)
        {
            return Ok(name);
        }
    }
}
