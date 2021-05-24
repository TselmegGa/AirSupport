using System;
using System.Threading.Tasks;
using Pitstop.Infrastructure.Messaging.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace EventManagment
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            Console.WriteLine("Starting up Event Manager Service");

            var host = CreateHostBuilder(args).Build();
            await host.RunAsync();
        }

        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            var hostBuilder = Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.UseRabbitMQMessageHandler(hostContext.Configuration);

                    services.AddTransient<EventManagerConfig>((svc) =>
                    {
                        var EventManagerConfigSection = hostContext.Configuration.GetSection("EventManagment");
                        string logPath = EventManagerConfigSection["path"];
                        return new EventManagerConfig { LogPath = logPath };
                    });

                    services.AddHostedService<EventManager>();
                })
                .UseSerilog((hostContext, loggerConfiguration) =>
                {
                    loggerConfiguration.ReadFrom.Configuration(hostContext.Configuration);
                })
                .UseConsoleLifetime();
                
            return hostBuilder;
        }        
    }
}
