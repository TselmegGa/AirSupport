using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Pitstop.Infrastructure.Messaging.Configuration;
using Pitstop.InvoiceService.CommunicationChannels;
using Pitstop.InvoiceService.DataAccess;
using Serilog;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Pitstop.InvoiceService
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
                .ConfigureServices((hostContext, services) =>
                {
                    services.UseRabbitMQMessageHandler(hostContext.Configuration);

                    services.AddTransient<InvoiceManagementDBContext>((svc) =>
                    {
                        var sqlConnectionString = hostContext.Configuration.GetConnectionString("InvoiceManagementCN");
                        var dbContextOptions = new DbContextOptionsBuilder<InvoiceManagementDBContext>()
                            .UseSqlServer(sqlConnectionString)
                            .Options;
                        var dbContext = new InvoiceManagementDBContext(dbContextOptions);

                        DBInitializer.Initialize(dbContext);

                        return dbContext;
                    });

                    services.AddTransient<InvoiceEventStoreDBContext>((svc) =>
                    {
                        var sqlConnectionString = hostContext.Configuration.GetConnectionString("InvoiceEventStoreCN");
                        var dbContextOptions = new DbContextOptionsBuilder<InvoiceEventStoreDBContext>()
                            .UseSqlServer(sqlConnectionString)
                            .Options;
                        var dbContext = new InvoiceEventStoreDBContext(dbContextOptions);

                        DBInitializer.InitializeEventStore(dbContext);

                        return dbContext;
                    });

                    services.AddTransient<IEmailCommunicator>((svc) =>
                    {
                        var mailConfigSection = hostContext.Configuration.GetSection("Email");
                        string mailHost = mailConfigSection["Host"];
                        int mailPort = Convert.ToInt32(mailConfigSection["Port"]);
                        string mailUserName = mailConfigSection["User"];
                        string mailPassword = mailConfigSection["Pwd"];
                        return new SMTPEmailCommunicator(mailHost, mailPort, mailUserName, mailPassword);
                    });

                    services.AddHostedService<InvoiceManager>();
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