using OfficeOpenXml;

namespace LearningEPpluse.Services
{
    public class ExcelServices
    {
        private readonly IConfiguration _config;
        public ExcelServices(IConfiguration config  )
        {
            _config = config;
        }

        public async Task<string> GetExcelData()
        {

            using (var pakagge = new ExcelPackage())
            {
                var ws = pakagge.Workbook.Worksheets.Add("Sheet1");
                ws.Cells["A1"].Value = "Name";
                ws.Cells["B1"].Value = "Age";   
                ws.Cells["A2"].Value = "Rajat";
                ws.Cells["B2"].Value = 23;

                for(int i = 1; i <= 10; i++)
                {
                    ws.Cells[$"C{i}"].Value = "My Name is Rajat Pandit";

                }
                var file = new FileInfo($"{new Guid()}MyFirstExcel1.xlsx");


                await pakagge.SaveAsAsync(file);    
            }
            using (var sheet = new ExcelPackage())
            {

                var ws = sheet.Workbook.Worksheets.Add("Yardi TB");
                var ws2 = sheet.Workbook.Worksheets.Add("LOC");
                var ws3 = sheet.Workbook.Worksheets.Add("Interco");
                var ws4 = sheet.Workbook.Worksheets.Add("BS Detail");
                var ws5 = sheet.Workbook.Worksheets.Add("Interest EIDL");
                var path = _config["AppSettings:"]
                var file = new FileInfo("filetemp.xlsx");
                await sheet.SaveAsAsync(file);

            }



            return "Rajat Pandit is Learning EP Plus";
            

        }

        //public async Task<string> yardiExcel(string yardipath)
        //{

          
        //    using (var yardiInput = new ExcelPackage(new FileInfo(yardipath)))
        //    {


        //        var wsy = yardiInput.Workbook.Worksheets[0];
                


        //    }

           

        //}

    }
    

}
