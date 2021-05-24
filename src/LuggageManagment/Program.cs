﻿using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Pitstop.Infrastructure.Messaging;
using Pitstop.Infrastructure.Messaging.Configuration;
using Pitstop.LuggageManagment.DataAccess;

using Serilog;

using Polly;

namespace Pitstop.LuggageManagment
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            Console.WriteLine("Starting up LuggageService");
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
                    
                    
                    services.AddTransient<LuggageManagmentDBContext>((svc) =>
                    {
                        var sqlConnectionString = hostContext.Configuration.GetConnectionString("LuggageManagmentCN");
                        var dbContextOptions = new DbContextOptionsBuilder<LuggageManagmentDBContext>()
                            .UseSqlServer(sqlConnectionString)
                            .Options;
                        var dbContext = new LuggageManagmentDBContext(dbContextOptions);

                        DBInitializer.Initialize(dbContext);

                        return dbContext;
                    });

                    services.AddHostedService<LuggageManagment>();

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
