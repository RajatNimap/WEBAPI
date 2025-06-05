//using System;
//using Dotmim.Sync;
//using Dotmim.Sync.SqlServer;
//using Microsoft.Data.SqlClient;

//class Program
//{
//    static void Main()
//    {
//        string localConnectionString = "Server=DESKTOP-BQ5HJVF; Database=SyncData; Trusted_Connection=true; TrustServerCertificate=true;";
//        string serverConnectionString = "Server=192.168.137.47,1433; Database=HospitalSystem; User Id=sa; Password=root;  TrustServerCertificate=True;";

//        TestConnection("Local Database", localConnectionString);
//        TestConnection("Server Database", serverConnectionString);
//        Synchronization(serverConnectionString, localConnectionString);
//        Console.ReadKey();
//    }

//    static void TestConnection(string name, string connectionString)
//    {
//        Console.WriteLine($"\nTesting {name} connection...");

//        try
//        {
//            using (var connection = new SqlConnection(connectionString))
//            {
//                connection.Open();
//                Console.WriteLine($"SUCCESS: Connected to {name}");
//                Console.WriteLine($"  Server: {connection.DataSource}");
//                Console.WriteLine($"  Database: {connection.Database}");
//                Console.WriteLine($"  Version: {connection.ServerVersion}");
//            }
//        }
//        catch (Exception ex)
//        {
//            Console.WriteLine($"FAILED: Could not connect to {name}");
//            Console.WriteLine($"  Error: {ex.Message}");

//            if (ex is SqlException sqlEx)
//            {
//                Console.WriteLine($"  SQL Error #{sqlEx.Number}");
//            }
//        }

//    }

//    static async void Synchronization(string serverConnectionString,string clientConnectionString)
//    {
//        var serverProvider = new SqlSyncProvider(serverConnectionString);


//        var clientProvider = new SqlSyncProvider(clientConnectionString);

//        var setup = new SyncSetup("Test");

//        var agent = new SyncAgent(clientProvider, serverProvider);

//        do
//        {
//            // Launch the sync process
//            var s1 = await agent.SynchronizeAsync(setup);
//            // Write results
//            Console.WriteLine(s1);

//        } while (Console.ReadKey().Key != ConsoleKey.Escape);

//        Console.WriteLine("End");
//    }
//}


using System;
using System.Threading;
using System.Threading.Tasks;
using Dotmim.Sync;
using Dotmim.Sync.SqlServer;
using Microsoft.Data.SqlClient;

class Program
{
    static async Task Main()
    {
        string localConnectionString = "Server=DESKTOP-BQ5HJVF; Database=SyncData; Trusted_Connection=true; TrustServerCertificate=true;";
        string serverConnectionString = "Server=192.168.137.47,1433; Database=HospitalSystem; User Id=sa; Password=root; TrustServerCertificate=True;";

        TestConnection("Local Database", localConnectionString);
        TestConnection("Server Database", serverConnectionString);

        var cts = new CancellationTokenSource();

        var syncTask = Synchronization(serverConnectionString, localConnectionString, cts.Token);

        Console.WriteLine("\nSynchronization started. Press 'Q' then Enter to stop.\n");
        while (true)
        {
            var input = Console.ReadLine();
            if (string.Equals(input, "Q", StringComparison.OrdinalIgnoreCase))
            {
                cts.Cancel();
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

    static void TestConnection(string name, string connectionString)
    {
        Console.WriteLine($"\nTesting {name} connection...");

        try
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                Console.WriteLine($"SUCCESS: Connected to {name}");
                Console.WriteLine($"  Server: {connection.DataSource}");
                Console.WriteLine($"  Database: {connection.Database}");
                Console.WriteLine($"  Version: {connection.ServerVersion}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"FAILED: Could not connect to {name}");
            Console.WriteLine($"  Error: {ex.Message}");

            if (ex is SqlException sqlEx)
            {
                Console.WriteLine($"  SQL Error #{sqlEx.Number}");
            }
        }
    }

    static async Task Synchronization(string serverConnectionString, string clientConnectionString, CancellationToken cancellationToken)
    {
        var serverProvider = new SqlSyncProvider(serverConnectionString);
        var clientProvider = new SqlSyncProvider(clientConnectionString);
        var setup = new SyncSetup("Test");
        var agent = new SyncAgent(clientProvider, serverProvider);

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                var syncResult = await agent.SynchronizeAsync(setup);

                Console.WriteLine($"[{DateTime.Now:T}] Synchronization succeeded: {syncResult.TotalChangesDownloadedFromServer} downloaded, {syncResult.TotalChangesUploadedToServer} uploaded.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[{DateTime.Now:T}] Synchronization failed: {ex.Message}");
            }

            try
            {
                await Task.Delay(TimeSpan.FromSeconds(10), cancellationToken);
            }
            catch (TaskCanceledException)
            {
                break;
            }
        }
    }
}

