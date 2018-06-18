using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using Orleans.Runtime.Configuration;
using System;
using System.Linq;
using System.Net;
using System.Runtime.Loader;
using System.Threading;
using System.Threading.Tasks;

namespace silo
{
    public class Program
    {
        private static ISiloHost _silo;
        private static readonly ManualResetEvent _siloStopped = new ManualResetEvent(false);
        private static readonly string environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

        static void Main(string[] args)
        {
            var ip = Dns.GetHostAddressesAsync(Dns.GetHostName()).Result;

            _silo = new SiloHostBuilder()
                .Configure<ClusterOptions>(op =>
                {
                    op.ClusterId = "rocketcon-backend";
                    op.ServiceId = "rocketcon";
                })
                .Configure<EndpointOptions>(op =>
                {
                    op.AdvertisedIPAddress = ip?.FirstOrDefault();
                    op.GatewayListeningEndpoint = new IPEndPoint(ip?.FirstOrDefault(), 30000);
                    op.SiloListeningEndpoint = new IPEndPoint(ip?.FirstOrDefault(), 11111);
                })

                // Configure all supported grains here
                .ConfigureApplicationParts(p => p.AddFromApplicationBaseDirectory())

                // Configure logging depending on current environment
                .ConfigureLogging(log =>
                {
                    if ("development".Equals(environment, StringComparison.OrdinalIgnoreCase))
                    {
                        log.SetMinimumLevel(LogLevel.Information).AddConsole();
                    }
                    else
                    {
                        log.SetMinimumLevel(LogLevel.Warning).AddConsole();
                    }
                })

                .UseMongoDBClustering(op =>
                {
                    op.DatabaseName = "rocketcon";
                    op.ConnectionString = "mongodb://mongodb";
                })

                .UseInMemoryReminderService()

                // Configure grain storage behaviors
                .AddMemoryGrainStorageAsDefault()
                //.AddMongoDBGrainStorage("MongoDBStorage", op => op.Bind()

                // Adds an administration/monitoring dashboard for orleans, just to make things easier to debug or inspect later on
                .UseDashboard(op =>
                {
                    op.Username = "test";                   // Selected username
                    op.Password = "test";                   // Selected password
                    op.Port = 8080;                         // Makes dashboard reachable on: http://localhost:8080
                    op.CounterUpdateIntervalMs = 6000;      // Forces 6sec update instead of default 1sec
                    op.HideTrace = true;                    // Makes logging a bit less spammy
                }).Build();

            // Boot up silo
            Task.Run(startSilo);

            // This will report silo stopped properly when used in a cluster
            AssemblyLoadContext.Default.Unloading += context =>
            {
                Task.Run(stopSilo);
                _siloStopped.WaitOne();
            };
        }

        private static async Task startSilo()
        {
            await _silo.StartAsync();
            Console.WriteLine("Silo started");
        }

        private static async Task stopSilo()
        {
            await _silo.StopAsync();
            Console.WriteLine("Silo stopped!");
        }
    }
}
