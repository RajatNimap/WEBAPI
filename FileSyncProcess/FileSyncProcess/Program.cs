

using System.Text.Json;
using System.Data;
using System.Net.NetworkInformation;
using System.Text;
using System.Diagnostics;
using System.Net.Sockets;
using IniParser;
using IniParser.Model;
using System.Collections.Concurrent;

namespace RobustAccessDbSync
{
    class Program
    {
        static string DRIVE_LETTER = "X:";
        static bool _syncRunning = true;
        static bool _isOnline = true;
        static DateTime _lastOnlineTime = DateTime.MinValue;
        static int _syncCycleWaitMinutes = 1;
        static Stopwatch _cycleTimer = new Stopwatch();
        static DateTime _nextSyncTime = DateTime.Now;
        static string clientPath;
        static string serverPath;
        static string SERVER_IP;
        static string SHARE_NAME;
        static string USERNAME;
        static string PASSWORD;
        static string? rememberedClientPath = null;
        static List<string> Client_Folders = [];
        static string syncMetaFile = "sync_metadata.json";
        static DateTime _lastSyncTime = DateTime.MinValue;
        static int Count = 0;

        // Enhanced metadata tracking
        class SyncMetadata
        {
            public DateTime LastSyncTime { get; set; } = DateTime.MinValue;
            public Dictionary<string, FileMetadata> Files { get; set; } = new Dictionary<string, FileMetadata>();
        }

        class FileMetadata
        {
            public DateTime LastModified { get; set; }
            public long FileSize { get; set; }
            public string FilePath { get; set; } = string.Empty;
        }

        static SyncMetadata _syncMetadata = new SyncMetadata();

        // Parallel processing settings
        static int _maxDegreeOfParallelism = Environment.ProcessorCount;
        static int _batchSize = 1000;

        static void GetServerCredentials()
        {
            while (true)
            {
                do
                {
                    Console.Write("Enter USERNAME: ");
                    USERNAME = Console.ReadLine();
                    if (string.IsNullOrWhiteSpace(USERNAME))
                        Console.WriteLine("USERNAME cannot be empty.");
                } while (string.IsNullOrWhiteSpace(USERNAME));

                do
                {
                    Console.Write("Enter PASSWORD: ");
                    PASSWORD = ReadPassword();
                    if (string.IsNullOrWhiteSpace(PASSWORD))
                        Console.WriteLine("PASSWORD cannot be empty.");
                } while (string.IsNullOrWhiteSpace(PASSWORD));

                Console.WriteLine("\nPress Enter to continue or type 'r' to re-enter:");

                string input = Console.ReadLine()?.Trim().ToLower();

                if (string.IsNullOrEmpty(input))
                {
                    break;
                }
                else if (input == "r")
                    continue;
                else
                    Console.WriteLine("Invalid input. Re-entering...\n");
            }
        }

        static void GetClientsServerPath()
        {
            Console.Title = "File Synchronization Tool";
            Console.CursorVisible = false;
            PrintHeader();
            ShowGameStyleLoader("Initializing File Synchronization Tool", 20);

            while (true)
            {
                do
                {
                    Console.Write("Enter Client Root Path(For Config/Server to Client Copy): ");
                    clientPath = Console.ReadLine();

                    if (!Directory.Exists(clientPath) || string.IsNullOrWhiteSpace(clientPath))
                    {
                        Console.WriteLine($"Path should be valid and cannot be empty");
                        clientPath = string.Empty;
                    }
                } while (string.IsNullOrWhiteSpace(clientPath));

                do
                {
                    Console.Write("Enter Server Path: ");
                    serverPath = Console.ReadLine();
                    if (string.IsNullOrWhiteSpace(serverPath))
                        Console.WriteLine("Server path cannot be empty");
                } while (string.IsNullOrWhiteSpace(serverPath));

                var serverParts = serverPath.Split(new[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);
                if (serverParts.Length < 2)
                {
                    PrintError("Invalid server path format. Expected format: \\\\server\\share\\path");
                }

                SERVER_IP = serverParts[0];
                SHARE_NAME = serverParts[1];

                Console.WriteLine("\nPress Enter to continue or type 'r' to re-enter:");

                string input = Console.ReadLine()?.Trim().ToLower();

                if (string.IsNullOrEmpty(input))
                {
                    break;
                }
                else if (input == "r")
                    continue;
                else
                    Console.WriteLine("Invalid input. Re-entering...\n");
            }
        }

        //static void GetClientPathCredentials()
        //{
        //    Client_Folders.Clear();
        //    int i = 1;
        //    while (true)
        //    {
        //        Console.Write($"Enter client folder path #{i} (leave blank to stop): ");
        //        string path = Console.ReadLine();
        //        if (string.IsNullOrWhiteSpace(path))
        //            break;

        //        if (Directory.Exists(path))
        //        {
        //            Client_Folders.Add(path);
        //            i++;
        //        }
        //        else
        //        {
        //            Console.WriteLine("Invalid folder. Please try again.");
        //        }
        //    }

        //    if (Client_Folders.Count == 0)
        //        Console.WriteLine("Warning: No folders entered for sync.");
        //}
        static void GetClientPathCredentials()
        {
            Client_Folders.Clear();
            int i = 1;

            while (true)
            {
                Console.Write($"Enter client folder path #{i} (at least one required): ");
                string path = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(path))
                {
                    if (Client_Folders.Count == 0)
                    {
                        Console.WriteLine("You must enter at least one folder!");
                        continue; // force user to enter at least one
                    }
                    else
                    {
                        break; // user can stop after entering at least one
                    }
                }

                if (Directory.Exists(path))
                {
                    Client_Folders.Add(path);
                    i++;
                }
                else
                {
                    Console.WriteLine("Invalid folder. Please try again.");
                }
            }
        }




        // Fast directory enumeration with progress
        static IEnumerable<string> EnumerateFilesFast(string path, string searchPattern = "*")
        {
            var files = new ConcurrentBag<string>();
            var directories = new ConcurrentStack<string>();
            directories.Push(path);

            while (directories.TryPop(out var currentDir))
            {
                try
                {
                    Parallel.ForEach(Directory.EnumerateFiles(currentDir, searchPattern), file =>
                    {
                        files.Add(file);
                    });

                    Parallel.ForEach(Directory.EnumerateDirectories(currentDir), dir =>
                    {
                        directories.Push(dir);
                    });
                }
                catch (UnauthorizedAccessException) { }
                catch (DirectoryNotFoundException) { }
            }

            return files;
        }

        // Load sync metadata from file
        static void LoadSyncMetadata()
        {
            try
            {
                string metadataPath = Path.Combine(clientPath, syncMetaFile);
                if (File.Exists(metadataPath))
                {
                    string json = File.ReadAllText(metadataPath);
                    _syncMetadata = JsonSerializer.Deserialize<SyncMetadata>(json) ?? new SyncMetadata();
                    _lastSyncTime = _syncMetadata.LastSyncTime;
                    PrintSuccess($"Loaded sync metadata. Last sync: {_lastSyncTime:yyyy-MM-dd HH:mm:ss}");
                    PrintInfo($"Tracked files: {_syncMetadata.Files.Count:N0}");
                }
                else
                {
                    PrintInfo("No previous sync metadata found. Starting fresh sync.");
                }
            }
            catch (Exception ex)
            {
                PrintError($"Error loading sync metadata: {ex.Message}");
                _syncMetadata = new SyncMetadata();
            }
        }

        // Save sync metadata to file
        static void SaveSyncMetadata()
        {
            try
            {
                _syncMetadata.LastSyncTime = DateTime.UtcNow;
                string metadataPath = Path.Combine(clientPath, syncMetaFile);
                string json = JsonSerializer.Serialize(_syncMetadata, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(metadataPath, json);
            }
            catch (Exception ex)
            {
                PrintError($"Error saving sync metadata: {ex.Message}");
            }
        }

        [System.Runtime.Versioning.SupportedOSPlatform("windows")]
        static async Task Main()
        {
            var parser = new FileIniDataParser();
            IniData data = null;
            const string pointerFile = "last_path.txt";

            try
            {
                if (File.Exists(pointerFile))
                {
                    rememberedClientPath = File.ReadAllText(pointerFile)?.Trim();
                }

                if (!string.IsNullOrEmpty(rememberedClientPath))
                {
                    string iniPath = Path.Combine(rememberedClientPath, "config.ini");
                    if (File.Exists(iniPath))
                    {
                        data = parser.ReadFile(iniPath);
                        USERNAME = data["Credentials"]["Username"];
                        PASSWORD = data["Credentials"]["Password"];
                        SERVER_IP = data["Server"]["IP"];
                        SHARE_NAME = data["Server"]["Share"];
                        serverPath = data["Server"]["Path"];
                        clientPath = data["Client"]["Path"];

                        int index = 1;
                        while (true)
                        {
                            string key = $"Path{index}";
                            if (data["folder"].ContainsKey(key))
                            {
                                Client_Folders.Add(data["folder"][key]);
                                index++;
                            }
                            else break;
                        }

                        Console.WriteLine("Loaded saved configuration.");
                    }
                }

                if (data == null)
                {
                    Console.WriteLine("No saved configuration found. Please enter details.");
                    GetClientsServerPath();
                    GetServerCredentials();
                    GetClientPathCredentials();

                    data = new IniData();
                    data.Sections.AddSection("folder");

                    data["Credentials"]["Username"] = USERNAME;
                    data["Credentials"]["Password"] = PASSWORD;
                    data["Server"]["IP"] = SERVER_IP;
                    data["Server"]["Share"] = SHARE_NAME;
                    data["Server"]["Path"] = serverPath;
                    data["Client"]["Path"] = clientPath;

                    for (int i = 0; i < Client_Folders.Count; i++)
                    {
                        data["folder"][$"Path{i + 1}"] = Client_Folders[i];
                    }

                    string iniPath = Path.Combine(clientPath, "config.ini");
                    parser.WriteFile(iniPath, data);
                }


                // Load sync metadata AFTER clientPath is determined
              

                LoadSyncMetadata();


                Console.WriteLine("Ready to sync using loaded configuration.");
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(" ERROR: " + ex.Message);
                Console.ResetColor();
            }

            File.WriteAllText(pointerFile, clientPath);

            PrintSuccess("\nStarting file synchronization...");
            PrintInfo("Press 'S' to stop, 'R' to restart, 'Q' to quit.\n");

            var syncTask = Task.Run(() => ContinuousFileSync());

            while (true)
            {
                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(true).Key;

                    if (key == ConsoleKey.Q)
                    {
                        if (_syncRunning)
                        {
                            _syncRunning = false;
                            PrintWarning("Stopping synchronization...");
                            await syncTask;
                        }
                        break;
                    }
                    else if (key == ConsoleKey.S)
                    {
                        if (_syncRunning)
                        {
                            _syncRunning = false;
                            PrintWarning("Stopping synchronization...");
                            await syncTask;
                            PrintInfo("Synchronization stopped. Press 'R' to restart or 'Q' to quit.");
                        }
                    }
                    else if (key == ConsoleKey.R)
                    {
                        if (_syncRunning)
                        {
                            _syncRunning = false;
                            PrintWarning("Restarting synchronization...");
                            await syncTask;
                        }
                        else
                        {
                            PrintWarning("Starting synchronization...");
                        }

                        _syncRunning = true;
                        syncTask = Task.Run(() => ContinuousFileSync());
                    }
                }

                await Task.Delay(100);
            }

            PrintInfo("\nExited. Press any key to close.");
            Console.CursorVisible = true;
            Console.ReadKey();
        }

       
        static string ReadPassword()
        {
            string password = "";
            ConsoleKeyInfo key;

            do
            {
                key = Console.ReadKey(true);
                if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter)
                {
                    password += key.KeyChar;
                    Console.Write("*");
                }
                else if (key.Key == ConsoleKey.Backspace && password.Length > 0)
                {
                    password = password.Substring(0, (password.Length - 1));
                    Console.Write("\b \b");
                }
            } while (key.Key != ConsoleKey.Enter);

            Console.WriteLine();
            return password;
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
                PrintError("Command execution failed: " + ex.Message);
                return false;
            }
        }

        // Optimized file sync with parallel processing
        static void SyncFiles(string sourceFolder, string targetFolder, string logFile, string direction, List<string>? excludeFolders, bool isFullServerToClient)
        {
            if (!Directory.Exists(sourceFolder))
                return;

            string today = DateTime.Today.ToString("yyyy-MM-dd");
            string logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, logFile);

            PrintInfo($"Scanning {Path.GetFileName(sourceFolder)} for changes...");

            // Fast parallel file enumeration
            var allFiles = EnumerateFilesFast(sourceFolder).ToArray();
            PrintInfo($"Found {allFiles.Length:N0} files total");

            // Parallel processing to find changed files
            var changedFiles = new ConcurrentBag<string>();
            var newMetadata = new ConcurrentDictionary<string, FileMetadata>();

            Parallel.ForEach(allFiles, new ParallelOptions { MaxDegreeOfParallelism = _maxDegreeOfParallelism }, file =>
            {
                try
                {
                    var fileInfo = new FileInfo(file); //need datetime of file
                    string relativePath = Path.GetRelativePath(sourceFolder, file);

                    // Check if file should be excluded
                    if (isFullServerToClient && excludeFolders != null) //any
                    {
                        string topLevel = relativePath.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)[0];
                        if (excludeFolders.Contains(topLevel, StringComparer.OrdinalIgnoreCase))
                            return;
                    }

                    var currentMetadata = new FileMetadata
                    {
                        LastModified = fileInfo.LastWriteTimeUtc,
                        FileSize = fileInfo.Length,
                        FilePath = relativePath
                    };

                    newMetadata[relativePath] = currentMetadata;

                    // Check if file has changed
                    bool hasChanged = true;
                    if (_syncMetadata.Files.TryGetValue(relativePath, out var oldMetadata))
                    {
                        hasChanged = currentMetadata.LastModified > oldMetadata.LastModified ||
                                    currentMetadata.FileSize != oldMetadata.FileSize;
                    }

                    if (hasChanged)
                    {
                        changedFiles.Add(file);
                    }
                }
                catch (Exception ex)
                {
                    PrintError($"Error processing file {file}: {ex.Message}");
                }
            });

            PrintInfo($"Found {changedFiles.Count:N0} changed file(s)");

            if (changedFiles.Count == 0)
            {
                // Update metadata even if no changes (for deleted files tracking)
                _syncMetadata.Files = new Dictionary<string, FileMetadata>(newMetadata);
                return;
            }

            // Process changed files in batches
            int copiedFiles = 0;
            var changedFilesArray = changedFiles.ToArray();
            var total = changedFiles.Count();

            for (int i = 0; i < changedFilesArray.Length; i += _batchSize)
            {
                var batch = changedFilesArray.Skip(i).Take(_batchSize).ToArray();

                Parallel.ForEach(batch, new ParallelOptions { MaxDegreeOfParallelism = _maxDegreeOfParallelism }, src =>
                {
                    try
                    {
                        string relativePath = Path.GetRelativePath(sourceFolder, src);
                        string dest = Path.Combine(targetFolder, relativePath);

                        Directory.CreateDirectory(Path.GetDirectoryName(dest));
                        File.Copy(src, dest, true);
                        Count++;

                        Interlocked.Increment(ref copiedFiles);
                        PrintSuccess($"  [✓] Copied: {relativePath} {direction}");
                        Console.WriteLine($"File Copied {Count} out of {total}");

                    }
                    catch (Exception ex)
                    {
                        PrintError($"Error copying file {src}: {ex.Message}");
                    }
                });
            }

            // Update metadata
            _syncMetadata.Files = new Dictionary<string, FileMetadata>(newMetadata);
            //Console.WriteLine($"")
            // Log results
            if (copiedFiles > 0)
            {
                string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                var logLines = new List<string>
                {
                    $"[{timestamp}]",
                    $"Sync = Files",
                    $"changes = {copiedFiles}",
                    $"direction = {direction}",
                    $"total_files = {allFiles.Length}",
                    ""
                };
                File.AppendAllLines(logPath, logLines);
            }
        }

        static void SyncFilesBothDirections()
        {
            try
            {
                if (_syncRunning)
                {
                    string logFilePath = Path.Combine(clientPath, "Configlog.ini");
                    string serverFolder = serverPath;
                    var excludeList = new List<string>();

                    PrintInfo($"Starting optimized sync (since {_lastSyncTime:yyyy-MM-dd HH:mm:ss})");

                    foreach (var clientFolder in Client_Folders)
                    {
                        if (!Directory.Exists(clientFolder)) continue;

                        string clientFolderName = Path.GetFileName(clientFolder);
                        excludeList.Add(clientFolderName);

                        string correspondingServerFolder = Path.Combine(serverFolder, clientFolderName);

                        if (!Directory.Exists(correspondingServerFolder))
                        {
                            Directory.CreateDirectory(correspondingServerFolder);
                            PrintSuccess($"[+] Created folder on server: {correspondingServerFolder}");
                        }

                        SyncFiles(clientFolder, correspondingServerFolder, logFilePath, "ClientToServer", excludeList, false);
                        SyncFiles(correspondingServerFolder, clientFolder, logFilePath, "ServerToClient", excludeList, false);
                    }

                    // Save metadata after successful sync
                    SaveSyncMetadata();
                }
            }
            catch (Exception ex)
            {
                PrintError($"[!] Error during sync: {ex.Message}");
            }
        }

        static async Task ContinuousFileSync()
        {
            while (_syncRunning)
            {
                try
                {
                    _cycleTimer.Restart();
                    PrintInfo($"Starting sync cycle at {DateTime.Now:T}");

                    _isOnline = CheckNetworkConnection(SERVER_IP);
                    if (_isOnline)
                    {
                        if (_lastOnlineTime == DateTime.MinValue)
                        {
                            PrintSuccess("Connection restored");
                        }
                        _lastOnlineTime = DateTime.Now;

                        RunCommand($"net use {DRIVE_LETTER} /delete", false);

                        string connectCmd = $"net use {DRIVE_LETTER} \\\\{SERVER_IP}\\{SHARE_NAME} /user:{USERNAME} {PASSWORD} /persistent:no";
                        if (RunCommand(connectCmd))
                        {
                            SyncFilesBothDirections();
                            RunCommand($"net use {DRIVE_LETTER} /delete", false);
                        }
                        else
                        {
                            PrintError("Failed to connect to shared folder");
                        }

                        _cycleTimer.Stop();
                        PrintSuccess($"Sync cycle completed in {_cycleTimer.Elapsed.TotalSeconds:0.00} seconds");
                    }
                    else
                    {
                        PrintWarning("Connection lost - entering offline mode");

                        if (_lastOnlineTime != DateTime.MinValue)
                        {
                            PrintWarning("Connection lost - entering offline mode");
                            _lastOnlineTime = DateTime.MinValue;
                        }
                    }

                    _nextSyncTime = DateTime.Now.AddMinutes(_syncCycleWaitMinutes);
                    PrintInfo($"Next sync at {_nextSyncTime:T}");

                    while (DateTime.Now < _nextSyncTime && _syncRunning)
                    {
                        TimeSpan remaining = _nextSyncTime - DateTime.Now;
                        Console.Write($"\rWaiting for next sync in {remaining.Minutes}:{remaining.Seconds:00}...");
                        await Task.Delay(1000);
                    }

                    Console.WriteLine();
                }
                catch (Exception ex)
                {
                    PrintError($"Sync cycle error: {ex.Message}");
                }
            }
        }

        static bool CheckNetworkConnection(string ip)
        {
            try
            {
                using (var tcpClient = new TcpClient())
                {
                    var result = tcpClient.BeginConnect(ip, 445, null, null);
                    var success = result.AsyncWaitHandle.WaitOne(2000);
                    if (tcpClient.Connected) tcpClient.EndConnect(result);
                    return success;
                }
            }
            catch
            {
                return false;
            }
        }

        static void PrintHeader()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\nFile Synchronization Tool");
            Console.WriteLine("-------------------------");
            Console.ResetColor();
        }

        static void PrintInfo(string message)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"[{DateTime.Now:T}] {message}");
            Console.ResetColor();
        }

        static void PrintSuccess(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"[{DateTime.Now:T}] {message}");
            Console.ResetColor();
        }

        static void PrintWarning(string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"[{DateTime.Now:T}] {message}");
            Console.ResetColor();
        }

        static void PrintError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[{DateTime.Now:T}] {message}");
            Console.ResetColor();
        }

        static void ShowGameStyleLoader(string message, int totalSteps)
        {
            Console.Write(message + " ");
            int progressBarWidth = 30;

            for (int i = 0; i <= totalSteps; i++)
            {
                double percentage = (double)i / totalSteps;
                int filledBars = (int)(percentage * progressBarWidth);
                string bar = new string('█', filledBars).PadRight(progressBarWidth, '-');

                Console.Write($"\r{message} [{bar}] {percentage * 100:0}%");
                Thread.Sleep(20);
            }
            Console.WriteLine();
        }
    }
}