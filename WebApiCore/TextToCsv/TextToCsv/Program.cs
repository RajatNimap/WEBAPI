//using System;
//using System.IO;
//using System.Text.RegularExpressions;

//class Program
//{
//    static void Main()
//    {
//        string txtPath = @"C:\Users\Nimap\Downloads\TxtToCsvSample.txt";
//        string csvPath = @"C:\Users\Nimap\Downloads\pra.csv";
       
//        int currentPage = 0;
//        bool headerWritten = false;

//        using (StreamReader reader = new StreamReader(txtPath))
//        using (StreamWriter writer = new StreamWriter(csvPath))
//        {
//            while (!reader.EndOfStream)
//            {
//                string line = reader.ReadLine();

//                if (string.IsNullOrWhiteSpace(line))
//                    continue;

//                // Detect page number
//                if (line.StartsWith("--- Page"))
//                {
//                    currentPage = int.Parse(
//                        Regex.Match(line, @"\d+").Value);
//                    continue;
//                }

//                // Skip report metadata
//                if (line.StartsWith("Company") ||
//                    line.StartsWith("Report") ||
//                    line.StartsWith("Generated"))
//                    continue;

//                // Handle header
//                if (line.StartsWith("EmployeeId"))
//                {
//                    if (!headerWritten)
//                    {
//                        writer.WriteLine("PageNumber,EmployeeId,Name,Department,Salary");
//                        headerWritten = true;
//                    }
//                    continue;
//                }

//                // Convert spaces to CSV
//                line = Regex.Replace(line.Trim(), @"\s+", ",");

//                // Write data with page number
//                writer.WriteLine($"{currentPage},{line}");
//            }
//        }

//        Console.WriteLine("CSV file created successfully.");
//    }
//}
