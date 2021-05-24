using AirSupport.Application.FlightManagement.DataAccess;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Pitstop.Infrastructure.Messaging.Configuration;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AirSupport.Application.FlightManagement
{
    public class Startup
    {
        private IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            
            // add DBContext
            var sqlConnectionString = _configuration.GetConnectionString("FlightManagementCN");

            services.AddDbContext<FlightManagementDBContext>(options => options.UseSqlServer(sqlConnectionString));

            // add messagepublisher
            services.UseRabbitMQMessagePublisher(_configuration);

            // Add framework services
            services
                .AddMvc(options => options.EnableEndpointRouting = false)
                .AddNewtonsoftJson();

            // Register the Swagger generator, defining one or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "FlightManagement API", Version = "v1" });
            });

            services.AddHealthChecks(checks =>
            {
                checks.WithDefaultCacheDuration(TimeSpan.FromSeconds(1));
                checks.AddSqlCheck("FlightManagementCN", _configuration.GetConnectionString("FlightManagementCN"));
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime lifetime, FlightManagementDBContext dbContext)
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
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "FlightManagement API - v1");
            });

            // auto migrate db
            using (var scope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                scope.ServiceProvider.GetService<FlightManagementDBContext>().MigrateDB();
            }
        }
    }
}
