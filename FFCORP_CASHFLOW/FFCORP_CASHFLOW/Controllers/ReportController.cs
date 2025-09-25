using FFCCorp.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FFCORP_CASHFLOW.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IReportService _excel;
        public ReportController(IConfiguration configuration, IReportService excel)
        {
            _configuration = configuration;
            _excel = excel;
        }
        [HttpGet("Yardi")]
        public async Task<IActionResult> Cashflow()
        {
            var httpRequest = HttpContext.Request;
            string BaseDirc = _configuration["AppSettings:BaseDirectory"] ?? string.Empty;

            string YardiTb = string.Empty;
            string Loc = string.Empty;
            string Interco = string.Empty;
            string BsDetail = string.Empty;
            string InterestEIDL = string.Empty;

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

            // await _excel.BalanceSheetReport(BaseDirc, YardiTb, Loc, BsDetail, Interco, InterestEIDL);
            await _excel.BalanceSheetReport(BaseDirc, YardiTb, Loc, Interco, "", "");

            return Ok("Success");   



        }
    }
}
