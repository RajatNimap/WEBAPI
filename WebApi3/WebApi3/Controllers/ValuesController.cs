using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApi3.Data;
using WebApi3.Model.Entities;

namespace WebApi3.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {

        private readonly DataContext datacontext;
        public ValuesController(DataContext datacontext) {

            this.datacontext = datacontext;
               
        }

        [HttpGet]
        public async Task<IActionResult> GetData()
        {
            var data = datacontext.superheros.ToList();
            if (data.Count == 0) {
                return NotFound();
            }
            return Ok(data);
            
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> Getdata(int id)
        {
            var data = datacontext.superheros.Find(id);
            if (data == null) { 
                return NotFound();
            }
            return Ok(data);
        }
        [HttpPost]
        public async Task<IActionResult> Postdata(Superhero superhero)
        {
           datacontext.superheros.Add(superhero);
           datacontext.SaveChanges();
           return Ok(datacontext.superheros.ToList());           
        }
        [HttpPut]
        [Route("{id}")]

        public async Task<IActionResult>Updatedata(Superhero update,int id)
        {
            var res=datacontext.superheros.Find(id);
            if (res == null) { 
                return BadRequest();    
            }
            res.Name = update.Name; 
            res.FirstName = update.FirstName;  
            res.LastName = update.LastName;
            res.Place=update.Place;
            datacontext.SaveChanges();
            return Ok(datacontext.superheros.ToList());
        }
        [HttpDelete]
        [Route("{id}")]

        public async Task<IActionResult> Deletedat(int id)
        {
            var res = datacontext.superheros.Find(id);
            if (res == null) {

                return NotFound();
            }
            datacontext.superheros.Remove(res);
            datacontext.SaveChanges();
            return Ok(datacontext.superheros.ToList());

        }
    }
}
