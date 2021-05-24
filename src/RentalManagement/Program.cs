using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Pitstop.Infrastructure.Messaging.Configuration;
using Serilog;
using AirSupport.RentalManagement.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace AirSupport.RentalManagement
{
    class Program
    {
         public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            await host.RunAsync();
        }

        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            var hostBuilder = Host.CreateDefaultBuilder(args)
                .ConfigureHostConfiguration(configHost =>
                {
                    configHost.SetBasePath(Directory.GetCurrentDirectory());
                    configHost.AddJsonFile("hostsettings.json", optional: true);
                    configHost.AddJsonFile($"appsettings.json", optional: false);
                    configHost.AddEnvironmentVariables();
                    configHost.AddEnvironmentVariables("DOTNET_");
                    configHost.AddCommandLine(args);
                })
                .ConfigureAppConfiguration((hostContext, config) =>
                {
                    config.AddJsonFile($"appsettings.{hostContext.HostingEnvironment.EnvironmentName}.json", optional: false);
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.UseRabbitMQMessageHandler(hostContext.Configuration);
                    
                    services.AddTransient<RentalManagementDBContext>((svc) =>
                    {
                        var sqlConnectionString = hostContext.Configuration.GetConnectionString("RentalManagementCN");
                        var dbContextOptions = new DbContextOptionsBuilder<RentalManagementDBContext>()
                            .UseSqlServer(sqlConnectionString)
                            .Options;
                        var dbContext = new RentalManagementDBContext(dbContextOptions);
                        
                        return dbContext;
                    });
                    
                    services.AddHostedService<RentalEventHandler>();
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
