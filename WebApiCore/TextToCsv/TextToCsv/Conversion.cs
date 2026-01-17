using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;
namespace TextToCsv
{
    public class Conversion
    {
        public static void Main(string[] args)
        {

            string textPath = @"C:\Users\Nimap\Downloads\TxtToCsvSample.txt";
            string csvPath = @"C:\Users\Nimap\Downloads\Practice.csv";

            if (!Path.Exists(csvPath))
            {
                File.Create(csvPath); // other wise overide
            }

            var pageNumber = 0;
            bool Header = false;
            using (StreamReader sr = new StreamReader(textPath))
            {
                using (StreamWriter sw = new StreamWriter(csvPath))
                {
                    while (!sr.EndOfStream) {

                        var line = sr.ReadLine();
                        if (line == null)
                        {
                            return;
                        }

                        if (string.IsNullOrWhiteSpace(line))
                        {
                            continue;
                        }
                        if (line.StartsWith("Company") || line.StartsWith("Report") || line.StartsWith("Generated"))
                        {
                            continue;
                        }

                        if (line.Contains("Page"))
                        {
                            pageNumber = int.Parse(Regex.Match(line, "\\d+").Value);
                            continue;

                        }

                        if (line.StartsWith("EmployeeId"))
                        {
                            if (!Header)
                            {
                                sw.WriteLine("PageNo,EmployeeId,Name,Department,Salary");
                                Header = true;
                            }
                            continue;
                        }

                        
                        line=Regex.Replace(line.Trim(), @"\s{2,100}", ",");

                        if (pageNumber > 0) { 
                        
                            sw.WriteLine($"{pageNumber},{line}");
                        }
                        



                    
                    }


                }
            }
        }

    }
}
