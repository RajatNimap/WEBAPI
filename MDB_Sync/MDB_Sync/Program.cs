using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ContinuousAccessDbSync
{
    class Program
    {
        private static bool _syncRunning = true;
        private static string _lastClientSyncTime;
        private static string _lastServerSyncTime;

        static async Task Main()
        {
            string clientDbPath = @"C:\Users\Nimap\Documents\mdbfile\rajat.mdb";
            string serverDbPath = @"\\192.168.1.87\MDB\suraj.mdb";
            const string tableName = "People";

            // Verify file existence
            if (!VerifyDatabaseFiles(clientDbPath, serverDbPath))
            {
                Console.ReadKey();
                return;
            }

            string clientConnStr = $"Provider=Microsoft.ACE.OLEDB.16.0;Data Source={clientDbPath};";
            string serverConnStr = $"Provider=Microsoft.ACE.OLEDB.16.0;Data Source={serverDbPath};";

            // Initial connection tests
            if (!TestConnection("Client DB", clientConnStr) || !TestConnection("Server DB", serverConnStr))
            {
                Console.ReadKey();
                return;
            }

            // Initialize last sync times
            _lastClientSyncTime = GetCurrentDatabaseTime(clientConnStr);
            _lastServerSyncTime = GetCurrentDatabaseTime(serverConnStr);

            Console.WriteLine("\nStarting continuous synchronization...");
            Console.WriteLine("Press 'Q' then Enter to stop synchronization.\n");

            // Start the sync loop in background
            var syncTask = Task.Run(() => ContinuousSync(serverConnStr, clientConnStr, tableName));

            // Monitor for 'Q' key press
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
                using (var connection = new OleDbConnection(connectionString))
                {
                    connection.Open();
                    Console.WriteLine($"{name} connection successful.");
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{name} connection failed: {ex.Message}");
                if (ex is OleDbException oleEx)
                {
                    foreach (OleDbError error in oleEx.Errors)
                    {
                        Console.WriteLine($"  OleDb Error: {error.Message}");
                    }
                }
                return false;
            }
        }

        static string GetCurrentDatabaseTime(string connectionString)
        {
            try
            {
                using (var conn = new OleDbConnection(connectionString))
                {
                    conn.Open();
                    using (var cmd = new OleDbCommand("SELECT MAX(LastModified) FROM People", conn))
                    {
                        var result = cmd.ExecuteScalar();
                        return result != DBNull.Value ? result.ToString() : DateTime.MinValue.ToString();
                    }
                }
            }
            catch
            {
                return DateTime.MinValue.ToString();
            }
        }

        static async Task ContinuousSync(string serverConnStr, string clientConnStr, string tableName)
        {
            while (_syncRunning)
            {
                try
                {
                    // Sync from server to client
                    Console.WriteLine($"[{DateTime.Now:T}] Syncing Server → Client...");
                    var serverChanges = SyncDirection(serverConnStr, clientConnStr, tableName, ref _lastServerSyncTime);
                    Console.WriteLine($"[{DateTime.Now:T}] Server → Client: {serverChanges} changes applied");

                    // Sync from client to server
                    Console.WriteLine($"[{DateTime.Now:T}] Syncing Client → Server...");
                    var clientChanges = SyncDirection(clientConnStr, serverConnStr, tableName, ref _lastClientSyncTime);
                    Console.WriteLine($"[{DateTime.Now:T}] Client → Server: {clientChanges} changes applied");

                    //    // Display current state
                    //    DisplayCurrentRecords(clientConnStr, "Client Database");
                    //    DisplayCurrentRecords(serverConnStr, "Server Database");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[{DateTime.Now:T}] Sync error: {ex.Message}");
                }

                // Wait 5 seconds before next sync
                await Task.Delay(5000);
            }
        }

        static int SyncDirection(string sourceConnStr, string targetConnStr, string tableName, ref string lastSyncTime)
        {
            int changesApplied = 0;

            using (var sourceConn = new OleDbConnection(sourceConnStr))
            using (var targetConn = new OleDbConnection(targetConnStr))
            {
                sourceConn.Open();
                targetConn.Open();

                // Get changes since last sync
                string getChangesQuery = $@"
                    SELECT * FROM [{tableName}] 
                    WHERE LastModified > #{lastSyncTime}# 
                    ORDER BY LastModified";

                using (var cmd = new OleDbCommand(getChangesQuery, sourceConn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var row = new Dictionary<string, object>();
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            row[reader.GetName(i)] = reader.IsDBNull(i) ? null : reader.GetValue(i);
                        }

                        // Apply change to target
                        if (ApplyChange(targetConn, tableName, row))
                        {
                            changesApplied++;
                            // Update last sync time to this record's timestamp
                            lastSyncTime = row["LastModified"].ToString();
                        }
                    }
                }
            }

            return changesApplied;
        }

        static bool ApplyChange(OleDbConnection targetConn, string tableName, Dictionary<string, object> row)
        {
            string pkColumn = "ID";
            bool exists = RecordExists(targetConn, tableName, pkColumn, row[pkColumn]);

            var columns = row.Keys.Where(k => k != pkColumn).ToList();
            var columnList = string.Join(", ", columns.Select(c => $"[{c}]"));
            var valuePlaceholders = string.Join(", ", columns.Select(_ => "?"));
            if (!columns.Contains("LastModified"))
            {
                columns.Add("LastModified");
                row["LastModified"] = DateTime.Now;
            }

            var updateSet = string.Join(", ", columns.Select(c => $"[{c}] = ?"));

            if (exists)
            {
                // Update existing record
                string updateQuery = $@"
                    UPDATE [{tableName}] 
                    SET {updateSet}
                    WHERE [{pkColumn}] = ?";

                using (var cmd = new OleDbCommand(updateQuery, targetConn))
                {
                    foreach (var col in columns)
                    {
                        cmd.Parameters.AddWithValue($"@{col}", row[col] ?? DBNull.Value);
                    }
                    cmd.Parameters.AddWithValue($"@{pkColumn}", row[pkColumn]);

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            else
            {
                // Insert new record
                string insertQuery = $@"
                    INSERT INTO [{tableName}] ({columnList}) 
                    VALUES ({valuePlaceholders})";

                using (var cmd = new OleDbCommand(insertQuery, targetConn))
                {
                    foreach (var col in columns)
                    {
                        cmd.Parameters.AddWithValue($"@{col}", row[col] ?? DBNull.Value);
                    }

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        static bool RecordExists(OleDbConnection conn, string tableName, string pkColumn, object pkValue)
        {
            string query = $"SELECT COUNT(*) FROM [{tableName}] WHERE [{pkColumn}] = ?";
            using (var cmd = new OleDbCommand(query, conn))
            {
                cmd.Parameters.AddWithValue($"@{pkColumn}", pkValue);
                return (int)cmd.ExecuteScalar() > 0;
            }
        }

        static void DisplayCurrentRecords(string connectionString, string dbName)
        {
            Console.WriteLine($"\nCurrent records in {dbName}:");

            try
            {
                using (var conn = new OleDbConnection(connectionString))
                {
                    conn.Open();
                    using (var cmd = new OleDbCommand("SELECT ID, Name, LastModified FROM People ORDER BY ID", conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Console.WriteLine($"  ID: {reader["ID"]}, Name: {reader["Name"]}, Last Modified: {reader["LastModified"]}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  Error displaying records: {ex.Message}");
            }
        }
    }
}