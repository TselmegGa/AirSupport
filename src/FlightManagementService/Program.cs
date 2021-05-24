using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore;
using Serilog;
using System.IO;
using Microsoft.Extensions.Hosting;
using Pitstop.Infrastructure.Messaging.Configuration;
using AirSupport.Application.FlightManagement.DataAccess;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using AirSupport.Application.FlightManagement.Handler;
using System.Threading.Tasks;
using AirSupport.Application.PassengerManagement.Model;

namespace AirSupport.Application.FlightManagement
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            await host.RunAsync();
        }

        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            var hostBuilder = Host.CreateDefaultBuilder(args)
                 .ConfigureServices((hostContext, services) =>
                 {
                     var connection = hostContext.Configuration.GetConnectionString("FlightManagementCN");
                     var dbContextOptions = new DbContextOptionsBuilder<FlightManagementDBContext>()
                        .UseSqlServer(connection)
                        .Options;
                     var dbContext = new FlightManagementDBContext(dbContextOptions);

                     services.UseRabbitMQMessageHandler(hostContext.Configuration);

                     services.AddTransient<FlightManagementDBContext>((svc) =>
                     {
                         return dbContext;
                     });

                     services.AddTransient<FlightAgregator>((svr) =>
                     {
                         return new FlightAgregator(dbContext);
                     });

                     services.AddHostedService<FlightHandler>();
                 })
                 .UseSerilog((hostContext,loggerConfiguration) =>
                 {
                     loggerConfiguration.ReadFrom.Configuration(hostContext.Configuration);
                 })
                 .UseConsoleLifetime();
            return hostBuilder;
        }
    }
}
