using System.Drawing;
using System.Reflection.Emit;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Components.Forms;
using OfficeOpenXml;

namespace LearningEPpluse.Services
{
    public class ExcelServices
    {
        private readonly IConfiguration _config;
        public ExcelServices(IConfiguration config)
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

                for (int i = 1; i <= 10; i++)
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

            //var template = _config["AppSettings:BaseDirectory"] ?? string.Empty;
            //var targetfile = Path.Combine(template, "filetemp.xlsx");

            //using (var sourceFile = new ExcelPackage(new FileInfo(yardipath)))
            //{
            //    using (var targetFile = new ExcelPackage(new FileInfo(targetfile)))
            //    {

            //        try
            //        {
            //            var CountofWorksheets = sourceFile.Workbook.Worksheets.Count;

            //            var targetStartRow = 5;

            //            for (int k = 0; k < CountofWorksheets; k++)
            //            {
            //                int startRow = (k == 0 ? 5 : 7);
            //                //Source Range
            //                var output = sourceFile.Workbook.Worksheets[k];
            //                if (output.Dimension == null) continue;
            //                var endrow = output.Dimension.End.Row;
            //                var range = output.Cells[$"A{startRow}:F{endrow - 1}"];

            //                var targetworksheet = targetFile.Workbook.Worksheets["Yardi TB"];

            //                for (int i = 1; i <= range.Rows; i++)
            //                {
            //                    for (int j = 1; j <= range.Columns; j++)
            //                    {
            //                        var src = output.Cells[range.Start.Row + i - 1, range.Start.Column + j - 1];
            //                        var tar = targetworksheet.Cells[i, j];
            //                        tar.Value = src.Value;

            //                    }
            //                }
            //                targetStartRow += range.Rows + 1;
            //                targetFile.Save();

            //            }
            //        }
            //        catch (Exception ex)
            //        {
            //            var error = ex.Message;
            //            Console.WriteLine(error);


            //        }



            //    }


            //    return "Yardi Data Imported";

            //}
            var template = _config["AppSettings:BaseDirectory"] ?? string.Empty;
            var targetfile = Path.Combine(template, "filetemp.xlsx");

            using (var sourceFile = new ExcelPackage(new FileInfo(yardipath)))
            using (var targetFile = new ExcelPackage(new FileInfo(targetfile)))
            {
                try
                {
                    // Ensure target worksheet exists
                    var targetWorksheet = targetFile.Workbook.Worksheets["Yardi TB"];
                    if (targetWorksheet == null)
                    {
                        targetWorksheet = targetFile.Workbook.Worksheets.Add("Yardi TB");
                    }

                    int targetStartRow = 1;

                    for (int k = 0; k < sourceFile.Workbook.Worksheets.Count; k++)
                    {
                       Console.WriteLine(sourceFile.Workbook.Worksheets[k]);
                        var sourceWorksheet = sourceFile.Workbook.Worksheets[k];

                        if (k == 1)
                        {
                            Console.WriteLine("Hello");
                        }

                        string entity = sourceWorksheet.Cells["A1"].Text;
                        if (sourceWorksheet.Dimension == null) continue;

                        int startRow = (k == 0 ? 5 : 7);
                        int endRow = sourceWorksheet.Dimension.End.Row;
                        int endCol = sourceWorksheet.Dimension.End.Column;

                        var checkcell = sourceWorksheet.Cells[7, 1].Text;
                        if (checkcell.Contains("No activity for given filter criteria") ) 
                        {
                            continue;
                        }
                        var range = sourceWorksheet.Cells[$"A{startRow}:F{endRow-1}"];
                        var entityword = WordSplit(entity);
                        int startentity = (targetStartRow == 1 ? 3 : targetStartRow);
                        var rangeofentity = targetWorksheet.Cells[$"H{startentity}:H{startentity + (endRow - 8)}"];
                        var rangeofentity1 = targetWorksheet.Cells[$"J{startentity}:J{startentity + (endRow - 8)}"];

                        rangeofentity.Value = entityword;
                        rangeofentity1.Value = entityword; 
                        for (int i = 1; i <= range.Rows; i++)
                        {
                            for (int j = 1; j <= range.Columns; j++)
                            {
                                var targetCell = targetWorksheet.Cells[targetStartRow + i - 1, j];
                                var sourceCell= sourceWorksheet.Cells[range.Start.Row + i - 1, range.Start.Column + j - 1];


                                targetCell.Value = sourceCell.Value;
                                
                            }
                        }
                        targetFile.Save();
                       // Console.WriteLine(targetStartRow);
                        //Console.WriteLine(range.Rows);
                         targetStartRow += range.Rows; // Move to next row block in target
                         Console.WriteLine(targetStartRow);
                    }

                    // targetFile.Save(); // Save once after all copying

                    targetWorksheet.Cells["I1"].Value = "Ent - Category";
                    targetWorksheet.Cells["J1"].Value = "Entity";
                    targetWorksheet.Cells["K1"].Value = "GL Code";
                    targetWorksheet.Cells["L1"].Value = "Entity-GLCode";
                    targetWorksheet.Cells["M1"].Value = "GL Description";

                   // targetWorksheet.Cells[$"K3:K{}"] 

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            return "Yardi Data Imported";


        }

        public static string WordSplit(string entity)
        {
            var match = Regex.Match(entity, @"\(([^)]*)\)");
            string insideBrackets = match.Success ? match.Groups[1].Value : "";
            return insideBrackets.ToUpper();
        }


    }
}
