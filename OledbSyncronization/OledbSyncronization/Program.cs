using System.Text.Json;
using System.Threading;
using System.Data.OleDb;
using System.Data;
using System.Net.NetworkInformation;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using IniParser;
using IniParser.Model;
using OledbSyncronization;

namespace RobustAccessDbSync
{
    class SyncMetadata
    {
        public Dictionary<string, DateTime> TableLastSync { get; set; } = new Dictionary<string, DateTime>();
    }

    class Program
    {
        static string DRIVE_LETTER = "X:";
        private static bool _syncRunning = true;
        private const string ConflictSuffix = "_CONFLICT_RESOLVED";
        static string SERVER_IP;
        static string SHARE_NAME;
        static string USERNAME;
        static string PASSWORD;
        private static bool _isOnline = true;
        private static DateTime _lastOnlineTime = DateTime.MinValue;
        private static int _syncCycleWaitMinutes = 1;
        private static Stopwatch _cycleTimer = new Stopwatch();
        private static DateTime _nextSyncTime = DateTime.Now;
        static string clientDbPath;
        static string serverDbPath;
        static string rememberedClientPath = null;
        static string clientFileFolder;
        static string serverFileFolder;

        // static string filePath = "user_data.txt"; // File to save the input

        static string syncMetaFile = "sync_metadata.json";

        static void GetClinetServerPath()
        {
            Console.Title = "Database Synchronization Tool";
            Console.CursorVisible = false;

            PrintHeader();
            ShowGameStyleLoader("Initializing Database Synchronization Tool", 20);
            while (true)
            {
                // Password input
                do
                {
                    Console.Write("Enter Client Path: ");
                    clientDbPath = Console.ReadLine();
                    if (string.IsNullOrWhiteSpace(clientDbPath))
                        Console.WriteLine("client path cannot empty");
                } while (string.IsNullOrWhiteSpace(clientDbPath));

                do
                {
                    Console.Write("Enter Server Path: ");
                    serverDbPath = Console.ReadLine();
                    if (string.IsNullOrWhiteSpace(serverDbPath))
                        Console.WriteLine("server path cannot empty");
                } while (string.IsNullOrWhiteSpace(serverDbPath));

                var serverParts = serverDbPath.Split(new[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);
                if (serverParts.Length < 2)
                {
                    PrintError("Invalid server path format. Expected format: \\\\server\\share\\path\\file.mdb");

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
        static void GetServerCredentials()
        {
            while (true)
            {
                // Username input
                do
                {
                    Console.Write("Enter USERNAME: ");
                    USERNAME = Console.ReadLine();
                    if (string.IsNullOrWhiteSpace(USERNAME))
                        Console.WriteLine("USERNAME cannot be empty.");
                } while (string.IsNullOrWhiteSpace(USERNAME));

                // Password input
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

        [System.Runtime.Versioning.SupportedOSPlatform("windows")]
        static async Task Main()
        {
            var parser = new FileIniDataParser();
            IniData data = null;

            const string pointerFile = "last_path.txt"; // Stores last-used client DB path

            try
            {

                // Step 1: Try to read last-used client path
                if (File.Exists(pointerFile))
                {
                    rememberedClientPath = File.ReadAllText(pointerFile)?.Trim();
                }

                // Step 2: Try to load config.ini if we know where to look
                if (!string.IsNullOrEmpty(rememberedClientPath))
                {

                    //string iniPath = Path.Combine(Path.GetDirectoryName(rememberedClientPath), "config.ini");
                    string iniPath = Path.Combine(rememberedClientPath, "config.ini");


                    if (File.Exists(iniPath))
                    {
                        data = parser.ReadFile(iniPath);

                        USERNAME = data["Credentials"]["Username"];
                        PASSWORD = data["Credentials"]["Password"];
                        SERVER_IP = data["Server"]["IP"];
                        SHARE_NAME = data["Server"]["Share"];
                        serverDbPath = data["Server"]["Path"];
                        clientDbPath = data["Client"]["Path"];

                        Console.WriteLine("Loaded saved configuration.");
                    }
                }

                // Step 3: If config not loaded, ask user and save it
                if (data == null)
                {
                    Console.WriteLine("No saved configuration found. Please enter details.");

                    GetClinetServerPath();       // sets clientDbPath
                    GetServerCredentials();      // sets USERNAME, PASSWORD

                    data = new IniData();
                    data["Credentials"]["Username"] = USERNAME;
                    data["Credentials"]["Password"] = PASSWORD;
                    data["Server"]["IP"] = SERVER_IP;
                    data["Server"]["Share"] = SHARE_NAME;
                    data["Server"]["Path"] = serverDbPath;
                    data["Client"]["Path"] = clientDbPath;

                    //string iniPath = Path.Combine(Path.GetDirectoryName(clientDbPath),"config.ini");
                    string iniPath = Path.Combine(clientDbPath, "config.ini");

                    parser.WriteFile(iniPath, data);

                    // Save clientDbPath to pointer file for future runs
                    File.WriteAllText(pointerFile, clientDbPath);

                    Console.WriteLine($" Configuration saved to: {iniPath}");
                }

                Console.WriteLine("Ready to sync using loaded configuration.");
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(" ERROR: " + ex.Message);
                Console.ResetColor();
            }


            bool isNewClientDb = false;

            if (!HasMdbExtension(clientDbPath))
            {
                if (File.Exists(Path.Combine(clientDbPath, Path.GetFileName(serverDbPath))))
                {
                    clientDbPath = Path.Combine(clientDbPath, Path.GetFileName(serverDbPath));
                }
                else
                {
                    while (true)
                    {
                        //string destFolder = clientDbPath; // Initialize with current clientDbPath

                        // If directory doesn't exist, get new input and restart the check immediately
                        if (!Directory.Exists(clientDbPath))
                        {
                            PrintError($"ERROR: Destination folder does not exist: {clientDbPath}");
                            Console.Write("Enter a valid destination folder: ");
                            clientDbPath = Console.ReadLine();
                            continue; // Restart loop to check the new input
                        }

                        // Proceed with network operations if directory exists
                        clientDbPath = Path.Combine(clientDbPath, Path.GetFileName(serverDbPath));
                        RunCommand($"net use {DRIVE_LETTER} /delete", false);

                        // PrintInfo("Mounting shared folder...");
                        var connectCmd = $"net use {DRIVE_LETTER} \\\\{SERVER_IP}\\{SHARE_NAME} /user:{USERNAME} {PASSWORD} /persistent:no";

                        if (!RunCommand(connectCmd))
                        {
                            PrintError("ERROR: Failed to connect to shared folder.");
                            GetServerCredentials();
                            GetClinetServerPath();
                            continue;
                        }

                        string serverFilePath = Path.Combine(DRIVE_LETTER, Path.GetFileName(serverDbPath));

                        if (!File.Exists(serverFilePath))
                        {
                            PrintError($"ERROR: File does not exist on server: {Path.GetFileName(serverDbPath)}");
                            RunCommand($"net use {DRIVE_LETTER} /delete", false);
                            Console.Write("Enter a valid server DB path: ");
                            serverDbPath = Console.ReadLine();
                            continue;
                        }

                        PrintInfo("Copying file from server...");

                        try
                        {
                            File.Copy(serverFilePath, clientDbPath, true);
                            PrintSuccess($"File successfully copied to: {clientDbPath}");
                            isNewClientDb = true;

                            // Clear any buffered key presses
                            while (Console.KeyAvailable)
                                Console.ReadKey(true);

                            // User input handling
                            PrintInfo("Synchronization is complete. (S) to Start Sync or (Q) to quit");

                            while (true)
                            {
                                if (Console.KeyAvailable)
                                {
                                    var key = Console.ReadKey(true).Key;

                                    if (key == ConsoleKey.Q)
                                    {
                                        _syncRunning = false;
                                        PrintWarning("Stopping synchronization...");
                                        Environment.Exit(0);
                                        break;
                                    }
                                    else if (key == ConsoleKey.S)
                                    {
                                        _syncRunning = true;
                                        PrintWarning("Starting synchronization...");
                                        break;
                                    }
                                }
                                await Task.Delay(100);
                            }

                            break; // Exit loop on success
                        }
                        catch (IOException ioEx)
                        {
                            PrintError($"File access error: {ioEx.Message}");
                            // Consider adding retry logic here with a delay
                        }
                        catch (UnauthorizedAccessException authEx)
                        {
                            PrintError($"Permission denied: {authEx.Message}");
                        }
                        catch (Exception ex)
                        {
                            PrintError($"ERROR: File copy failed. {ex.Message}");
                        }
                        RunCommand($"net use {DRIVE_LETTER} /delete", false);
                    }
                }
            }


            SyncMetadata metadata = null;

            if (!File.Exists(clientDbPath))
            {
                PrintInfo("Client database not found. Attempting to pull from server...");
                if (await PullDatabaseFromServer(serverDbPath, clientDbPath))
                {
                    PrintSuccess("Successfully pulled database from server to client.");
                    isNewClientDb = true;
                }
                else
                {
                    PrintError("\nPress any key to exit...");
                    Console.ReadKey();
                    return;
                }
            }

            ShowGameStyleLoader("Verifying database files", 10);
            Console.WriteLine();

            if (!VerifyDatabaseFiles(clientDbPath, serverDbPath))
            {
                PrintError("\nPress any key to exit...");
                Console.ReadKey();
                return;
            }
            string serverConnStr = $"Provider=Microsoft.Jet.OLEDB.4.0;Data Source={serverDbPath};";
            string clientConnStr = $"Provider=Microsoft.Jet.OLEDB.4.0;Data Source={clientDbPath};";


            ShowGameStyleLoader("Testing database connections", 20);
            if (!TestConnection("Client DB", clientConnStr) || !TestConnection("Server DB", serverConnStr))
            {
                PrintError("\nPress any key to exit...");
                Console.ReadKey();
                return;
            }

            bool Testingdata = isNewClientDb;
            UpdateNullServerzeit(clientConnStr, serverConnStr);

            metadata = LoadSyncMetadata(syncMetaFile) ?? new SyncMetadata();
            InitializeMetadata(metadata, clientConnStr, serverConnStr, isNewClientDb);

            PrintSuccess("\nStarting optimized synchronization...");
            PrintInfo("Press 'S' to stop, 'R' to restart, 'Q' to quit.\n");
            //await ContinuousSync(serverConnStr, clientConnStr, syncMetaFile, metadata);
            var syncTask = Task.Run(() => ContinuousSync(serverConnStr, clientConnStr, syncMetaFile, metadata));

            //while (_syncRunning)
            //{

            //    if (Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Q)
            //    {
            //        _syncRunning = false;
            //        PrintWarning("Stopping synchronization...");
            //    }
            //    await Task.Delay(100);
            //}

            //await syncTask;
            //PrintInfo("\nSynchronization stopped. Press any key to exit.");
            //Console.CursorVisible = true;
            //Console.ReadKey();
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
                        syncTask = Task.Run(() => ContinuousSync(serverConnStr, clientConnStr, syncMetaFile, metadata));
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

        static void UpdateNullServerzeitForTable(string connectionString, string tableName)
        {
            try
            {
                using var connection = new OleDbConnection(connectionString);
                connection.Open();

                string checkQuery = $"SELECT COUNT(*) FROM [{tableName}] WHERE Serverzeit IS NULL";
                using var checkCmd = new OleDbCommand(checkQuery, connection);
                int nullCount = (int)checkCmd.ExecuteScalar();

                if (nullCount == 0) return;

                string updateQuery = $"UPDATE [{tableName}] SET Serverzeit = ?";
                using var updateCmd = new OleDbCommand(updateQuery, connection);
                updateCmd.Parameters.AddWithValue("?", SafeTimestamp(DateTime.UtcNow));
                updateCmd.ExecuteNonQuery();

                PrintInfo($"[{tableName}] - Updated {nullCount} NULL Serverzeit values.");
            }
            catch (Exception ex)
            {
                PrintError($"[UpdateNullServerzeit] {tableName} - {ex.Message}");
            }
        }



        static void UpdateNullServerzeit(string clientConnStr, string serverConnStr)
        {
            var clientTables = GetAllTableNames(clientConnStr);
            var serverTables = GetAllTableNames(serverConnStr);

            Parallel.ForEach(clientTables, table =>
            {
                UpdateNullServerzeitForTable(clientConnStr, table);
            });

            Parallel.ForEach(serverTables, table =>
            {
                UpdateNullServerzeitForTable(serverConnStr, table);
            });
        }



        static void SyncTableStructure(string sourceConnStr, string targetConnStr, string tableName, SyncMetadata metadata)
        {
            try
            {
                using var targetConn = new OleDbConnection(targetConnStr);
                targetConn.Open();

                // Skip if table already exists in target
                if (TableExists(targetConn, tableName)) return;

                using var sourceConn = new OleDbConnection(sourceConnStr);
                sourceConn.Open();

                // Get table schema from source
                DataTable schema = sourceConn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables,
                    new object[] { null, null, tableName, "TABLE" });

                if (schema.Rows.Count == 0) return;

                // Get columns information
                DataTable columns = sourceConn.GetOleDbSchemaTable(OleDbSchemaGuid.Columns,
                    new object[] { null, null, tableName, null });

                // Get primary key information
                DataTable primaryKeys = sourceConn.GetOleDbSchemaTable(OleDbSchemaGuid.Primary_Keys,
                    new object[] { null, null, tableName });

                // Build CREATE TABLE statement
                var createTableSql = new StringBuilder($"CREATE TABLE [{tableName}] (");

                foreach (DataRow column in columns.Rows)
                {
                    string columnName = column["COLUMN_NAME"].ToString();
                    string dataType = GetSqlDataType(column);
                    bool isPrimaryKey = primaryKeys.Select($"COLUMN_NAME = '{columnName}'").Length > 0;

                    createTableSql.Append($"[{columnName}] {dataType}");

                    if (isPrimaryKey)
                        createTableSql.Append(" PRIMARY KEY");

                    createTableSql.Append(", ");
                }

                // Add Serverzeit if not exists
                if (columns.Select("COLUMN_NAME = 'Serverzeit'").Length == 0)
                {
                    createTableSql.Append("[Serverzeit] DATETIME DEFAULT Now(), ");
                }

                // Remove trailing comma and close
                createTableSql.Length -= 2;
                createTableSql.Append(")");

                // Execute creation
                ExecuteNonQuery(targetConn, createTableSql.ToString());
                Console.WriteLine($"Created table {tableName} in target database");

                metadata.TableLastSync[tableName] = DateTime.MinValue;
                SaveSyncMetadata(syncMetaFile, metadata);
                PrintInfo($"[New Table] Initialized sync time for '{tableName}' to {DateTime.MinValue:yyyy-MM-dd HH:mm:ss}");



            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error synchronizing table {tableName}: {ex.Message}");
            }
        }
        static void ExecuteNonQuery(OleDbConnection conn, string sql, params (string, object)[] parameters)
        {
            using var cmd = new OleDbCommand(sql, conn);
            foreach (var (name, value) in parameters)
            {
                cmd.Parameters.AddWithValue(name, value);
            }
            cmd.ExecuteNonQuery();
        }
        static void PrintHeader()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\nDatabase Synchronization Tool");
            Console.WriteLine("-----------------------------");
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

        static void InitializeMetadata(SyncMetadata metadata, string clientConnStr, string serverConnStr, bool isNewClientDb)
        {
            var clientTables = GetAllTableNames(clientConnStr);
            var serverTables = GetAllTableNames(serverConnStr);
            var allTables = clientTables.Union(serverTables, StringComparer.OrdinalIgnoreCase).ToList();

            using var clientConn = new OleDbConnection(clientConnStr);
            using var serverConn = new OleDbConnection(serverConnStr);
            clientConn.Open();
            serverConn.Open();

            Parallel.ForEach(allTables, table =>
            {
                foreach (var (conn, connStr, label) in new[] {
            (clientConn, clientConnStr, "client"),
            (serverConn, serverConnStr, "server")
        })
                {
                    try
                    {
                        if (!ColumnExists(conn, table, "Serverzeit"))
                        {
                            // Add Serverzeit column
                            string alterSql = $"ALTER TABLE [{table}] ADD COLUMN [Serverzeit] DATETIME DEFAULT Now()";
                            ExecuteNonQuery(conn, alterSql);

                            // Set current UTC as Serverzeit
                            DateTime utcNow = SafeTimestamp(DateTime.UtcNow);
                            string updateSql = $"UPDATE [{table}] SET Serverzeit = ?";
                            using var updateCmd = new OleDbCommand(updateSql, conn);
                            updateCmd.Parameters.AddWithValue("?", utcNow);
                            updateCmd.ExecuteNonQuery();

                            lock (metadata)
                            {
                                if (!metadata.TableLastSync.ContainsKey(table))
                                {
                                    metadata.TableLastSync[table] = utcNow.AddSeconds(-1);
                                    SaveSyncMetadata(syncMetaFile, metadata);
                                    PrintInfo($"[INIT] {label} '{table}': Added 'Serverzeit' and initialized metadata.");
                                }
                            }
                        }

                        // If Serverzeit exists but no metadata
                        if (!metadata.TableLastSync.ContainsKey(table))
                        {
                            string query = $"SELECT MAX(Serverzeit) FROM [{table}]";
                            using var cmd = new OleDbCommand(query, conn);
                            var result = cmd.ExecuteScalar();

                            DateTime maxTime = (result != null && result != DBNull.Value)
                                ? ((DateTime)result).ToUniversalTime()
                                : DateTime.MinValue;

                            lock (metadata)
                            {
                                metadata.TableLastSync[table] = maxTime;
                                SaveSyncMetadata(syncMetaFile, metadata);
                                PrintInfo($"[INIT] {label} '{table}': Initialized sync time to {maxTime:yyyy-MM-dd HH:mm:ss}");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        lock (metadata)
                        {
                            if (!metadata.TableLastSync.ContainsKey(table))
                            {
                                metadata.TableLastSync[table] = DateTime.MinValue;
                                SaveSyncMetadata(syncMetaFile, metadata);
                            }
                        }

                        PrintWarning($"[INIT] {label} '{table}' failed: {ex.Message}");
                    }
                }

                // Sync table structure both ways
                SyncTableStructure(clientConnStr, serverConnStr, table, metadata);
                SyncTableStructure(serverConnStr, clientConnStr, table, metadata);
            });
        }

        // Helper method to check if column exists
        static bool ColumnExists(OleDbConnection conn, string tableName, string columnName)
        {
            try
            {
                DataTable columns = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Columns,
                    new object[] { null, null, tableName, null });

                return columns.Rows.Cast<DataRow>()
                    .Any(row => row["COLUMN_NAME"].ToString().Equals(columnName, StringComparison.OrdinalIgnoreCase));
            }
            catch
            {
                return false;
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
                PrintError("Command execution failed: " + ex.Message);
                return false;
            }
        }

        public static bool HasMdbExtension(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return false;

            string extension = Path.GetExtension(path);


            return extension.Equals(".mdb", StringComparison.OrdinalIgnoreCase) || extension.Equals(".crm", StringComparison.OrdinalIgnoreCase);
        }

        static async Task<bool> PullDatabaseFromServer(string serverPath, string clientPath)
        {
            try
            {
                var serverParts = serverPath.Split(new[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);
                if (serverParts.Length < 2)
                {
                    PrintError("Invalid server path format. Expected format: \\\\server\\share\\path\\file.mdb");
                    return false;
                }

                string serverIP = serverParts[0];
                string shareName = serverParts[1];
                string serverFilePath = string.Join("\\", serverParts.Skip(2));
                string fileName = Path.GetFileName(serverPath);

                if (!PingHost("127.0.0.1") || !PingHost(serverIP))
                {
                    PrintError("ERROR: Network connectivity issues");
                    return false;
                }

                bool isClientPathDirectory = Directory.Exists(clientPath) ||
                                           (clientPath.EndsWith("\\") ||
                                            clientPath.EndsWith("/"));

                string finalClientPath;
                if (isClientPathDirectory)
                {
                    Directory.CreateDirectory(clientPath);
                    finalClientPath = Path.Combine(clientPath, fileName);
                }
                else
                {
                    string directory = Path.GetDirectoryName(clientPath);
                    if (!string.IsNullOrEmpty(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }
                    finalClientPath = clientPath;
                }

                RunCommand($"net use {DRIVE_LETTER} /delete", false);

                PrintInfo("Mounting server share...");
                string connectCmd = $"net use {DRIVE_LETTER} \\\\{serverIP}\\{shareName} /user:{USERNAME} {PASSWORD} /persistent:no";
                if (!RunCommand(connectCmd))
                {
                    PrintError("ERROR: Failed to connect to shared folder");
                    return false;
                }

                string serverFile = $"{DRIVE_LETTER}\\{serverFilePath}";
                if (!File.Exists(serverFile))
                {
                    PrintError($"ERROR: File does not exist on server: {serverFilePath}");
                    RunCommand($"net use {DRIVE_LETTER} /delete", false);
                    return false;
                }

                PrintInfo($"Copying file from server to {finalClientPath}...");
                try
                {
                    File.Copy(serverFile, finalClientPath, true);
                    PrintSuccess("File successfully copied");
                    return true;
                }

                catch (Exception ex)
                {
                    PrintError($"ERROR: File copy failed: {ex.Message}");
                    return false;
                }
                finally
                {
                    RunCommand($"net use {DRIVE_LETTER} /delete", false);
                }
            }
            catch (Exception ex)
            {
                PrintError($"Error pulling database from server: {ex.Message}");
                return false;
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
        static void SyncFiles(string sourceFolder, string targetFolder, string logFile)
        {
            if (!Directory.Exists(sourceFolder) || !Directory.Exists(targetFolder))
                return;

            string today = DateTime.Today.ToString("yyyy-MM-dd");

            string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".pdf", ".docx", ".xlsx", ".txt", ".jfif" };

            string[] sourceFiles = Directory
                .EnumerateFiles(sourceFolder, ".", SearchOption.AllDirectories)
                .Where(f =>
                    !f.EndsWith(".ldb", StringComparison.OrdinalIgnoreCase) &&
                    allowedExtensions.Contains(Path.GetExtension(f).ToLower()))
                .ToArray();

            var loggedFiles = new HashSet<string>();
            string logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, logFile);

            if (File.Exists(logPath))
            {
                var existingLogs = File.ReadAllLines(logPath);
                loggedFiles = existingLogs
                    .Where(line => line.StartsWith(today))
                    .Select(line => line.Split('|')[1])
                    .ToHashSet();
            }

            foreach (var src in sourceFiles)
            {
                string relativePath = Path.GetRelativePath(sourceFolder, src);
                string dest = Path.Combine(targetFolder, relativePath);

                Directory.CreateDirectory(Path.GetDirectoryName(dest)!);

                bool shouldCopy = !File.Exists(dest) || File.GetLastWriteTimeUtc(src) > File.GetLastWriteTimeUtc(dest);
                bool alreadyLoggedToday = loggedFiles.Contains(relativePath);

                if (shouldCopy)
                {
                    File.Copy(src, dest, true);

                    if (!alreadyLoggedToday)
                    {
                        File.AppendAllText(logPath, $"{today}|{relativePath}|{File.GetLastWriteTimeUtc(src)}{Environment.NewLine}");
                        PrintSuccess($"File synced: {relativePath}");
                    }
                }
            }
        }
        static void SyncFilesBothDirections(string clientDbPath, string serverDbPath)
        {
            string logFilePath = Path.Combine(Path.GetDirectoryName(clientDbPath), "file_sync_log.txt");

            if (HasMdbExtension(clientDbPath) && HasMdbExtension(serverDbPath))
            {
                string clientFolder = Path.GetDirectoryName(clientDbPath)!;
                string serverFolder = Path.GetDirectoryName(serverDbPath)!;

                if (Directory.Exists(clientFolder) && Directory.Exists(serverFolder))
                {

                    SyncFiles(clientFolder, serverFolder, logFilePath);
                    SyncFiles(serverFolder, clientFolder, logFilePath);
                }
            }
            else
            {
                if (Directory.Exists(clientDbPath) && Directory.Exists(serverDbPath))
                {
                    SyncFiles(clientDbPath, serverDbPath, logFilePath);
                    SyncFiles(serverDbPath, clientDbPath, logFilePath);
                }
            }
        }


        static async Task ContinuousSync(
string serverConnStr,
string clientConnStr,
string syncMetaFile,
SyncMetadata metadata)
        {
            // if (!_syncRunning) return;

            while (_syncRunning)
            {

                try
                {
                    var clientTables = GetAllTableNames(clientConnStr);
                    var serverTables = GetAllTableNames(serverConnStr);
                    var allTables = clientTables.Union(serverTables).ToList();

                    foreach (var table in allTables)
                    {
                        if (!_syncRunning) break;
                        DateTime lastSync1 = metadata.TableLastSync.TryGetValue(table, out var syncTime2)
                                   ? syncTime2
                                   : DateTime.MinValue;
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
                            var fileSyncTask = Task.Run(() => SyncFilesBothDirections(clientDbPath, serverDbPath));

                            //  List<DateTime> lastSyncTimes1 = metadata.TableLastSync.Values.ToList();
                            int totalChanges = 0;

                            foreach (var tableName in allTables)
                            {
                                if (!_syncRunning) break;
                                try
                                {

                                    // Ensure no null Serverzeit entries
                                    UpdateNullServerzeitForTable(clientConnStr, tableName);
                                    UpdateNullServerzeitForTable(serverConnStr, tableName);

                                    // Load last sync timestamp from metadata
                                    DateTime lastSync = metadata.TableLastSync.TryGetValue(tableName, out var syncTime)
                                        ? syncTime
                                        : DateTime.MinValue;

                                    PrintInfo($"Syncing {tableName} since {lastSync:yyyy-MM-dd HH:mm:ss}");


                                    // Sync Server → Client
                                    int serverToClient = await SyncDirection(
                                        sourceConnStr: serverConnStr,
                                        targetConnStr: clientConnStr,
                                        tableName: tableName,
                                        lastSync: lastSync,
                                        isServerToClient: true,
                                        metadata: metadata
                                    );

                                    if (serverToClient > 0)
                                    {
                                        PrintSuccess($"{tableName} sync: Server → Client: {serverToClient}");
                                        totalChanges += serverToClient;
                                    }

                                    lastSync = metadata.TableLastSync.TryGetValue(tableName, out var syncTime1)
                                        ? syncTime1
                                        : DateTime.MinValue;
                                    // Refresh last sync from metadata in case it was updated

                                    // Sync Client → Server
                                    int clientToServer = await SyncDirection(
                                        sourceConnStr: clientConnStr,
                                        targetConnStr: serverConnStr,
                                        tableName: tableName,
                                        lastSync: lastSync,
                                        isServerToClient: false,
                                        metadata: metadata
                                    );

                                    if (clientToServer > 0)
                                    {
                                        PrintSuccess($"{tableName} sync: Client → Server: {clientToServer}");
                                        totalChanges += clientToServer;
                                    }

                                    if (serverToClient > 0 || clientToServer > 0)
                                    {
                                        //PrintSuccess($"{tableName} sync: Server→Client: {serverToClient}, Client→Server: {clientToServer}");// Do not overwrite metadata.TableLastSync here
                                        //totalChanges += serverToClient + clientToServer;

                                        //metadata.TableLastSync[tableName] = lastSync;
                                        //SaveSyncMetadata(syncMetaFile, metadata);


                                    }
                                    if (serverToClient == 0 && clientToServer == 0)
                                    {
                                        PrintInfo($"No changes for {tableName}");
                                    }

                                    // Optional: show updated sync timestamp
                                    if (metadata.TableLastSync.TryGetValue(tableName, out var updatedSyncTime))
                                    {
                                        //PrintInfo($"Updated sync time for {tableName}: {updatedSyncTime:yyyy-MM-dd HH:mm:ss}");
                                    }
                                }
                                catch (Exception ex)
                                {
                                    // PrintError($"Error syncing table {tableName}: {ex.Message}");
                                }
                            }
                            await fileSyncTask;
                            _cycleTimer.Stop();
                            PrintSuccess($"Sync cycle completed in {_cycleTimer.Elapsed.TotalSeconds:0.00} seconds. Total changes: {totalChanges}");

                            if (totalChanges == 0)
                            {
                                PrintInfo("No changes detected in this cycle");
                            }
                        }
                        else
                        {
                            if (_lastOnlineTime != DateTime.MinValue)
                            {
                                PrintWarning("Connection lost - entering offline mode");
                                _lastOnlineTime = DateTime.MinValue;
                            }

                        }

                        // Wait until the next sync cycle
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
                }
                catch (Exception ex)
                {
                    PrintError($"Sync cycle error: {ex.Message}");
                }
                finally
                {
                    SaveSyncMetadata(syncMetaFile, metadata);
                }
            }
        }




        static async Task<int> SyncDirection(
        string sourceConnStr,
            string targetConnStr,
            string tableName,
            DateTime lastSync,
            bool isServerToClient,
            SyncMetadata metadata)
        {
            int changesApplied = 0;

            DateTime maxTimestamp = lastSync;

            try
            {


                if (isServerToClient)
                {
                    var clientTables = GetAllTableNames(sourceConnStr);
                    var serverTables = GetAllTableNames(targetConnStr);
                    var newTables = clientTables.Except(serverTables).ToList();

                    foreach (var table in newTables)
                    {
                        SyncTableStructure(sourceConnStr, targetConnStr, table, metadata);
                    }


                }
                if (!isServerToClient)
                {
                    var clientTables = GetAllTableNames(sourceConnStr);
                    var serverTables = GetAllTableNames(targetConnStr);
                    var newTables = clientTables.Except(serverTables).ToList();


                    foreach (var table in newTables)
                    {
                        SyncTableStructure(sourceConnStr, targetConnStr, table, metadata);
                    }
                }
                using (var sourceConn = new OleDbConnection(sourceConnStr))
                {
                    sourceConn.Open();

                    string pkColumn = GetPrimaryKeyColumn(sourceConnStr, tableName);
                    if (string.IsNullOrEmpty(pkColumn))
                    {
                        ExecuteNonQuery(sourceConn, $"ALTER TABLE [{tableName}] ADD COLUMN [DefaultSynCID] GUID");
                        ExecuteNonQuery(sourceConn, $"ALTER TABLE [{tableName}] ADD CONSTRAINT pk_{tableName}_DefaultSynCID PRIMARY KEY ([DefaultSynCID])");
                    }



                    if (isServerToClient)
                    {
                        string query = $@"SELECT * FROM [{tableName}] 
                                                    WHERE Serverzeit > ?
                                                    ORDER BY Serverzeit DESC";  // Newest first
                        using (var cmd = new OleDbCommand(query, sourceConn))
                        {
                            DateTime cleanedLastSync = SafeTimestamp(lastSync.ToLocalTime()); // keep original timestamp, strip milliseconds
                            cmd.Parameters.AddWithValue("?", cleanedLastSync);

                            using (var reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    var row = new Dictionary<string, object>();
                                    for (int i = 0; i < reader.FieldCount; i++)
                                    {
                                        row[reader.GetName(i)] = reader.IsDBNull(i) ? null : reader.GetValue(i);

                                    }
                                    using (var targetConn = new OleDbConnection(targetConnStr))
                                    {
                                        string Targetpkcolumn = GetPrimaryKeyColumn(targetConnStr, tableName);
                                        //var Loggingdata = new List<ChangesModel>();
                                        targetConn.Open();
                                        string Targetquery = $@"SELECT * FROM [{tableName}] 
                                                WHERE [{Targetpkcolumn}] = ?";
                                        var Loggingdata = new List<ChangesModel>();

                                        using (var cmd2 = new OleDbCommand(Targetquery, targetConn))
                                        {
                                            cmd2.Parameters.AddWithValue("?", row[pkColumn]);
                                            using (var reader1 = cmd2.ExecuteReader())
                                            {
                                                if (reader1.HasRows)
                                                {
                                                    while (reader1.Read())
                                                    {
                                                        var TargetLog = new Dictionary<string, object>();

                                                        for (int i = 0; i < reader1.FieldCount; i++)
                                                        {
                                                            TargetLog[reader1.GetName(i)] = reader1.IsDBNull(i) ? null : reader1.GetValue(i);
                                                        }

                                                        foreach (var key in row.Keys)
                                                        {
                                                            if (key == "Serverzeit")
                                                                continue;

                                                            if (!TargetLog.ContainsKey(key))
                                                                continue;

                                                            string oldVal = TargetLog[key]?.ToString();
                                                            string newVal = row[key]?.ToString();

                                                            if (oldVal != newVal)
                                                            {
                                                                Loggingdata.Add(new ChangesModel
                                                                {
                                                                    Direction = "ServerToClient",
                                                                    ColumnName = key,
                                                                    OldValue = oldVal,
                                                                    NewValue = newVal,
                                                                });
                                                            }
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    // INSERT CASE: row does not exist in target
                                                    foreach (var key in row.Keys)
                                                    {
                                                        if (key == "Serverzeit")
                                                            continue;

                                                        Loggingdata.Add(new ChangesModel
                                                        {
                                                            Direction = "ServerToClient",
                                                            ColumnName = key,
                                                            OldValue = null,
                                                            NewValue = row[key]?.ToString(),
                                                        });
                                                    }
                                                }
                                            }

                                        }

                                        if (ApplyChangeWithConflictResolution(
                                            targetConn,
                                            tableName,
                                            row,
                                            isServerToClient,
                                            pkColumn))
                                        {
                                            changesApplied++;
                                            var rowTimestamp = Convert.ToDateTime(row["Serverzeit"]);
                                            if (rowTimestamp > maxTimestamp)
                                                maxTimestamp = rowTimestamp;
                                        }
                                        if (Loggingdata.Count > 0)
                                        {
                                            string logFilePath = Path.Combine(Path.GetDirectoryName(clientDbPath), "Configlog.ini");

                                            WriteChangesToIni(tableName, row[pkColumn], Loggingdata, logFilePath);
                                        }

                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        string query = $@"SELECT * FROM [{tableName}] 
                                                    WHERE Serverzeit > ?
                                                    ORDER BY Serverzeit DESC";  // Newest first
                        using (var cmd = new OleDbCommand(query, sourceConn))
                        {
                            DateTime cleanedLastSync = SafeTimestamp(lastSync.ToLocalTime()); // keep original timestamp, strip milliseconds
                            cmd.Parameters.AddWithValue("?", cleanedLastSync);

                            using (var reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    var row = new Dictionary<string, object>();
                                    for (int i = 0; i < reader.FieldCount; i++)
                                    {
                                        row[reader.GetName(i)] = reader.IsDBNull(i) ? null : reader.GetValue(i);

                                    }
                                    using (var targetConn = new OleDbConnection(targetConnStr))
                                    {
                                        string Targetpkcolumn = GetPrimaryKeyColumn(targetConnStr, tableName);
                                        //var Loggingdata = new List<ChangesModel>();
                                        targetConn.Open();
                                        string Targetquery = $@"SELECT * FROM [{tableName}] 
                                                WHERE [{Targetpkcolumn}] = ?";
                                        var Loggingdata = new List<ChangesModel>();

                                        using (var cmd2 = new OleDbCommand(Targetquery, targetConn))
                                        {
                                            cmd2.Parameters.AddWithValue("?", row[pkColumn]);
                                            using (var reader1 = cmd2.ExecuteReader())
                                            {
                                                if (reader1.HasRows)
                                                {
                                                    while (reader1.Read())
                                                    {
                                                        var TargetLog = new Dictionary<string, object>();

                                                        for (int i = 0; i < reader1.FieldCount; i++)
                                                        {
                                                            TargetLog[reader1.GetName(i)] = reader1.IsDBNull(i) ? null : reader1.GetValue(i);
                                                        }

                                                        foreach (var key in row.Keys)
                                                        {
                                                            if (key == "Serverzeit")
                                                                continue;

                                                            if (!TargetLog.ContainsKey(key))
                                                                continue;

                                                            string oldVal = TargetLog[key]?.ToString();
                                                            string newVal = row[key]?.ToString();

                                                            if (oldVal != newVal)
                                                            {
                                                                Loggingdata.Add(new ChangesModel
                                                                {
                                                                    Direction = "ClientToServer",
                                                                    ColumnName = key,
                                                                    OldValue = oldVal,
                                                                    NewValue = newVal,
                                                                });
                                                            }
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    // INSERT CASE: row does not exist in target
                                                    foreach (var key in row.Keys)
                                                    {
                                                        if (key == "Serverzeit")
                                                            continue;

                                                        Loggingdata.Add(new ChangesModel
                                                        {
                                                            Direction = "ClientToServer",
                                                            ColumnName = key,
                                                            OldValue = null,
                                                            NewValue = row[key]?.ToString(),
                                                        });
                                                    }
                                                }
                                            }
                                        }

                                        if (ApplyChangeWithConflictResolution(
                                        targetConn,
                                        tableName,
                                        row,
                                        isServerToClient,
                                        pkColumn))
                                        {
                                            changesApplied++;
                                            var rowTimestamp = Convert.ToDateTime(row["Serverzeit"]);
                                            if (rowTimestamp > maxTimestamp)
                                                maxTimestamp = rowTimestamp;
                                        }
                                        if (Loggingdata.Count > 0)
                                        {
                                            string logFilePath = Path.Combine(Path.GetDirectoryName(clientDbPath), "Configlog.ini");

                                            WriteChangesToIni(tableName, row[pkColumn], Loggingdata, logFilePath);
                                        }

                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //PrintError($"Error syncing {tableName}: {ex.Message}");
            }
            if (maxTimestamp >= metadata.TableLastSync[tableName])
            {
                if (changesApplied > 0 && isServerToClient == true)
                {
                    maxTimestamp = maxTimestamp.AddSeconds(1).ToUniversalTime();
                    metadata.TableLastSync[tableName] = maxTimestamp;
                    SaveSyncMetadata(syncMetaFile, metadata);

                }
                else
                {
                    metadata.TableLastSync[tableName] = maxTimestamp.ToUniversalTime();
                    SaveSyncMetadata(syncMetaFile, metadata);
                }

            }
            // Add this after important metadata updates

            return changesApplied;
        }

        static void WriteChangesToIni(string tableName, object primaryKey, List<ChangesModel> changes, string filePath)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"[TableName: {tableName}]");
            sb.AppendLine($"Direction = {changes[0].Direction}");
            sb.AppendLine($"PrimaryKey = {primaryKey}");    
            // sb.AppendLine($"ChangedAt = {primaryKey}");

            foreach (var change in changes)
            {
                sb.AppendLine($"ColumnName {change.ColumnName} = {change.OldValue} -> {change.NewValue}");
            }
            sb.AppendLine($"ChangedAt = {DateTime.Now:O}");
            sb.AppendLine();
            File.AppendAllText(filePath, sb.ToString());
        }

        static DateTime SafeTimestamp(DateTime dt)
        {
            return new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second, dt.Kind);
        }

        static bool ApplyChangeWithConflictResolution(OleDbConnection targetConn,
                                            string tableName,
                                            Dictionary<string, object> row,
                                            bool isServerToClient,
                                            string pkColumn)
        {
            try
            {
                var pkValue = row[pkColumn]; // This is a GUID
                                             //  var incomingLastModified = Convert.ToDateTime(row["Serverzeit"]);

                bool exists = RecordExists(targetConn, tableName, pkColumn, pkValue);
                if (!exists)
                    return InsertRecord(targetConn, tableName, row);

                // var targetLastModified = GetLastModified(targetConn, tableName, pkColumn, pkValue);
                //var targetRecord = GetRecord(targetConn, tableName, pkColumn, pkValue);

                // Simple conflict resolution - server wins
                if (isServerToClient)
                {
                    //  bool dataIsDifferent = !row["Name"].Equals(targetRecord["Name"]);

                    return UpdateRecord(targetConn, tableName, row, pkColumn);
                }
                else
                {
                    // For client to server, only update if client has newer version
                    return UpdateRecord(targetConn, tableName, row, pkColumn);

                }
            }
            catch (Exception ex)
            {

                return false;
            }

        }


        static string GetSqlDataType(DataRow column)

        {
            int oleDbType = (int)column["DATA_TYPE"];
            int size = column["CHARACTER_MAXIMUM_LENGTH"] is DBNull ? 0 : Convert.ToInt32(column["CHARACTER_MAXIMUM_LENGTH"]);

            switch (oleDbType)
            {
                case 130: return size > 0 ? $"TEXT({size})" : "TEXT(255)";
                case 3: return "INTEGER";
                case 5: return "DOUBLE";
                case 7: return "DATETIME";
                case 11: return "BIT";
                case 72: return "GUID";
                case 203: return "MEMO";
                default: return "TEXT(255)";
            }
        }

        static SyncMetadata LoadSyncMetadata(string path)
        {
            if (!File.Exists(path)) return new SyncMetadata();

            try
            {
                var json = File.ReadAllText(path);
                return JsonSerializer.Deserialize<SyncMetadata>(json);
            }
            catch (Exception ex)
            {
                PrintError($"Error loading metadata: {ex.Message}");
                return new SyncMetadata();
            }
        }

        static void SaveSyncMetadata(string path, SyncMetadata metadata)
        {
            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                var json = JsonSerializer.Serialize(metadata, options);
                File.WriteAllText(path, json);
            }
            catch (Exception ex)
            {
                PrintError($"Error saving metadata: {ex.Message}");
            }
        }

        static bool PingHost(string nameOrAddress)
        {
            try
            {
                using (var ping = new Ping())
                {
                    var reply = ping.Send(nameOrAddress, 2000);
                    return reply.Status == IPStatus.Success;
                }
            }
            catch
            {
                return false;
            }
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

        static bool VerifyDatabaseFiles(string clientPath, string serverPath)
        {
            if (!File.Exists(clientPath))
            {
                PrintError($"\nClient database not found at: {clientPath}");
                return false;
            }

            if (!File.Exists(serverPath))
            {
                PrintError($"\nServer database not found at: {serverPath}");
                return false;
            }

            return true;
        }

        static bool TestConnection(string name, string connectionString)
        {
            try
            {
                using var connection = new OleDbConnection(connectionString);
                connection.Open();
                PrintSuccess($"{name} connection successfull");
                return true;
            }
            catch (Exception ex)
            {
                PrintError($"\n{name} connection failed: {ex.Message}");
                return false;
            }
        }

        static List<string> GetAllTableNames(string connectionString)
        {
            var tables = new List<string>();
            try
            {
                using var conn = new OleDbConnection(connectionString);
                conn.Open();
                DataTable schemaTables = conn.GetSchema("Tables");

                foreach (DataRow row in schemaTables.Rows)
                {
                    string tableName = row["TABLE_NAME"].ToString();
                    string tableType = row["TABLE_TYPE"].ToString();

                    if (tableType == "TABLE" && !tableName.StartsWith("MSys")
                        && !tableName.StartsWith("~TMP") && !tableName.StartsWith("_"))
                    {
                        tables.Add(tableName);
                    }
                }
            }
            catch (Exception ex)
            {
                PrintError($"Error getting table names: {ex.Message}");
            }
            return tables;
        }

        static string GetPrimaryKeyColumn(string connectionString, string tableName)
        {
            try
            {
                using var conn = new OleDbConnection(connectionString);
                conn.Open();

                DataTable schema = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Primary_Keys,
                    new object[] { null, null, tableName });

                if (schema.Rows.Count > 0)
                {
                    return schema.Rows[0]["COLUMN_NAME"].ToString();
                }

                DataTable columns = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Columns,
                    new object[] { null, null, tableName, null });

                foreach (DataRow row in columns.Rows)
                {
                    string col = row["COLUMN_NAME"].ToString();

                    if (col.Equals("ID", StringComparison.OrdinalIgnoreCase) ||
                        col.Equals("GUID", StringComparison.OrdinalIgnoreCase))
                    {
                        return col;
                    }
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        static bool TableExists(OleDbConnection conn, string tableName)
        {
            try
            {
                DataTable schema = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables,
                    new object[] { null, null, tableName, "TABLE" });
                return schema.Rows.Count > 0;
            }
            catch
            {
                return false;
            }
        }

        static bool RecordExists(OleDbConnection conn, string tableName, string pkColumn, object pkValue)
        {
            try
            {
                string query = $"SELECT COUNT(*) FROM [{tableName}] WHERE [{pkColumn}] = ?";
                using var cmd = new OleDbCommand(query, conn);
                cmd.Parameters.AddWithValue($"@{pkColumn}", pkValue);
                return (int)cmd.ExecuteScalar() > 0;
            }
            catch
            {
                return false;
            }
        }

        static bool InsertRecord(OleDbConnection conn, string tableName, Dictionary<string, object> row)
        {
            try
            {
                var columns = row.Keys.ToList();
                var columnList = string.Join(", ", columns.Select(c => $"[{c}]"));
                var valuePlaceholders = string.Join(", ", columns.Select(_ => "?"));

                string insertQuery = $@"INSERT INTO [{tableName}] ({columnList}) VALUES ({valuePlaceholders})";

                using var cmd = new OleDbCommand(insertQuery, conn);
                foreach (var col in columns)
                    cmd.Parameters.AddWithValue($"@{col}", row[col] ?? DBNull.Value);

                return cmd.ExecuteNonQuery() > 0;
            }
            catch (Exception ex)
            {
                PrintError($"Error inserting record into {tableName}: {ex.Message}");
                return false;
            }
        }

        static bool UpdateRecord(OleDbConnection conn, string tableName, Dictionary<string, object> row, string pkColumn)
        {
            try
            {
                var columns = row.Keys.Where(k => k != pkColumn).ToList();
                var updateSet = string.Join(", ", columns.Select(c => $"[{c}] = ?"));
                string updateQuery = $@"UPDATE [{tableName}] SET {updateSet} WHERE [{pkColumn}] = ?";

                using var cmd = new OleDbCommand(updateQuery, conn);
                foreach (var col in columns)
                    cmd.Parameters.AddWithValue($"@{col}", row[col] ?? DBNull.Value);
                cmd.Parameters.AddWithValue($"@{pkColumn}", row[pkColumn]);

                return cmd.ExecuteNonQuery() > 0;
            }
            catch (Exception ex)
            {
                PrintError($"Error updating record in {tableName}: {ex.Message}");
                return false;
            }
        }

        static DateTime GetLastModified(OleDbConnection conn, string tableName, string pkColumn, object pkValue)
        {
            try
            {
                string query = $"SELECT Serverzeit FROM [{tableName}] WHERE [{pkColumn}] = ?";
                using var cmd = new OleDbCommand(query, conn);
                cmd.Parameters.AddWithValue($"@{pkColumn}", pkValue);
                var result = cmd.ExecuteScalar();
                return (result != DBNull.Value && result != null) ? Convert.ToDateTime(result) : DateTime.MinValue;
            }
            catch
            {
                return DateTime.MinValue;
            }
        }
    }
}