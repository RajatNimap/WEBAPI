using Microsoft.AspNetCore.Components.Forms;
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
                var path = _config["AppSettings:"];
                var file = new FileInfo("filetemp.xlsx");
                await sheet.SaveAsAsync(file);

            }



            return "Rajat Pandit is Learning EP Plus";
            

        }

        public async Task<string> yardiExcel(string yardipath)
        {

            //var template=_config["AppSettings:BaseDirectory"] ?? string.Empty;
            //var file=Path.Combine(template, "filetemp.xlsx");
            //var fileTemplate=new ExcelPackage(new FileInfo(file));  

            //using (var yardiInput = new ExcelPackage(new FileInfo(yardipath)))
            //{
            //    var wsy = yardiInput.Workbook.Worksheets[0];
            //    var wsTemplate = fileTemplate.Workbook.Worksheets["Yardi TB"];
                
            //        var range = wsy.Cells["A5:F394"];
            //    for (int row = 0; row < range.End.Row - range.Start.Row + 1; row++)
            //    {
            //        for (int col = 0; col < range.End.Column - range.Start.Column + 1; col++)
            //        {
            //            var source = wsy.Cells[range.Start.Row + row, range.Start.Column + col];
            //            var dest = wsTemplate.Cells[row + 1, col + 1];

            //            if (!string.IsNullOrEmpty(source.Formula))
            //                dest.Formula = source.Formula;   // keep formula
            //            else
            //                dest.Value = source.Value;       // copy value
            //        }
            //    }

            //    fileTemplate.Save();
               
            //}

            var template = _config["AppSettings:BaseDirectory"] ?? string.Empty;   
            var targetfile= Path.Combine(template, "filetemp.xlsx");

            using (var sourceFile = new ExcelPackage(new FileInfo(yardipath)))
            {
                using (var targetFile = new ExcelPackage(new FileInfo(targetfile))) { 
                
                        var sourceWorksheet = sourceFile.Workbook.Worksheets[0];
                        var endrow = sourceWorksheet.Dimension.End.Row;
                    //Source Range
                        var range = sourceFile.Workbook.Worksheets[0].Cells[$"A5:F{endrow}"];
                        var targetworksheet=targetFile.Workbook.Worksheets["Yardi TB"];

                    var targetstartrow = 1;
                    var targetstartcolumn = 1;
                    for (int i = 1; i < range.Rows; i++)
                    {
                            for(int j=1; j <= range.Columns; j++)
                            {
                            var soucecell = range[i,j];
                                int targetRow = targetstartrow + i - 1;
                                int targetCol = targetstartcolumn + j - 1;

                                var targetCell = targetworksheet.Cells[targetRow, targetCol];
                                targetCell.Value = soucecell.Value;
                            targetCell.Style.Font.Bold = soucecell.Style.Font.Bold;
                            }
                    }

              

                }



            }


            return "Yardi Data Imported";

        }

    }
    

}
