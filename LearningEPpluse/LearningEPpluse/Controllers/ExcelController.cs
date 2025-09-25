using LearningEPpluse.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LearningEPpluse.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExcelController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ExcelServices _excel;
        public ExcelController(IConfiguration configuration,ExcelServices excel)
        {
            _configuration = configuration;
            _excel = excel; 
        }   
        [HttpGet("Yardi")]  
        public async Task<IActionResult> Yardi()
        {   
            var httpRequest=HttpContext.Request;
            string BaseDirc = _configuration["AppSettings:BaseDirectory"] ??  string.Empty;

            string YardiTb = string.Empty;
            string Loc= string.Empty;   
            string Interco=string.Empty;
            string BsDetail=string.Empty;
            string InterestEIDL=string.Empty;

            foreach (var file in httpRequest.Form.Files)
            {
                var fileName = file.FileName;
                if (fileName.Equals("Scheduler_Reports.xlsx"))
                {
                    YardiTb = Path.Combine(BaseDirc, $"{file.Name}_{Guid.NewGuid()}.xlsx");
                    using (var stream = new FileStream(YardiTb, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                }
                if (fileName.Equals("loc-1756906948114.xlsx"))
                {
                    Loc = Path.Combine(BaseDirc, $"{file}_{Guid.NewGuid()}.xlsx");
                    using (var stream = new FileStream(Loc, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                }
                if (fileName.Equals("TB 2022.xlsx"))
                {
                    Interco = Path.Combine(BaseDirc, $"{file}_{Guid.NewGuid()}.xlsx");
                    using (var stream = new FileStream(Interco, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                }
                if (fileName.Equals("Interest Exp - EIDL.xlsx"))
                {
                    BsDetail = Path.Combine(BaseDirc, $"{file}_{Guid.NewGuid()}.xlsx");
                    using (var stream = new FileStream(BsDetail, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                }

            }

           // return Ok (await _excel.GetExcelData());  
           //return Ok(await _excel.yardiExcel(YardiTb));
           return Ok(await _excel.LOCFileter(Loc));



        }   
        
    }
}
