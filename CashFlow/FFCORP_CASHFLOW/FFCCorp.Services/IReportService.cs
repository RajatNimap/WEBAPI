using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFCCorp.Services
{
    public interface IReportService
    {
        Task<string> BalanceSheetReport(string baseDirectory,string yardi,string loc,string bsdetail,string interco,string interestEidl);


    }
}
