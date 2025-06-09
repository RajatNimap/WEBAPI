using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ContinuousAccessDbSync
{
    class Program
    {
        private static bool _syncRunning = true;
        private static DateTime _lastClientSyncTime;
        private static DateTime _lastServerSyncTime;

        static async Task Main()
        {
            string clientDbPath = @"C:\Users\Nimap\Documents\mdbfile\rajat.mdb";
            string serverDbPath = @"\\192.168.1.128\mdb\yash.mdb";
            const string tableName = "People";

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

            _lastClientSyncTime = GetMaxLastModified(serverConnStr, tableName);
            _lastServerSyncTime = GetMaxLastModified(clientConnStr, tableName);

            Console.WriteLine("\nStarting continuous synchronization...");
            Console.WriteLine("Press 'Q' then Enter to stop synchronization.\n");

            var syncTask = Task.Run(() => ContinuousSync(serverConnStr, clientConnStr, tableName));

            while (true)
            {
                var input = Console.ReadLine();
                if (string.Equals(input, "Q", StringComparison.OrdinalIgnoreCase))
                {
                    _syncRunning = false;
                    break;
                }
                else
                {
                    Console.WriteLine("Invalid input. Press 'Q' then Enter to stop synchronization.");
                }
            }

            await syncTask;

            Console.WriteLine("Synchronization stopped. Press any key to exit.");
            Console.ReadKey();
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

        static async Task ContinuousSync(string serverConnStr, string clientConnStr, string tableName)
        {
            const string pkColumn = "ID";

            while (_syncRunning)
            {
                try
                {
                    Console.WriteLine($"[{DateTime.Now:T}] Syncing Server → Client...");
                    using var serverConn = new OleDbConnection(serverConnStr);
                    using var clientConn = new OleDbConnection(clientConnStr);
                    serverConn.Open();
                    clientConn.Open();

                    int serverToClientChanges = SyncDirection(serverConnStr, clientConnStr, tableName, ref _lastServerSyncTime);
                    Console.WriteLine($"[{DateTime.Now:T}] Server → Client: {serverToClientChanges} changes applied");

                    Console.WriteLine($"[{DateTime.Now:T}] Syncing Client → Server...");
                    int clientToServerChanges = SyncDirection(clientConnStr, serverConnStr, tableName, ref _lastClientSyncTime);
                    Console.WriteLine($"[{DateTime.Now:T}] Client → Server: {clientToServerChanges} changes applied");

                    SyncDeletions(serverConn, clientConn, tableName, pkColumn);
                    SyncDeletions(clientConn, serverConn, tableName, pkColumn);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[{DateTime.Now:T}] Sync error: {ex.Message}");
                }

                await Task.Delay(5000);
            }
        }

        static int SyncDirection(string sourceConnStr, string targetConnStr, string tableName, ref DateTime lastSyncTime)
        {
            int changesApplied = 0;
            DateTime maxTimestamp = lastSyncTime;

            using var sourceConn = new OleDbConnection(sourceConnStr);
            using var targetConn = new OleDbConnection(targetConnStr);

            sourceConn.Open();
            targetConn.Open();

            string lastSyncFormatted = lastSyncTime.ToString("MM/dd/yyyy hh:mm:ss tt");
            string getChangesQuery = $@"
                SELECT * FROM [{tableName}] 
                WHERE LastModified > ? 
                ORDER BY LastModified";

            using var cmd = new OleDbCommand(getChangesQuery, sourceConn);
            cmd.Parameters.AddWithValue("@LastModified", lastSyncFormatted);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var row = new Dictionary<string, object>();
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    row[reader.GetName(i)] = reader.IsDBNull(i) ? null : reader.GetValue(i);
                }

                if (ApplyChange(targetConn, tableName, row))
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

        static bool ApplyChange(OleDbConnection targetConn, string tableName, Dictionary<string, object> row)
        {
            string pkColumn = "ID";
            var pkValue = row[pkColumn];
            var incomingLastModified = Convert.ToDateTime(row["LastModified"]);

            bool exists = RecordExists(targetConn, tableName, pkColumn, pkValue);

            if (exists)
            {
                var targetLastModified = GetLastModified(targetConn, tableName, pkColumn, pkValue);

                if (incomingLastModified <= targetLastModified)
                    return false;

                if (IsDataDifferent(targetConn, tableName, pkColumn, pkValue, row))
                {
                    // Clone with new ID before update
                    var newRow = new Dictionary<string, object>(row);
                    newRow["ID"] = GetNewId(targetConn, tableName, pkColumn);
                    InsertRow(targetConn, tableName, newRow);
                }

                var columns = row.Keys.Where(k => k != pkColumn).ToList();
                var updateSet = string.Join(", ", columns.Select(c => $"[{c}] = ?"));

                string updateQuery = $@"
                    UPDATE [{tableName}] 
                    SET {updateSet}
                    WHERE [{pkColumn}] = ?";

                using var cmd = new OleDbCommand(updateQuery, targetConn);

                foreach (var col in columns)
                    cmd.Parameters.AddWithValue($"@{col}", row[col] ?? DBNull.Value);

                cmd.Parameters.AddWithValue($"@{pkColumn}", pkValue);

                return cmd.ExecuteNonQuery() > 0;
            }
            else
            {
                InsertRow(targetConn, tableName, row);
                return true;
            }
        }

        static bool IsDataDifferent(OleDbConnection conn, string tableName, string pkColumn, object pkValue, Dictionary<string, object> incomingRow)
        {
            string query = $"SELECT * FROM [{tableName}] WHERE [{pkColumn}] = ?";
            using var cmd = new OleDbCommand(query, conn);
            cmd.Parameters.AddWithValue($"@{pkColumn}", pkValue);

            using var reader = cmd.ExecuteReader();
            if (!reader.Read()) return true;

            for (int i = 0; i < reader.FieldCount; i++)
            {
                string colName = reader.GetName(i);
                if (colName == pkColumn) continue;

                object existingValue = reader.IsDBNull(i) ? null : reader.GetValue(i);
                object incomingValue = incomingRow.ContainsKey(colName) ? incomingRow[colName] : null;

                if (!object.Equals(existingValue, incomingValue))
                    return true;
            }

            return false;
        }

        static void InsertRow(OleDbConnection conn, string tableName, Dictionary<string, object> row)
        {
            var columns = row.Keys.ToList();
            var columnList = string.Join(", ", columns.Select(c => $"[{c}]"));
            var valuePlaceholders = string.Join(", ", columns.Select(_ => "?"));

            string insertQuery = $@"
                INSERT INTO [{tableName}] ({columnList}) 
                VALUES ({valuePlaceholders})";

            using var cmd = new OleDbCommand(insertQuery, conn);
            foreach (var col in columns)
                cmd.Parameters.AddWithValue($"@{col}", row[col] ?? DBNull.Value);

            cmd.ExecuteNonQuery();
        }

        static int GetNewId(OleDbConnection conn, string tableName, string pkColumn)
        {
            string query = $"SELECT MAX([{pkColumn}]) FROM [{tableName}]";
            using var cmd = new OleDbCommand(query, conn);
            var result = cmd.ExecuteScalar();
            return (result != DBNull.Value && result != null) ? Convert.ToInt32(result) + 1 : 1;
        }

        static DateTime GetLastModified(OleDbConnection conn, string tableName, string pkColumn, object pkValue)
        {
            string query = $"SELECT LastModified FROM [{tableName}] WHERE [{pkColumn}] = ?";
            using var cmd = new OleDbCommand(query, conn);
            cmd.Parameters.AddWithValue($"@{pkColumn}", pkValue);
            var result = cmd.ExecuteScalar();
            return (result != DBNull.Value && result != null) ? Convert.ToDateTime(result) : DateTime.MinValue;
        }

        static bool RecordExists(OleDbConnection conn, string tableName, string pkColumn, object pkValue)
        {
            string query = $"SELECT COUNT(*) FROM [{tableName}] WHERE [{pkColumn}] = ?";
            using var cmd = new OleDbCommand(query, conn);
            cmd.Parameters.AddWithValue($"@{pkColumn}", pkValue);
            return (int)cmd.ExecuteScalar() > 0;
        }

        static void SyncDeletions(OleDbConnection sourceConn, OleDbConnection targetConn, string tableName, string pkColumn)
        {
            var sourceIds = GetAllIds(sourceConn, tableName, pkColumn);
            var targetIds = GetAllIds(targetConn, tableName, pkColumn);

            var toDeleteInTarget = targetIds.Except(sourceIds).ToList();

            foreach (var id in toDeleteInTarget)
            {
                string deleteQuery = $"DELETE FROM [{tableName}] WHERE [{pkColumn}] = ?";
                using var cmd = new OleDbCommand(deleteQuery, targetConn);
                cmd.Parameters.AddWithValue($"@{pkColumn}", id);
                cmd.ExecuteNonQuery();
                Console.WriteLine($"Deleted ID {id} from target.");
            }
        }

        static List<int> GetAllIds(OleDbConnection conn, string tableName, string pkColumn)
        {
            var ids = new List<int>();
            string query = $"SELECT [{pkColumn}] FROM [{tableName}]";

            using var cmd = new OleDbCommand(query, conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                ids.Add(reader.GetInt32(0));
            }

            return ids;
        }
    }
}
