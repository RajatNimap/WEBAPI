
using System;
using System.Diagnostics;
using System.IO;

class Program
{
    // Configuration - Replace with your values or fetch from config
    static string SERVER_IP = "95.111.230.3";
    static string SHARE_NAME = "BatFolder";
    static string USERNAME = "administrator";
    static string PASSWORD = "N1m@p2025$Server";
    static string DRIVE_LETTER = "X:";

     static void Main()
    {
        while (true)
        {
            Console.Write("Enter the file name on the server (e.g., C# Notes.txt): ");
            string fileName = Console.ReadLine();

            Console.Write("Enter destination folder (e.g., C:\\DemoFiles): ");
            string destFolder = Console.ReadLine();

            if (!Directory.Exists(destFolder))
            {
                Console.WriteLine($"ERROR: Destination folder does not exist: {destFolder}");
                Console.ReadKey();
                continue;
            }

            // Combine folder + filename to get final local path
            string localDestPath = Path.Combine(destFolder, Path.GetFileName(fileName));



            RunCommand($"net use {DRIVE_LETTER} /delete", false); // Clean up existing

            Console.WriteLine("Mounting shared folder...");
            var connectCmd = $"net use {DRIVE_LETTER} \\\\{SERVER_IP}\\{SHARE_NAME} /user:{USERNAME} {PASSWORD} /persistent:no";
            if (!RunCommand(connectCmd))
            {
                Console.WriteLine("ERROR: Failed to connect to shared folder.");
                Console.ReadKey();
                continue;
            }

            string serverFilePath = Path.Combine(DRIVE_LETTER, fileName);

            if (!File.Exists(serverFilePath))
            {
                Console.WriteLine($"ERROR: File does not exist on server: {fileName}");
                RunCommand($"net use {DRIVE_LETTER} /delete", false);
                Console.ReadKey();
                continue;
            }

            Console.WriteLine("Copying file from server...");
            try
            {
                File.Copy(serverFilePath, localDestPath, true);
                Console.WriteLine($"File successfully copied to: {localDestPath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR: File copy failed. {ex.Message}");
            }

            RunCommand($"net use {DRIVE_LETTER} /delete", false);

            Console.Write("Do you want to continue? Type q to quit, any other key to continue: ");
            var input = Console.ReadLine();
            if (input?.ToLower() == "q")
                break;
        }
    }

    static bool RunCommand(string command, bool showOutput = true)
    {
        try
        {
            ProcessStartInfo procInfo = new ProcessStartInfo("cmd.exe", "/c " + command)
            {
                RedirectStandardOutput = !showOutput,
                RedirectStandardError = !showOutput,
                UseShellExecute = false,
                CreateNoWindow = !showOutput
            };

            using (Process proc = Process.Start(procInfo))
            {
                proc.WaitForExit();
                return proc.ExitCode == 0;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Command execution failed: " + ex.Message);
            return false;
        }
    }
}
