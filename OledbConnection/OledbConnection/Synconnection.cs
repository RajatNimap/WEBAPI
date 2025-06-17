
//using System.Text.Json;
//using System.Threading;
//using System.Data.OleDb;
//using System.Data;
//using System.Net.NetworkInformation;
//using System.IO;
//using System.Diagnostics;

//namespace OledbConnection
//{
//    class SyncMetadata
//    {
//        public DateTime LastClientSyncTime { get; set; }
//        public DateTime LastServerSyncTime { get; set; }

//    }

//    class Program
//    {
//        static string DRIVE_LETTER = "X:";

//        private static bool _syncRunning = true;
//        private static DateTime _lastClientSyncTime;
//        private static DateTime _lastServerSyncTime;
//        private const string ConflictSuffix = "_CONFLICT_RESOLVED";
//        static string SERVER_IP = "95.111.230.3";
//    static string SHARE_NAME = "BatFolder";
//    static string USERNAME = "administrator";
//    static string PASSWORD = "N1m@p2025$Server";

//        [System.Runtime.Versioning.SupportedOSPlatform("windows")]
//        static async Task Main()
//        {
//            Console.CursorVisible = false;
//            ShowGameStyleLoader("Initializing Database Synchronization Tool", 20);

//            Console.WriteLine("\nDatabase Synchronization Tool");
//            Console.WriteLine("-----------------------------");

//            // Get user inputs
//            Console.Write("Enter client database path (e.g., C:\\path\\client.mdb): ");
//            string clientDbPath = Console.ReadLine();
//            if (!HasMdbExtension(clientDbPath))
//            {
//                while (true)
//                {
//                    Console.Write("Enter the file name on the server (e.g., C# Notes.txt): ");
//                    string fileName = Console.ReadLine();

//                    Console.Write("Enter destination folder (e.g., C:\\DemoFiles): ");
//                    string destFolder = Console.ReadLine();

//                    if (!Directory.Exists(destFolder))
//                    {
//                        Console.WriteLine($"ERROR: Destination folder does not exist: {destFolder}");
//                        Console.ReadKey();
//                        continue;
//                    }

//                    // Combine folder + filename to get final local path
//                     clientDbPath = Path.Combine(destFolder, Path.GetFileName(fileName));


//                    RunCommand($"net use {DRIVE_LETTER} /delete", false); // Clean up existing

//                    Console.WriteLine("Mounting shared folder...");
//                    var connectCmd = $"net use {DRIVE_LETTER} \\\\{SERVER_IP}\\{SHARE_NAME} /user:{USERNAME} {PASSWORD} /persistent:no";
//                    if (!RunCommand(connectCmd))
//                    {
//                        Console.WriteLine("ERROR: Failed to connect to shared folder.");
//                        Console.ReadKey();
//                        continue;
//                    }

//                    string serverFilePath = Path.Combine(DRIVE_LETTER, fileName);

//                    if (!File.Exists(serverFilePath))
//                    {
//                        Console.WriteLine($"ERROR: File does not exist on server: {fileName}");
//                        RunCommand($"net use {DRIVE_LETTER} /delete", false);
//                        Console.ReadKey();
//                        continue;
//                    }

//                    Console.WriteLine("Copying file from server...");
//                    try
//                    {
//                        File.Copy(serverFilePath, clientDbPath, true);
//                        Console.WriteLine($"File successfully copied to: {clientDbPath}");
//                    }
//                    catch (Exception ex)
//                    {
//                        Console.WriteLine($"ERROR: File copy failed. {ex.Message}");
//                    }

//                    RunCommand($"net use {DRIVE_LETTER} /delete", false);

//                    Console.Write("Do you want to continue? Type q to quit, any other key to continue: ");
//                    var input = Console.ReadLine();
//                    if (input?.ToLower() == "q")
//                        break;
//                }
//                static bool RunCommand(string command, bool showOutput = true)
//                {
//                    try
//                    {
//                        ProcessStartInfo procInfo = new ProcessStartInfo("cmd.exe", "/c " + command)
//                        {
//                            RedirectStandardOutput = !showOutput,
//                            RedirectStandardError = !showOutput,
//                            UseShellExecute = false,
//                            CreateNoWindow = !showOutput
//                        };

//                        using (Process proc = Process.Start(procInfo))
//                        {
//                            proc.WaitForExit();
//                            return proc.ExitCode == 0;
//                        }
//                    }
//                    catch (Exception ex)
//                    {
//                        Console.WriteLine("Command execution failed: " + ex.Message);
//                        return false;
//                    }
//                }

//            }

//            Console.WriteLine();

//            Console.Write("Enter server database path (e.g., \\\\server\\path\\server.mdb): ");
//            string serverDbPath = Console.ReadLine();
//            Console.WriteLine();

//            // Check if client DB exists, if not pull from server
//            if (!File.Exists(clientDbPath))
//            {
//                Console.WriteLine("Client database not found. Attempting to pull from server...");
//                if (await PullDatabaseFromServer(serverDbPath, clientDbPath))
//                {
//                    Console.WriteLine("Successfully pulled database from server to client.");
//                }
//                else
//                {
//                    Console.WriteLine("\nPress any key to exit...");
//                    Console.ReadKey();
//                    return;
//                }
//            }

//            Console.Write("Enter table name to synchronize (default: Employee): ");
//            string tableName = Console.ReadLine();
//            if (string.IsNullOrEmpty(tableName)) tableName = "Employee";
//            Console.WriteLine();

//            Console.Write("Enter primary key column name (default: ID): ");
//            string pkColumn = Console.ReadLine();
//            if (string.IsNullOrEmpty(pkColumn)) pkColumn = "ID";
//            Console.WriteLine();

//            string syncMetaFile = "sync_metadata.json";

//            ShowGameStyleLoader("Verifying database files", 10);
//            Console.WriteLine();

//            if (!VerifyDatabaseFiles(clientDbPath, serverDbPath))
//            {
//                Console.WriteLine("\nPress any key to exit...");
//                Console.ReadKey();
//                return;
//            }

//            string clientConnStr = $"Provider=Microsoft.ACE.OLEDB.16.0;Data Source={clientDbPath};";
//            string serverConnStr = $"Provider=Microsoft.ACE.OLEDB.16.0;Data Source={serverDbPath};";

//            ShowGameStyleLoader("Testing database connections", 20);
//            if (!TestConnection("Client DB", clientConnStr) || !TestConnection("Server DB", serverConnStr))
//            {
//                Console.WriteLine("\nPress any key to exit...");
//                Console.ReadKey();
//                return;
//            }

//            // Create table if it doesn't exist in both databases
//            ShowGameStyleLoader("Ensuring table structure exists", 30);
//            Console.WriteLine();

//            EnsureTableExists(clientConnStr, tableName, pkColumn, "client");
//            EnsureTableExists(serverConnStr, tableName, pkColumn, "server");

//            // Load last sync times from file
//            ShowGameStyleLoader("Loading synchronization metadata", 10);
//            Console.WriteLine();

//            var metadata = LoadSyncMetadata(syncMetaFile) ?? new SyncMetadata
//            {
//                LastClientSyncTime = GetMaxLastModified(serverConnStr, tableName),
//                LastServerSyncTime = GetMaxLastModified(clientConnStr, tableName)
//            };

//            _lastClientSyncTime = metadata.LastClientSyncTime;
//            _lastServerSyncTime = metadata.LastServerSyncTime;

//            Console.WriteLine("\nStarting continuous synchronization...");
//            Console.WriteLine("Press 'Q' then Enter to stop synchronization.\n");

//            var syncTask = Task.Run(() => ContinuousSync(serverConnStr, clientConnStr, tableName, pkColumn, syncMetaFile));

//            while (_syncRunning)
//            {
//                if (Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Q)
//                {
//                    _syncRunning = false;
//                    Console.WriteLine("Stopping synchronization...");
//                }
//                await Task.Delay(100);
//            }

//            await syncTask;
//            Console.WriteLine("\nSynchronization stopped. Press any key to exit.");
//            Console.CursorVisible = true;
//            Console.ReadKey();
//        }
//        public static bool HasMdbExtension(string path)
//        {
//            if (string.IsNullOrWhiteSpace(path))
//                return false;

//            string extension = Path.GetExtension(path);
//            return extension.Equals(".mdb", StringComparison.OrdinalIgnoreCase);
//        }
//        static async Task<bool> PullDatabaseFromServer(string serverPath, string clientPath)
//        {
//            try
//            {
//                // Extract server IP and share name from the UNC path
//                var serverParts = serverPath.Split(new[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);
//                if (serverParts.Length < 2)
//                {
//                    Console.WriteLine("Invalid server path format. Expected format: \\\\server\\share\\path\\file.mdb");
//                    return false;
//                }

//                string serverIP = serverParts[0];
//                string shareName = serverParts[1];
//                string serverFilePath = string.Join("\\", serverParts.Skip(2));

//                // Check localhost connection
//                if (!PingHost("127.0.0.1"))
//                {
//                    Console.WriteLine("ERROR: Localhost is not responding...");
//                    return false;
//                }
//                Console.WriteLine("Maintaining connection with LocalMachine is successful...");

//                // Check server connection
//                if (!PingHost(serverIP))
//                {
//                    Console.WriteLine($"ERROR: Cannot reach server {serverIP}.");
//                    return false;
//                }
//                Console.WriteLine("Maintaining connection with Server is successful...");

//                // Create destination directory if it doesn't exist
//                string destFolder = Path.GetDirectoryName(clientPath);
//                if (!Directory.Exists(destFolder))
//                {
//                    Directory.CreateDirectory(destFolder);
//                }

//                // Temporary drive letter for mapping
//                string driveLetter = "Z:";

//                // Forcefully disconnect if already mapped
//                await RunCommand($"net use {driveLetter} /delete");

//                // Connect to shared server folder
//                // Note: In a real application, you should securely handle credentials
//                Console.WriteLine("Connecting to server share...");
//                string connectCmd = $"net use {driveLetter} \\\\{serverIP}\\{shareName} /user:administrator RahulJain@1234";
//                if (!await RunCommand(connectCmd))
//                {
//                    Console.WriteLine("ERROR: Failed to connect to shared folder.");
//                    return false;
//                }

//                // Check if file exists on server
//                string serverFile = $"{driveLetter}\\{serverFilePath}";
//                if (!File.Exists(serverFile))
//                {
//                    Console.WriteLine($"ERROR: File does not exist on server: {serverFilePath}");
//                    await RunCommand($"net use {driveLetter} /delete");
//                    return false;
//                }

//                // Copy from server to local destination path
//                Console.WriteLine("Copying file from server...");
//                try
//                {
//                    File.Copy(serverFile, clientPath, true);
//                    Console.WriteLine($"File successfully copied to: {clientPath}");
//                }
//                catch (Exception ex)
//                {
//                    Console.WriteLine($"ERROR: File copy failed. {ex.Message}");
//                    await RunCommand($"net use {driveLetter} /delete");
//                    return false;
//                }

//                // Disconnect server drive
//                await RunCommand($"net use {driveLetter} /delete");
//                return true;
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"Error pulling database from server: {ex.Message}");
//                return false;
//            }
//        }

//        static bool PingHost(string nameOrAddress)
//        {
//            try
//            {
//                using (var ping = new Ping())
//                {
//                    var reply = ping.Send(nameOrAddress, 2000); // 2 second timeout
//                    return reply.Status == IPStatus.Success;
//                }
//            }
//            catch
//            {
//                return false;
//            }
//        }

//        static async Task<bool> RunCommand(string command)
//        {
//            try
//            {
//                var processInfo = new System.Diagnostics.ProcessStartInfo("cmd.exe", "/c " + command)
//                {
//                    CreateNoWindow = true,
//                    UseShellExecute = false,
//                    RedirectStandardError = true,
//                    RedirectStandardOutput = true
//                };

//                using (var process = System.Diagnostics.Process.Start(processInfo))
//                {
//                    process.WaitForExit();
//                    return process.ExitCode == 0;
//                }
//            }
//            catch
//            {
//                return false;
//            }
//        }

//        // [Rest of your existing methods remain unchanged...]
//        static void ShowGameStyleLoader(string message, int totalSteps)
//        {
//            Console.Write(message + " ");
//            int progressBarWidth = 30;

//            for (int i = 0; i <= totalSteps; i++)
//            {
//                double percentage = (double)i / totalSteps;
//                int filledBars = (int)(percentage * progressBarWidth);
//                string bar = new string('█', filledBars).PadRight(progressBarWidth, '-');

//                Console.Write($"\r{message} [{bar}] {percentage * 100:0}%");
//                Console.ForegroundColor = ConsoleColor.Green;
//                int delay = message.Contains("connection") ? 50 :
//                           message.Contains("metadata") ? 30 :
//                           message.Contains("structure") ? 70 : 20;
//                Thread.Sleep(delay);
//            }
//            Console.WriteLine();
//        }

//        static void EnsureTableExists(string connectionString, string tableName, string pkColumn, string system)
//        {
//            try
//            {
//                using var conn = new OleDbConnection(connectionString);
//                conn.Open();

//                DataTable tables = conn.GetSchema("Tables");
//                bool tableExists = false;

//                foreach (DataRow row in tables.Rows)
//                {
//                    if (row["TABLE_NAME"].ToString().Equals(tableName, StringComparison.OrdinalIgnoreCase))
//                    {
//                        tableExists = true;
//                        break;
//                    }
//                }

//                if (!tableExists)
//                {
//                    Console.WriteLine($"Table '{tableName}' not found. Creating...");

//                    using var cmd = new OleDbCommand($@"
//                    CREATE TABLE [{tableName}] (
//                        [{pkColumn}] GUID PRIMARY KEY,
//                        [Name] TEXT(255),
//                        [LastModified] DATETIME DEFAULT Now(),
//                        [Notes] MEMO
//                    )", conn);

//                    cmd.ExecuteNonQuery();
//                    Console.WriteLine("Table created successfully.");
//                }
//                else
//                {
//                    Console.WriteLine($"Table '{tableName}' already exists in {system}.");
//                }
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"\nError ensuring table exists: {ex.Message}");
//            }
//        }
//        static SyncMetadata LoadSyncMetadata(string path)
//        {
//            if (!File.Exists(path)) return null;
//            var json = File.ReadAllText(path);
//            return JsonSerializer.Deserialize<SyncMetadata>(json);
//        }

//        static void SaveSyncMetadata(string path, SyncMetadata metadata)
//        {
//            var json = JsonSerializer.Serialize(metadata, new JsonSerializerOptions { WriteIndented = true });
//            File.WriteAllText(path, json);
//        }

//        static bool VerifyDatabaseFiles(string clientPath, string serverPath)
//        {
//            if (!File.Exists(clientPath))
//            {
//                Console.WriteLine($"\nClient database not found at: {clientPath}");
//                return false;
//            }

//            if (!File.Exists(serverPath))
//            {
//                Console.WriteLine($"\nServer database not found at: {serverPath}");
//                return false;
//            }

//            return true;
//        }

//        static bool TestConnection(string name, string connectionString)
//        {
//            try
//            {
//                using var connection = new OleDbConnection(connectionString);
//                connection.Open();
//                return true;
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"\n{name} connection failed: {ex.Message}");
//                return false;
//            }
//        }

//        static DateTime GetMaxLastModified(string connectionString, string tableName)
//        {
//            try
//            {
//                using var conn = new OleDbConnection(connectionString);
//                conn.Open();
//                using var cmd = new OleDbCommand($"SELECT MAX(LastModified) FROM [{tableName}]", conn);
//                var result = cmd.ExecuteScalar();
//                if (result != DBNull.Value && result != null)
//                    return Convert.ToDateTime(result);
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"\nError fetching max LastModified: {ex.Message}");
//            }
//            return DateTime.MinValue;
//        }

//        static async Task ContinuousSync(string serverConnStr, string clientConnStr, string tableName, string pkColumn, string syncMetaFile)
//        {
//            while (_syncRunning)
//            {
//                try
//                {
//                    using var serverConn = new OleDbConnection(serverConnStr);
//                    using var clientConn = new OleDbConnection(clientConnStr);
//                    serverConn.Open();
//                    clientConn.Open();

//                    Console.WriteLine($"[{DateTime.Now:T}] Syncing Server → Client...");
//                    int serverToClientChanges = SyncDirection(
//                        sourceConn: serverConn,
//                        targetConn: clientConn,
//                        tableName: tableName,
//                        lastSyncTime: ref _lastServerSyncTime,
//                        isServerToClient: true,
//                        pkColumn: pkColumn);
//                    Console.WriteLine($"[{DateTime.Now:T}] Server → Client: {serverToClientChanges} changes applied");

//                    Console.WriteLine($"[{DateTime.Now:T}] Syncing Client → Server...");
//                    int clientToServerChanges = SyncDirection(
//                        sourceConn: clientConn,
//                        targetConn: serverConn,
//                        tableName: tableName,
//                        lastSyncTime: ref _lastClientSyncTime,
//                        isServerToClient: false,
//                        pkColumn: pkColumn);
//                    Console.WriteLine($"[{DateTime.Now:T}] Client → Server: {clientToServerChanges} changes applied");

//                    // Save updated sync times
//                    SaveSyncMetadata(syncMetaFile, new SyncMetadata
//                    {
//                        LastClientSyncTime = _lastClientSyncTime,
//                        LastServerSyncTime = _lastServerSyncTime
//                    });

//                    // Wait 10 seconds before running deletion sync
//                    await Task.Delay(10000);

//                    // Deletion sync: Remove missing IDs from both sides
//                    Console.WriteLine($"[{DateTime.Now:T}] Checking for deletions...");
//                    SyncDeletionsByComparison(serverConn, clientConn, tableName, pkColumn); // server → client
//                    SyncDeletionsByComparison(clientConn, serverConn, tableName, pkColumn); // client → server
//                }
//                catch (Exception ex)
//                {
//                    Console.WriteLine($"[{DateTime.Now:T}] Sync error: {ex.Message}");
//                }

//                await Task.Delay(5000); // Wait before next sync cycle
//            }
//        }

//        static void SyncDeletionsByComparison(OleDbConnection sourceConn, OleDbConnection targetConn, string tableName, string pkColumn)
//        {
//            var sourceIds = GetAllIds(sourceConn, tableName, pkColumn);
//            var targetIds = GetAllIds(targetConn, tableName, pkColumn);

//            var idsToDelete = targetIds.Except(sourceIds);

//            foreach (var id in idsToDelete)
//            {
//                string deleteQuery = $"DELETE FROM [{tableName}] WHERE [{pkColumn}] = ?";
//                using var cmd = new OleDbCommand(deleteQuery, targetConn);
//                cmd.Parameters.AddWithValue($"@{pkColumn}", id);
//                int result = cmd.ExecuteNonQuery();
//                if (result > 0)
//                    Console.WriteLine($"Deleted ID {id} from target (not present in source)");
//            }
//        }

//        static int SyncDirection(OleDbConnection sourceConn, OleDbConnection targetConn,
//                          string tableName, ref DateTime lastSyncTime,
//                          bool isServerToClient, string pkColumn)
//        {
//            int changesApplied = 0;
//            DateTime maxTimestamp = lastSyncTime;

//            string getChangesQuery = $@"
//        SELECT * FROM [{tableName}]
//        WHERE LastModified > ?
//        ORDER BY LastModified";

//            using var cmd = new OleDbCommand(getChangesQuery, sourceConn);
//            cmd.Parameters.AddWithValue("@LastModified", lastSyncTime);

//            using var reader = cmd.ExecuteReader();
//            while (reader.Read())
//            {
//                var row = new Dictionary<string, object>();
//                for (int i = 0; i < reader.FieldCount; i++)
//                {
//                    row[reader.GetName(i)] = reader.IsDBNull(i) ? null : reader.GetValue(i);
//                }

//                if (ApplyChangeWithConflictResolution(targetConn, tableName, row, isServerToClient, pkColumn))
//                {
//                    changesApplied++;
//                    var rowTimestamp = Convert.ToDateTime(row["LastModified"]);
//                    if (rowTimestamp > maxTimestamp)
//                        maxTimestamp = rowTimestamp;
//                }
//            }

//            lastSyncTime = maxTimestamp;
//            return changesApplied;
//        }

//        static bool ApplyChangeWithConflictResolution(OleDbConnection targetConn,
//                                                    string tableName,
//                                                    Dictionary<string, object> row,
//                                                    bool isServerToClient,
//                                                    string pkColumn)
//        {
//            var pkValue = row[pkColumn];
//            var incomingLastModified = Convert.ToDateTime(row["LastModified"]);

//            bool exists = RecordExists(targetConn, tableName, pkColumn, pkValue);
//            if (!exists)
//                return InsertRecord(targetConn, tableName, row);

//            var targetLastModified = GetLastModified(targetConn, tableName, pkColumn, pkValue);
//            var targetRecord = GetRecord(targetConn, tableName, pkColumn, pkValue);

//            // Simple conflict resolution - server wins
//            if (isServerToClient)
//            {
//                bool dataIsDifferent = !row["Name"].Equals(targetRecord["Name"]);
//                if (dataIsDifferent)
//                {
//                    Console.WriteLine($"Overwriting client data for ID {pkValue} with server version");
//                }
//                return UpdateRecord(targetConn, tableName, row, pkColumn);
//            }
//            else
//            {
//                // For client to server, only update if client has newer version
//                if (incomingLastModified > targetLastModified)
//                {
//                    return UpdateRecord(targetConn, tableName, row, pkColumn);
//                }
//                return false;
//            }
//        }

//        static List<Guid> GetAllIds(OleDbConnection conn, string tableName, string pkColumn)
//        {
//            var ids = new List<Guid>();
//            string query = $"SELECT [{pkColumn}] FROM [{tableName}]";

//            using var cmd = new OleDbCommand(query, conn);
//            using var reader = cmd.ExecuteReader();
//            while (reader.Read())
//            {
//                ids.Add(reader.GetGuid(0));
//            }
//            return ids;
//        }

//        static Dictionary<string, object> GetRecord(OleDbConnection conn, string tableName, string pkColumn, object pkValue)
//        {
//            var record = new Dictionary<string, object>();
//            string query = $"SELECT * FROM [{tableName}] WHERE [{pkColumn}] = ?";

//            using var cmd = new OleDbCommand(query, conn);
//            cmd.Parameters.AddWithValue($"@{pkColumn}", pkValue);

//            using var reader = cmd.ExecuteReader();
//            if (reader.Read())
//            {
//                for (int i = 0; i < reader.FieldCount; i++)
//                    record[reader.GetName(i)] = reader.IsDBNull(i) ? null : reader.GetValue(i);
//            }

//            return record;
//        }

//        static bool UpdateRecord(OleDbConnection conn, string tableName, Dictionary<string, object> row, string pkColumn)
//        {
//            var columns = row.Keys.Where(k => k != pkColumn).ToList();
//            var updateSet = string.Join(", ", columns.Select(c => $"[{c}] = ?"));
//            string updateQuery = $@"UPDATE [{tableName}] SET {updateSet} WHERE [{pkColumn}] = ?";

//            using var cmd = new OleDbCommand(updateQuery, conn);
//            foreach (var col in columns)
//                cmd.Parameters.AddWithValue($"@{col}", row[col] ?? DBNull.Value);
//            cmd.Parameters.AddWithValue($"@{pkColumn}", row[pkColumn]);

//            return cmd.ExecuteNonQuery() > 0;
//        }

//        static bool InsertRecord(OleDbConnection conn, string tableName, Dictionary<string, object> row)
//        {
//            var columns = row.Keys.ToList();
//            var columnList = string.Join(", ", columns.Select(c => $"[{c}]"));
//            var valuePlaceholders = string.Join(", ", columns.Select(_ => "?"));

//            string insertQuery = $@"INSERT INTO [{tableName}] ({columnList}) VALUES ({valuePlaceholders})";

//            using var cmd = new OleDbCommand(insertQuery, conn);
//            foreach (var col in columns)
//                cmd.Parameters.AddWithValue($"@{col}", row[col] ?? DBNull.Value);

//            return cmd.ExecuteNonQuery() > 0;
//        }

//        static bool RecordExists(OleDbConnection conn, string tableName, string pkColumn, object pkValue)
//        {
//            string query = $"SELECT COUNT(*) FROM [{tableName}] WHERE [{pkColumn}] = ?";
//            using var cmd = new OleDbCommand(query, conn);
//            cmd.Parameters.AddWithValue($"@{pkColumn}", pkValue);
//            return (int)cmd.ExecuteScalar() > 0;
//        }

//        static DateTime GetLastModified(OleDbConnection conn, string tableName, string pkColumn, object pkValue)
//        {
//            string query = $"SELECT LastModified FROM [{tableName}] WHERE [{pkColumn}] = ?";
//            using var cmd = new OleDbCommand(query, conn);
//            cmd.Parameters.AddWithValue($"@{pkColumn}", pkValue);
//            var result = cmd.ExecuteScalar();
//            return (result != DBNull.Value && result != null) ? Convert.ToDateTime(result) : DateTime.MinValue;
//        }

//        static DateTime GetSafeFutureTimestamp()
//        {
//            var now = DateTime.Now;
//            return new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second).AddSeconds(5);
//        }

//        static string AddConflictNote(Dictionary<string, object> record, string note)
//        {
//            if (record.ContainsKey("Notes") && record["Notes"] != null)
//            {
//                return record["Notes"].ToString() + "; " + note;
//            }
//            return note;
//        }

//        static bool IsConflictResolvedRecord(Dictionary<string, object> record)
//        {
//            return record.ContainsKey("Notes") &&
//                   record["Notes"] != null &&
//                   record["Notes"].ToString().Contains("CONFLICT_RESOLVED");
//        }
//    }
//}
