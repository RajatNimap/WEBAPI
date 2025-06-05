using System;
using System.Data.OleDb;
using System.Xml.Linq;

class Program
{
    static void Main()
    {
        // 1️⃣ Define Connection Strings
        // Client DB (local)
        string clientDbPath = "C:\\SyncFile\\rajat.mdb";
        string clientConnectionString = $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={clientDbPath};";

        // Server DB (your laptop via IP)
        string serverIp = "192.168.1.87"; // Replace with your server IP
        //string serverDbPath = $"\\{serverIp}\\Shared\\suraj.mdb"; // UNC Path
        //string serverConnectionString = $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={serverDbPath};";



        string serverDbPath = $@"\\{serverIp}\MDB\suraj.mdb";
        //string serverConnectionString = $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={serverDbPath};";
        string serverConnectionString = $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={serverDbPath};";

        //string serverConnectionString = $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=\\\\{serverIp}\\Shared\\suraj.mdb;";

        // 2️⃣ Test Connections
        TestConnection("Client DB", clientConnectionString);
        TestConnection("Server DB", serverConnectionString);

        // 3️⃣ Sync Logic (Example)

        Console.ReadKey();
    }

    // ✅ Test if connection works
    static void TestConnection(string name, string connectionString)
    {
        Console.WriteLine($"Testing {name} connection...");
        try
        {
            using (var connection = new OleDbConnection(connectionString))
            {
                connection.Open();
                Console.WriteLine($"{name} connection successful.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"{name} connection failed: {ex.Message}");
        }
    }

    // 🔄 Simple Sync Example (Copy data from Server → Client)
    //static void SyncDatabases(string sourceConnStr, string targetConnStr, string tableName)
//    {
//        Console.WriteLine($"\nSyncing {tableName}...");

//        using (var sourceConn = new OleDbConnection(sourceConnStr))
//        using (var targetConn = new OleDbConnection(targetConnStr))
//        {
//            sourceConn.Open();
//            targetConn.Open();

//            // 1. Fetch data from Server
//            string selectQuery = $"SELECT * FROM {tableName}";
//            var cmd = new OleDbCommand(selectQuery, sourceConn);
//            var reader = cmd.ExecuteReader();

//            // 2. Insert into Client DB
//            while (reader.Read())
//            {
//                string insertQuery = $"INSERT INTO {tableName} VALUES (@ID, @Name)"; // Modify columns
//                var insertCmd = new OleDbCommand(insertQuery, targetConn);

//                // Example: Assuming columns are "ID" and "Name"
//                insertCmd.Parameters.AddWithValue("@ID", reader["ID"]);
//                insertCmd.Parameters.AddWithValue("@Name", reader["Name"]);

//                insertCmd.ExecuteNonQuery();
//            }

//            Console.WriteLine($"🔄 Synced {tableName} successfully!");
//        }
//    }
}

