using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using AirSupport.Application.RentalManagementAPI.DataAccess;
using AirSupport.Application.RentalManagementAPI.Repositories;
using Pitstop.Infrastructure.Messaging;
using Serilog;
using Microsoft.Extensions.HealthChecks;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.Hosting;
using Pitstop.Infrastructure.Messaging.Configuration;

namespace AirSupport.Application.RentalManagementAPI
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
            var eventStoreConnectionString = _configuration.GetConnectionString("EventStoreCN");
            services.AddTransient<IRentalPlanningRepository>((sp) => 
                new SqlServerRentalPlanningRepository(eventStoreConnectionString));

            var sqlConnectionString = _configuration.GetConnectionString("RentalManagementCN");
            services.AddDbContext<RentalManagementDBContext>(options => options.UseSqlServer(sqlConnectionString));

            // add messagepublisher
            services.UseRabbitMQMessagePublisher(_configuration);

            // Add framework services.
            services
                .AddMvc(options => options.EnableEndpointRouting = false)
                .AddNewtonsoftJson();

            // Register the Swagger generator, defining one or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "RentalManagement API", Version = "v1" });
            });

            services.AddHealthChecks(checks =>
            {
                checks.WithDefaultCacheDuration(TimeSpan.FromSeconds(1));
                checks.AddSqlCheck("EventStoreCN", _configuration.GetConnectionString("EventStoreCN"));
                checks.AddSqlCheck("RentalManagementCN", _configuration.GetConnectionString("RentalManagementCN"));
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime lifetime, IRentalPlanningRepository rentalPlanningRepo)
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
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "RentalManagement API - v1");
            });
            rentalPlanningRepo.EnsureDatabase();

                    
        }
    }
}
