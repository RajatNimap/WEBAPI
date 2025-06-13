using System.Collections.Generic;
using System.Data.OleDb;
using System.Text.Json;

namespace RobustAccessDbSync
{
    class SyncMetadata
    {
        public DateTime LastClientSyncTime { get; set; }
        public DateTime LastServerSyncTime { get; set; }
    }

    class Program
    {
        private static bool _syncRunning = true;
        private static DateTime _lastClientSyncTime;
        private static DateTime _lastServerSyncTime;
        private const string ConflictSuffix = "_CONFLICT_RESOLVED";
        private static string TimestampColumnName;
        private static string clientDbPath;
        private static string serverDbPath;
        private static string ID;

        [System.Runtime.Versioning.SupportedOSPlatform("windows")]
        static async Task Main()
        {
            string clientDbPath = @"C:\Users\Nimap\Documents\mdbfile\rajat.mdb";
            string serverDbPath = @"\\192.168.1.87\MDB\suraj.mdb";
            const string tableName = "Employee";
            const string pkColumn = "ID";
            string syncMetaFile = "sync_metadata.json";

            if (!VerifyDatabaseFiles(clientDbPath, serverDbPath))
            {
                Console.ReadKey();
                return;
            }

            string clientConnStr = $"Provider=Microsoft.ACE.OLEDB.16.0;Data Source={clientDbPath};";
            string serverConnStr = $"Provider=Microsoft.ACE.OLEDB.16.0;Data Source={serverDbPath};";

            if (!TestConnection("Client DB", clientConnStr) || !TestConnection("Server DB", serverConnStr))
            {
                Console.ReadKey();
                return;
            }

            // Load last sync times from file
            var metadata = LoadSyncMetadata(syncMetaFile) ?? new SyncMetadata
            {
                LastClientSyncTime = GetMaxLastModified(serverConnStr, tableName),
                LastServerSyncTime = GetMaxLastModified(clientConnStr, tableName)
            };

            _lastClientSyncTime = metadata.LastClientSyncTime;
            _lastServerSyncTime = metadata.LastServerSyncTime;

            Console.WriteLine("\nStarting continuous synchronization...");
            Console.WriteLine("Press 'Q' then Enter to stop synchronization.\n");

            var syncTask = Task.Run(() => ContinuousSync(serverConnStr, clientConnStr, tableName, pkColumn, syncMetaFile));

            while (_syncRunning)
            {
                if (Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Q)
                {
                    _syncRunning = false;
                    Console.WriteLine("Stopping synchronization...");
                }
                await Task.Delay(100);
            }

            await syncTask;
            Console.WriteLine("Synchronization stopped. Press any key to exit.");
            Console.ReadKey();
        }

        static SyncMetadata LoadSyncMetadata(string path)
        {
            if (!File.Exists(path)) return null;
            var json = File.ReadAllText(path);
            return JsonSerializer.Deserialize<SyncMetadata>(json);
        }

        static void SaveSyncMetadata(string path, SyncMetadata metadata)
        {
            var json = JsonSerializer.Serialize(metadata, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(path, json);
        }

        static bool VerifyDatabaseFiles(string clientPath, string serverPath)
        {
            if (!File.Exists(clientPath))
            {
                Console.WriteLine($"Client database not found at: {clientPath}");
                return false;
            }

            if (!File.Exists(serverPath))
            {
                Console.WriteLine($"Server database not found at: {serverPath}");
                return false;
            }

            return true;
        }

        static bool TestConnection(string name, string connectionString)
        {
            Console.WriteLine($"Testing {name} connection...");
            try
            {
                using var connection = new OleDbConnection(connectionString);
                connection.Open();
                Console.WriteLine($"{name} connection successful.");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{name} connection failed: {ex.Message}");
                return false;
            }
        }

        static DateTime GetMaxLastModified(string connectionString, string tableName)
        {
            try
            {
                using var conn = new OleDbConnection(connectionString);
                conn.Open();
                using var cmd = new OleDbCommand($"SELECT MAX(LastModified) FROM [{tableName}]", conn);
                var result = cmd.ExecuteScalar();
                if (result != DBNull.Value && result != null)
                    return Convert.ToDateTime(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching max LastModified: {ex.Message}");
            }
            return DateTime.MinValue;
        }

        static async Task ContinuousSync(string serverConnStr, string clientConnStr, string tableName, string pkColumn, string syncMetaFile)
        {
            while (_syncRunning)
            {
                try
                {
                    using var serverConn = new OleDbConnection(serverConnStr);
                    using var clientConn = new OleDbConnection(clientConnStr);
                    serverConn.Open();
                    clientConn.Open();

                    Console.WriteLine($"[{DateTime.Now:T}] Syncing Server → Client...");
                    int serverToClientChanges = SyncDirection(
                        sourceConn: serverConn,
                        targetConn: clientConn,
                        tableName: tableName,
                        lastSyncTime: ref _lastServerSyncTime,
                        isServerToClient: true,
                        pkColumn: pkColumn);
                    Console.WriteLine($"[{DateTime.Now:T}] Server → Client: {serverToClientChanges} changes applied");

                    Console.WriteLine($"[{DateTime.Now:T}] Syncing Client → Server...");
                    int clientToServerChanges = SyncDirection(
                        sourceConn: clientConn,
                        targetConn: serverConn,
                        tableName: tableName,
                        lastSyncTime: ref _lastClientSyncTime,
                        isServerToClient: false,
                        pkColumn: pkColumn);
                    Console.WriteLine($"[{DateTime.Now:T}] Client → Server: {clientToServerChanges} changes applied");

                    // Save updated sync times
                    SaveSyncMetadata(syncMetaFile, new SyncMetadata
                    {
                        LastClientSyncTime = _lastClientSyncTime,
                        LastServerSyncTime = _lastServerSyncTime
                    });

                    // Wait 10 seconds before running deletion sync
                    await Task.Delay(10000);

                    // Deletion sync: Remove missing IDs from both sides
                    Console.WriteLine($"[{DateTime.Now:T}] Checking for deletions...");
                    SyncDeletionsByComparison(serverConn, clientConn, tableName, pkColumn); // server → client
                    SyncDeletionsByComparison(clientConn, serverConn, tableName, pkColumn); // client → server
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[{DateTime.Now:T}] Sync error: {ex.Message}");
                }

                await Task.Delay(5000); // Wait before next sync cycle
            }
        }
     

        static void SyncDeletionsByComparison(OleDbConnection sourceConn, OleDbConnection targetConn, string tableName, string pkColumn)
        {
            var sourceIds = GetAllIds(sourceConn, tableName, pkColumn);
            var targetIds = GetAllIds(targetConn, tableName, pkColumn);

            var idsToDelete = targetIds.Except(sourceIds);

            foreach (var id in idsToDelete)
            {
                string deleteQuery = $"DELETE FROM [{tableName}] WHERE [{pkColumn}] = ?";
                using var cmd = new OleDbCommand(deleteQuery, targetConn);
                cmd.Parameters.AddWithValue($"@{pkColumn}", id);
                int result = cmd.ExecuteNonQuery();
                if (result > 0)
                    Console.WriteLine($"Deleted ID {id} from target (not present in source)");
            }
        }


        static int SyncDirection(OleDbConnection sourceConn, OleDbConnection targetConn,
                          string tableName, ref DateTime lastSyncTime,
                          bool isServerToClient, string pkColumn)
        {
            int changesApplied = 0;
            DateTime maxTimestamp = lastSyncTime;

            string getChangesQuery = $@"
            SELECT * FROM [{tableName}]
            WHERE LastModified > ?
            ORDER BY LastModified";

            using var cmd = new OleDbCommand(getChangesQuery, sourceConn);
            cmd.Parameters.AddWithValue("@LastModified", lastSyncTime);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var row = new Dictionary<string, object>();
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    row[reader.GetName(i)] = reader.IsDBNull(i) ? null : reader.GetValue(i);
                }

                if (ApplyChangeWithConflictResolution(targetConn,sourceConn ,tableName, row, isServerToClient, pkColumn))
                {
                    changesApplied++;
                    var rowTimestamp = Convert.ToDateTime(row["LastModified"]);
                    if (rowTimestamp > maxTimestamp)
                        maxTimestamp = rowTimestamp;
                }
            }

            lastSyncTime = maxTimestamp;
            return changesApplied;
        }

        static bool ApplyChangeWithConflictResolution(OleDbConnection targetConn,OleDbConnection sourceConn,
                                                    string tableName,
                                                    Dictionary<string, object> row,
                                                    bool isServerToClient,
                                                    string pkColumn)
        {
            var pkValue = row[pkColumn]; // This is a GUID
            var incomingLastModified = Convert.ToDateTime(row["LastModified"]);
          

            bool exists = RecordExists(targetConn, tableName, pkColumn, pkValue);
            if (!exists)
                return InsertRecord(targetConn, tableName, row);

            var targetLastModified = GetLastModified(targetConn, tableName, pkColumn, pkValue);
            var targetRecord = GetRecord(targetConn, tableName, pkColumn, pkValue);
            var ClientRecord = GetRecord(sourceConn, tableName, pkColumn, pkValue);
            var ClinetLastModified = GetLastModified(sourceConn, tableName, pkColumn, pkValue);
            var clientrow = GetRecordForCopy(sourceConn, tableName);
            var serverrow = GetRecordForCopy(targetConn, tableName);


            if (targetRecord == null) {
                return InsertRecord(targetConn, tableName, row);
            }

            if(ClientRecord == null)
            {
                return InsertRecord(sourceConn, tableName, row);

            }


            // Simple conflict resolution - server wins
            if (isServerToClient)
            {
                bool dataIsDifferent = !row["Name"].Equals(targetRecord["Name"]);
                if (dataIsDifferent)
                {
                    Console.WriteLine($"Overwriting client data for ID {pkValue} with server version");
                }
                return UpdateRecord(targetConn, tableName, row, pkColumn);
            }
            else
            {
                // For client to server, only update if client has newer version
                if (incomingLastModified > targetLastModified)
                {
                    return UpdateRecord(targetConn, tableName, row, pkColumn);
                }
                return false;
            }
        }

        static List<Guid> GetAllIds(OleDbConnection conn, string tableName, string pkColumn)
        {
            var ids = new List<Guid>();
            string query = $"SELECT [{pkColumn}] FROM [{tableName}]";

            using var cmd = new OleDbCommand(query, conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var id = reader.GetValue(0);
                if (id is Guid guid) ids.Add(guid);
                else if (Guid.TryParse(id.ToString(), out var parsed)) ids.Add(parsed);

            }
            return ids;
        }

        //static void SyncDeletionsByComparison(OleDbConnection sourceConn, OleDbConnection targetConn,
        //                                    string tableName, string pkColumn)
        //{
        //    var sourceIds = GetAllIds(sourceConn, tableName, pkColumn);
        //    var targetIds = GetAllIds(targetConn, tableName, pkColumn);

        //    var idsToDelete = targetIds.Except(sourceIds);

        //    foreach (var id in idsToDelete)
        //    {
        //        string deleteQuery = $"DELETE FROM [{tableName}] WHERE [{pkColumn}] = ?";
        //        using var cmd = new OleDbCommand(deleteQuery, targetConn);
        //        cmd.Parameters.AddWithValue($"@{pkColumn}", id);
        //        int result = cmd.ExecuteNonQuery();
        //        if (result > 0)
        //            Console.WriteLine($"Deleted ID {id} from target (not present in source)");
        //    }
        //}

        //static bool ApplyChangeWithConflictResolution(OleDbConnection targetConn,
        //                                              string tableName,
        //                                              Dictionary<string, object> row,
        //                                              bool isServerToClient,
        //                                              string pkColumn)
        //{
        //    var pkValue = row[pkColumn];
        //    var incomingLastModified = Convert.ToDateTime(row["LastModified"]);

        //    bool exists = RecordExists(targetConn, tableName, pkColumn, pkValue);
        //    if (!exists)
        //        return InsertRecord(targetConn, tableName, row);

        //    var targetLastModified = GetLastModified(targetConn, tableName, pkColumn, pkValue);
        //    var targetRecord = GetRecord(targetConn, tableName, pkColumn, pkValue);

        //    if (isServerToClient)
        //    {
        //        bool dataIsDifferent = !row["Name"].Equals(targetRecord["Name"]);
        //        if (dataIsDifferent && !IsConflictResolvedRecord(targetRecord))
        //        {
        //            int newId = GetNextAvailableId(targetConn, tableName, pkColumn);
        //            targetRecord[pkColumn] = newId;
        //            targetRecord["Notes"] = AddConflictNote(targetRecord,
        //                $"Preserved client data (original ID: {pkValue}) before server overwrite");
        //            targetRecord["LastModified"] = GetSafeFutureTimestamp();

        //            InsertRecord(targetConn, tableName, targetRecord);
        //            Console.WriteLine($"Preserved client data for ID {pkValue} as new ID {newId}");
        //        }

        //        return UpdateRecord(targetConn, tableName, row, pkColumn);
        //    }
        //    else
        //    {
        //        Console.WriteLine($"Skipped client record for ID {pkValue}; server version retained");
        //        return false;
        //    }
        //}

        static DateTime GetSafeFutureTimestamp()
        {
            var now = DateTime.Now;
            return new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second).AddSeconds(5);
        }

        static string AddConflictNote(Dictionary<string, object> record, string note)
        {
            if (record.ContainsKey("Notes") && record["Notes"] != null)
            {
                return record["Notes"].ToString() + "; " + note;
            }
            return note;
        }

        static bool IsConflictResolvedRecord(Dictionary<string, object> record)
        {
            return record.ContainsKey("Notes") &&
                   record["Notes"] != null &&
                   record["Notes"].ToString().Contains("CONFLICT_RESOLVED");
        }

        static Dictionary<string, object> GetRecord(OleDbConnection conn, string tableName, string pkColumn, object pkValue)
        {
            var record = new Dictionary<string, object>();
            string query = $"SELECT * FROM [{tableName}] WHERE [{pkColumn}] = ?";

            using var cmd = new OleDbCommand(query, conn);
            cmd.Parameters.AddWithValue($"@{pkColumn}", pkValue);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                for (int i = 0; i < reader.FieldCount; i++)
                    record[reader.GetName(i)] = reader.IsDBNull(i) ? null : reader.GetValue(i);
            }

            return record;
        }
        static Dictionary<string, object> GetRecordForCopy(OleDbConnection conn, string tableName)
        {
            var record = new Dictionary<string, object>();
            string query = $"SELECT * FROM [{tableName}]";

            using var cmd = new OleDbCommand(query, conn);
           
            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                for (int i = 0; i < reader.FieldCount; i++)
                    record[reader.GetName(i)] = reader.IsDBNull(i) ? null : reader.GetValue(i);
            }

            return record;
        }

        static bool UpdateRecord(OleDbConnection conn, string tableName, Dictionary<string, object> row, string pkColumn)
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

        static bool InsertRecord(OleDbConnection conn, string tableName, Dictionary<string, object> row)
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

        static int GetNextAvailableId(OleDbConnection conn, string tableName, string pkColumn)
        {
            string query = $"SELECT MAX([{pkColumn}]) FROM [{tableName}]";
            using var cmd = new OleDbCommand(query, conn);
            var result = cmd.ExecuteScalar();
            int maxId = (result != DBNull.Value && result != null) ? Convert.ToInt32(result) : 0;
            return maxId + 1;
        }

        static bool RecordExists(OleDbConnection conn, string tableName, string pkColumn, object pkValue)
        {
            string query = $"SELECT COUNT(*) FROM [{tableName}] WHERE [{pkColumn}] = ?";
            using var cmd = new OleDbCommand(query, conn);
            cmd.Parameters.AddWithValue($"@{pkColumn}", pkValue);
            return (int)cmd.ExecuteScalar() > 0;
        }

        static DateTime GetLastModified(OleDbConnection conn, string tableName, string pkColumn, object pkValue)
        {
            string query = $"SELECT LastModified FROM [{tableName}] WHERE [{pkColumn}] = ?";
            using var cmd = new OleDbCommand(query, conn);
            cmd.Parameters.AddWithValue($"@{pkColumn}", pkValue);
            var result = cmd.ExecuteScalar();
            return (result != DBNull.Value && result != null) ? Convert.ToDateTime(result) : DateTime.MinValue;
        }
    }
}
