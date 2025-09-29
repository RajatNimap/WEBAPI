using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using OfficeOpenXml;

namespace FFCCorp.Services
{
    public class ReportService : IReportService
    {
        public async Task<string> BalanceSheetReport(string baseDirectory, string yardi, string loc, string bsdetail, string interestEidl, string interco)
        {
            var targetpath = Path.Combine(baseDirectory, "filetemp.xlsx");
            using (var targetFile = new ExcelPackage(new FileInfo(targetpath)))
            {
                using (var sourceFile = new ExcelPackage(new FileInfo(yardi)))
                {
                    try
                    {
                        var targetWorksheet = targetFile.Workbook.Worksheets["Yardi TB"];

                        if (targetWorksheet == null)
                        {
                            targetWorksheet = targetFile.Workbook.Worksheets.Add("Yardi TB");
                        }
                        targetWorksheet.Cells.Clear();
                        
                        int targetStartRow = 3;

                        for (int k = 0; k < sourceFile.Workbook.Worksheets.Count; k++)
                        {
                           // Console.WriteLine(sourceFile.Workbook.Worksheets[k]);
                            
                            var sourceWorksheet = sourceFile.Workbook.Worksheets[k];
                            string entity = sourceWorksheet.Cells["A1"].Text;
                            if (sourceWorksheet.Dimension == null) continue;

                            //int startRow = (k == 0 ? 5 : 7);
                            int endRow = sourceWorksheet.Dimension.End.Row;
                            int endCol = sourceWorksheet.Dimension.End.Column;

                            var checkcell = sourceWorksheet.Cells[7, 1].Text;
                            if (checkcell.Contains("No activity for given filter criteria"))
                            {
                                continue;
                            }
                            var range = sourceWorksheet.Cells[$"A7:F{endRow - 1}"];
                            var entityword = WordSplit(entity);
                            var rangeofentity = targetWorksheet.Cells[$"H{targetStartRow}:H{targetStartRow+range.Rows}"];
                           //var rangeofentity = targetWorksheet.Cells[$"H{targetStartRow}:H{targetStartRow + (endRow - 8)}"];

                            rangeofentity.Value = entityword;

                            //var rangeofentity1 = targetWorksheet.Cells[$"J{startentity}:J{startentity + (endRow - 8)}"];
                            //rangeofentity1.Value = entityword; 
                            //for (int i = 1; i <= range.Rows; i++)
                            //{
                            //    for (int j = 1; j <= range.Columns; j++)
                            //    {
                            //        var targetCell = targetWorksheet.Cells[targetStartRow + i - 1, j];
                            //        var sourceCell = sourceWorksheet.Cells[range.Start.Row + i - 1, range.Start.Column + j - 1];
                            //        targetCell.Value = sourceCell.Value;
                            //    }
                            //}
                            sourceWorksheet.Cells[5, 1, 6, endCol].Copy(targetWorksheet.Cells[1, 1]);
                            sourceWorksheet.Cells[range.Start.Row, range.Start.Column, range.End.Row,range.End.Column].Copy(targetWorksheet.Cells[targetStartRow, 1]);
                            targetWorksheet.Columns.AutoFit();       // adjust column width
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
                        var lastRow = targetWorksheet.Dimension.End.Row;
                        var entityrange = targetWorksheet.Cells[$"J3:J{lastRow}"].Formula = "H3";
                        var Glcoderange = targetWorksheet.Cells[$"K3:K{lastRow}"].Formula = "A3";
                        var entityglcode = targetWorksheet.Cells[$"L3:L{lastRow}"].Formula = "=+CONCATENATE(J3,\"-\",K3)";
                        var description = targetWorksheet.Cells[$"M3:M{lastRow}"].Formula = "B3";
                        targetFile.Save();

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }

                }

                using (var sourceFile = new ExcelPackage(new FileInfo(loc)))
                // using (var targetFile = new ExcelPackage(new FileInfo(targetfile)))
                {
                    try
                    {
                        var sourceWorksheet = sourceFile.Workbook.Worksheets[0];
                        var targetWorksheet = targetFile.Workbook.Worksheets["LOC"];

                        if (targetWorksheet == null)
                        {
                            targetWorksheet = targetFile.Workbook.Worksheets.Add("LOC");
                        }
                        targetWorksheet.Cells.Clear();

                        var Endcol = sourceWorksheet.Dimension.Columns;
                        int targeRow = 1;
                        sourceWorksheet.Cells[1, 1, 1, Endcol].Copy(targetWorksheet.Cells[targeRow, 1]);
                        targeRow++;

                        var LastRow = sourceWorksheet.Dimension.End.Row;
                        var FilteredRow = Enumerable.Range(2, LastRow - 1)
                            .Where(r => sourceWorksheet.Cells[r, 7].Text.Trim() == "Lines of Credit")
                            .ToList();

                        foreach (var rownum in FilteredRow)
                        {
                            sourceWorksheet.Cells[rownum, 1, rownum, Endcol].Copy(targetWorksheet.Cells[targeRow, 1]);
                            targeRow++;

                        }

                        targetWorksheet.Columns.AutoFit();       // adjust column width
                        targetFile.Save();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }

                }

                using (var sourceFile = new ExcelPackage(new FileInfo(bsdetail)))
                //using (var targetFile = new ExcelPackage(new FileInfo(targetfile)))
                {
                    try
                    {
                        var sourceWorksheet = sourceFile.Workbook.Worksheets[0];
                        var targetWorksheet = targetFile.Workbook.Worksheets["BS-Detail"];

                        if (targetWorksheet == null)
                        {
                            targetWorksheet = targetFile.Workbook.Worksheets.Add("BS-Detail");
                        }
                        targetWorksheet.Cells.Clear();

                        var Endcol = sourceWorksheet.Dimension.Columns;

                        //Header copy 
                        sourceWorksheet.Cells[38, 1, 38, 8].Copy(targetWorksheet.Cells[1, 4]);
                        //Data copy
                        var lastRow = sourceWorksheet.Dimension.End.Row;
                        if (sourceWorksheet.Dimension != null)
                        {
                            sourceWorksheet.Cells[42, 1, lastRow, 8]
                                .Copy(targetWorksheet.Cells[2, 4]);
                        }
                        //targetFile.Save();

                        targetWorksheet.Cells["A1"].Value = "Ent-Category";
                        targetWorksheet.Cells["B1"].Value = "Entity";
                        targetWorksheet.Cells["C1"].Value = "GL Code";
                        targetWorksheet.Cells["J1"].Value = "Month";

                        targetWorksheet.Cells[$"C2:C{lastRow}"].Formula = $"=RIGHT(D2,10)";
                        targetWorksheet.Cells[$"B2:B{lastRow}"].Formula = $"=LEFT(D2,3)";
                        targetWorksheet.Cells[$"A2:A{lastRow}"].Formula = $"=B2&\" - \"&TRIM(H2)";

                        targetWorksheet.Columns.AutoFit();       // adjust column width

                        targetFile.Save();
                    }
                    catch (Exception ex)
                    {

                        Console.WriteLine(ex.Message);
                    }

                }

                using (var sourceFile = new ExcelPackage(new FileInfo(interestEidl)))
                {
                    Console.WriteLine(File.Exists(interestEidl));
                    Console.WriteLine("Hello world");
                    try
                    {
                        var sourceWorksheet = sourceFile.Workbook.Worksheets[0];
                        var targetWorksheet = targetFile.Workbook.Worksheets["Interest - EIDL"];

                        if (targetWorksheet == null)
                        {
                            targetWorksheet = targetFile.Workbook.Worksheets.Add("Interest - EIDL");
                        }
                        targetWorksheet.Cells.Clear();
                        var row = sourceWorksheet.Dimension.Rows;
                        var col = sourceWorksheet.Dimension.Columns;
                        //copy header
                        sourceWorksheet.Cells[6, 1, 6, col].Copy(targetWorksheet.Cells[1, 1]);
                        //copy actual data
                        sourceWorksheet.Cells[8, 1, row - 3, col].Copy(targetWorksheet.Cells[2, 1]);

                        targetWorksheet.Columns.AutoFit();       // adjust column width
                        Console.WriteLine($"{col} {row}");
                        targetFile.Save();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);

                    }

                }

                using (var sourceFile = new ExcelPackage(new FileInfo(bsdetail)))
                {
                    try
                    {
                        var sourceWorksheet = sourceFile.Workbook.Worksheets[0];
                        var targetWorksheet = targetFile.Workbook.Worksheets["Interco"];

                        if (targetWorksheet == null)
                        {
                            targetWorksheet = targetFile.Workbook.Worksheets.Add("Interco");
                        }
                        targetWorksheet.Cells.Clear();

                        int row = sourceWorksheet.Dimension.Rows;
                        int col = sourceWorksheet.Dimension.Columns;

                          sourceWorksheet.Cells[38, 1, 38, col].Copy(targetWorksheet.Cells[1, 1]);
                          sourceWorksheet.Cells["J1"].Value = "Month";

                        var FiterInterco = Enumerable.Range(42, row-41)
                            .Where(x => !string.IsNullOrEmpty(sourceWorksheet.Cells[x,5].Text) && sourceWorksheet.Cells[x, 5].Text
                            .StartsWith("interco",StringComparison.OrdinalIgnoreCase)).ToList();

                        int targeRow = 2;
                        foreach (var rowdata in FiterInterco)
                        {
                            sourceWorksheet.Cells[rowdata, 1, rowdata, col].Copy(targetWorksheet.Cells[targeRow, 1]);
                            targeRow++;
                        }
                        targetWorksheet.Columns.AutoFit();       // adjust column width
                        targetFile.Save();
                        Console.WriteLine(FiterInterco);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }

                }

                return "Hello World";
            }
        }
        public static string WordSplit(string entity)
        {
            var match = Regex.Match(entity, @"\(([^)]*)\)");
            string insideBrackets = match.Success ? match.Groups[1].Value : "";
            return insideBrackets.ToUpper();
        }
    }
}
