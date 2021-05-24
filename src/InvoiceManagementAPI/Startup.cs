using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pitstop.Infrastructure.Messaging.Configuration;
using System;
using Serilog;
using Microsoft.Extensions.HealthChecks;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.Hosting;
using Pitstop.InvoiceManagementAPI.Repositories;
using Pitstop.WorkshopManagementAPI.Repositories;

namespace Pitstop.InvoiceManagementAPI
{
    public class Startup
    {
        private IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // add DBContext
            var sqlConnectionString = _configuration.GetConnectionString("InvoiceManagementCN");
            services.AddTransient<IInvoiceManagementRepository>((sp) => new SqlServerInvoiceManagementRepository(sqlConnectionString));
            services.AddTransient<IInvoiceRepository>((sp) => new SqlServerRefDataRepository(sqlConnectionString));
            services.AddTransient<IRentersRepository>((sp) => new SqlServerRefDataRepository(sqlConnectionString));

            // add messagepublisher
            services.UseRabbitMQMessagePublisher(_configuration);

            // Add framework services
            services
                .AddMvc(options => options.EnableEndpointRouting = false)
                .AddNewtonsoftJson();

            // Register the Swagger generator, defining one or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "invoiceManagement API", Version = "v1" });
            });
            
            services.AddHealthChecks(checks =>
            {
                checks.WithDefaultCacheDuration(TimeSpan.FromSeconds(1));
                checks.AddSqlCheck("InvoiceManagementCN", _configuration.GetConnectionString("InvoiceManagementCN"));
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime lifetime, IInvoiceManagementRepository invoiceManagementRepository)
        {
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(_configuration)
                .Enrich.WithMachineName()
                .CreateLogger();

            app.UseMvc();
            app.UseDefaultFiles();
            app.UseStaticFiles();

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS etc.), specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "CustomerManagement API - v1");
            });

            invoiceManagementRepository.EnsureDatabase();
        }
    }
}
