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
        static string rememberedClientPath = string.Empty;
        static List<string> Client_Folders = [];
        static string syncMetaFile = "sync_metadata.json";
        static DateTime _lastSyncTime = DateTime.MinValue;
        static int Count = 0;

        #region Models

        // Enhanced metadata tracking
        class SyncMetadata
        {
            public DateTime LastSyncTime { get; set; } = DateTime.MinValue;
            public Dictionary<string, FolderMetadata> Folders { get; set; } = new Dictionary<string, FolderMetadata>();
        }

        class FolderMetadata
        {
            public string FolderPath { get; set; } = string.Empty;
            public Dictionary<string, FileMetadata> Files { get; set; } = new Dictionary<string, FileMetadata>();
            public DateTime LastScanTime { get; set; } = DateTime.MinValue;

        }

        class FileMetadata
        {
            public DateTime LastModified { get; set; }
            public long FileSize { get; set; }
            public string FilePath { get; set; } = string.Empty;
            public DateTime LastSyncTime { get; set; } = DateTime.UtcNow;

        }

        static SyncMetadata _syncMetadata = new SyncMetadata();

        #endregion

        // Parallel processing settings
        static int _maxDegreeOfParallelism = Environment.ProcessorCount;
        static int _batchSize = 1000;

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
                    string iniPath = Path.Combine(rememberedClientPath, "Config.ini");

                    if (File.Exists(iniPath))
                    {
                        data = parser.ReadFile(iniPath);

                        if (!data.Sections.ContainsSection("Credentials"))
                        {
                            GetServerCredentials();
                            data["Credentials"]["Username"] = USERNAME;
                            data["Credentials"]["Password"] = PASSWORD;
                            parser.WriteFile(iniPath, data);

                        }
                        if (!data.Sections.ContainsSection("Server") || !data.Sections.ContainsSection("Client"))
                        {
                            GetClientsServerPath();
                            data["Server"]["Path"] = serverPath;
                            data["Client"]["Path"] = clientPath;
                            parser.WriteFile(iniPath, data);
                        }
                        if (!data.Sections.ContainsSection("folder"))
                        {
                            GetClientPathCredentials();
                            data.Sections.AddSection("folder");
                            for (int i = 0; i < Client_Folders.Count; i++)
                            {
                                data["folder"][$"Path{i + 1}"] = Client_Folders[i];
                            }
                            parser.WriteFile(iniPath, data);

                        }
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

                    clientPath = GetSafeDirectoryPath(clientPath);
                    serverPath=GetSafeDirectoryPath(serverPath);

                    string iniPath = Path.Combine(clientPath, "Config.ini");
                    parser.WriteFile(iniPath, data);
                }

                clientPath = GetSafeDirectoryPath(clientPath);
                serverPath = GetSafeDirectoryPath(serverPath);

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
            ValidateConfiguration();


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

                    if (!Directory.Exists(Path.GetDirectoryName(clientPath)) || string.IsNullOrWhiteSpace(clientPath))
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
                        continue; 
                    }
                    else
                    {
                        break; 
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
                    PrintInfo($"Tracked folders: {_syncMetadata.Folders.Count:N0}");
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

        static void ValidateConfiguration()
        {
            if (string.IsNullOrWhiteSpace(clientPath) || !Directory.Exists(clientPath))
            {
                Console.WriteLine($"Invalid client root path: {clientPath}");
                _syncRunning = false;

                return;
            }
            if (Client_Folders.Count == 0)
            {
                Console.WriteLine("No client folders found in configuration.");
                _syncRunning = false;

                return;
            }
            else
            {
                for (int i = 0; i < Client_Folders.Count; i++)
                {
                    string folder = Client_Folders[i];
                    if (!Directory.Exists(folder))
                    {
                        Console.WriteLine($"Check The INI File.");
                        _syncRunning = false;
                        return;
                    }
                }
            }
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

        static void SyncFiles(string sourceFolder, string targetFolder, string logFile, string direction)
        {
            if (!Directory.Exists(sourceFolder))
                return;

            string today = DateTime.Today.ToString("yyyy-MM-dd");
            string logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, logFile);

            PrintInfo($"Scanning {Path.GetFileName(sourceFolder)} for changes...");

            if (!_syncMetadata.Folders.TryGetValue(sourceFolder, out var folderMetadata))
            {
                folderMetadata = new FolderMetadata { FolderPath = sourceFolder };
                _syncMetadata.Folders[sourceFolder] = folderMetadata;
            }

            var allFiles = EnumerateFilesFast(sourceFolder).ToArray();
            PrintInfo($"Found {allFiles.Length:N0} files total");

            // Track current files
            var currentFiles = new HashSet<string>(); 

            var changedFiles = new ConcurrentBag<string>();
            var newFileMetadata = new ConcurrentDictionary<string, FileMetadata>();

            Parallel.ForEach(allFiles, new ParallelOptions { MaxDegreeOfParallelism = _maxDegreeOfParallelism }, file =>
            {
                try
                {
                    var fileInfo = new FileInfo(file);
                    string relativePath = Path.GetRelativePath(sourceFolder, file);
                    currentFiles.Add(relativePath);

                    var currentMetadata = new FileMetadata
                    {
                        LastModified = fileInfo.LastWriteTimeUtc,
                        FileSize = fileInfo.Length,
                        FilePath = relativePath
                    };

                    newFileMetadata[relativePath] = currentMetadata;
                    bool hasChanged = true;
                    if (folderMetadata.Files.TryGetValue(relativePath, out var oldMetadata))
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

            var missingFiles = new List<string>();
            foreach (var knownFile in folderMetadata.Files.Keys)
            {
                if (!currentFiles.Contains(knownFile))
                {
                    missingFiles.Add(knownFile);

                    if (newFileMetadata.TryGetValue(knownFile, out var fileMeta))
                    {
                        // File was in metadata but not found in current scan
                    }
                    else
                    {
                        // Add to metadata to maintain tracking
                        newFileMetadata[knownFile] = new FileMetadata
                        {
                            FilePath = knownFile,
                            LastModified = DateTime.MinValue,
                            FileSize = 0
                        };
                    }
                }
            }

            PrintInfo($"Found {changedFiles.Count:N0} changed file(s)");
            // Process missing files - check if they exist in target and copy back to source
            foreach (var relativePath in missingFiles)
            {
                try
                {
                    string targetFile = Path.Combine(targetFolder, relativePath);
                    string sourceFile = Path.Combine(sourceFolder, relativePath);

                    if (File.Exists(targetFile) && !File.Exists(sourceFile))
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(sourceFile));
                        File.Copy(targetFile, sourceFile, true);
                        PrintSuccess($" Restored: {relativePath} from {direction}");

                        var fileInfo = new FileInfo(sourceFile);
                        newFileMetadata[relativePath] = new FileMetadata
                        {
                            LastModified = fileInfo.LastWriteTimeUtc,
                            FileSize = fileInfo.Length,
                            FilePath = relativePath
                        };

                        // Log restoration
                        string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                        File.AppendAllLines(logPath, new[] {
                    $"[{timestamp}] RESTORED: {relativePath} from {direction}"
                });
                    }
                }
                catch (Exception ex)
                {
                    PrintError($"Error processing missing file {relativePath}: {ex.Message}");
                }
            }

            if (changedFiles.Count == 0 && missingFiles.Count == 0)
            {
                folderMetadata.Files = new Dictionary<string, FileMetadata>(newFileMetadata);
                folderMetadata.LastScanTime = DateTime.UtcNow;
                return;
            }
            if (!Directory.Exists(targetFolder))
            {
                Directory.CreateDirectory(targetFolder);
                PrintInfo($"Created target directory: {targetFolder}");
            }
            // Process changed files in batches
            int copiedFiles = 0;
            var changedFilesArray = changedFiles.ToArray();
            var totalChanges = changedFiles.Count() + missingFiles.Count();

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
                        var sourceInfo = new FileInfo(src);
                        UpdateFileMetadata(sourceFolder, relativePath, sourceInfo);

                        var destInfo = new FileInfo(dest);
                        UpdateFileMetadata(targetFolder, relativePath, destInfo);
                        Count++;

                        Interlocked.Increment(ref copiedFiles);
                        PrintSuccess($"  [✓] Copied: {relativePath} {direction}");

                    }
                    catch (Exception ex)
                    {
                        PrintError($"Error copying file {src}: {ex.Message}");
                    }
                });
            }
            // Also sync empty directories
            SyncEmptyDirectories(sourceFolder, targetFolder);
            folderMetadata.Files = new Dictionary<string, FileMetadata>(newFileMetadata);
            folderMetadata.LastScanTime = DateTime.UtcNow;
            SaveSyncMetadata();

            // Log results
            if (copiedFiles > 0 || missingFiles.Count > 0)
            {
                string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                var logLines = new List<string>
        {
            $"[{timestamp}]",
            $"Sync = Files",
            $"changes = {copiedFiles}",
            $"restorations = {missingFiles.Count}",
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

                    PrintInfo($"Starting optimized sync (since {_lastSyncTime:yyyy-MM-dd HH:mm:ss})");

                    foreach (var clientFolder in Client_Folders)
                    {
                        if (!Directory.Exists(clientFolder)) continue;


                        string clientFolderName = Path.GetFileName(clientFolder.TrimEnd(Path.DirectorySeparatorChar));

                        string correspondingServerFolder = Path.Combine(serverFolder, clientFolderName);

                        if (!Directory.Exists(correspondingServerFolder))
                        {
                            Directory.CreateDirectory(correspondingServerFolder);
                            PrintSuccess($"[+] Created folder on server: {correspondingServerFolder}");
                        }

                        SyncFiles(clientFolder, correspondingServerFolder, logFilePath, "ClientToServer");
                        SyncFiles(correspondingServerFolder, clientFolder, logFilePath, "ServerToClient");
                    }

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
                        string connectCmd = $@"net use {DRIVE_LETTER} ""\\{SERVER_IP}\{SHARE_NAME}"" /user:""{USERNAME}"" ""{PASSWORD}"" /persistent:no";


                        if (RunCommand(connectCmd, false))
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

        // Add this helper method to your Program class
        static void UpdateFileMetadata(string folderPath, string relativePath, FileInfo fileInfo)
        {
            // Get or create folder metadata
            if (!_syncMetadata.Folders.TryGetValue(folderPath, out var folderMetadata))
            {
                folderMetadata = new FolderMetadata { FolderPath = folderPath };
                _syncMetadata.Folders[folderPath] = folderMetadata;
            }

            // Update the file metadata
            folderMetadata.Files[relativePath] = new FileMetadata
            {
                LastModified = fileInfo.LastWriteTimeUtc,
                FileSize = fileInfo.Length,
                FilePath = relativePath,
                LastSyncTime = DateTime.UtcNow  
            };
        }

        // Method to synchronize empty directories between source and target
        static void SyncEmptyDirectories(string sourceFolder, string targetFolder)
        {
            try
            {
              
                var allDirectories = Directory.GetDirectories(sourceFolder, "*", SearchOption.AllDirectories);

                foreach (var sourceDir in allDirectories)
                {
                    string relativePath = Path.GetRelativePath(sourceFolder, sourceDir);
                    string targetDir = Path.Combine(targetFolder, relativePath);

                    if (!Directory.Exists(targetDir))
                    {
                        Directory.CreateDirectory(targetDir);
                        PrintInfo($"Created directory: {relativePath}");
                    }
                }
            }
            catch (Exception ex)
            {
                PrintError($"Error syncing directories: {ex.Message}");
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

        public static string GetSafeDirectoryPath(string clientPath)
        {
            try
            {
                if (Path.HasExtension(clientPath))
                {
                    clientPath = Path.GetDirectoryName(clientPath);
                }
            }
            catch (PathTooLongException)
            {
                Console.WriteLine("The specified path is too long.");

            }
            catch (ArgumentException)
            {
                Console.WriteLine("The specified path has an invalid format or contains illegal characters.");
                clientPath = null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
                
            }

            return clientPath;
        }
    }
}