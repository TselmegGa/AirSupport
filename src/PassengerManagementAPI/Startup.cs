﻿using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using AirSupport.Application.PassengerManagement.DataAccess;
using Pitstop.Infrastructure.Messaging;
using Serilog;
using Microsoft.Extensions.HealthChecks;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.Hosting;
using Pitstop.Infrastructure.Messaging.Configuration;

namespace AirSupport.Application.PassengerManagement
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
            var sqlConnectionString = _configuration.GetConnectionString("PassengerManagementCN");
            services.AddDbContext<PassengerManagementDBContext>(options => options.UseSqlServer(sqlConnectionString));

            // add messagepublisher
            services.UseRabbitMQMessagePublisher(_configuration);
            services.UseRabbitMQMessageHandler(_configuration);
            services.AddHostedService<MassageHandler>();

            // Add framework services.
            services
                .AddMvc(options => options.EnableEndpointRouting = false)
                .AddNewtonsoftJson();

            // Register the Swagger generator, defining one or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "PassengerManagement API", Version = "v1" });
            });

            services.AddHealthChecks(checks =>
            {
                checks.WithDefaultCacheDuration(TimeSpan.FromSeconds(1));
                checks.AddSqlCheck("PassengerManagementCN", _configuration.GetConnectionString("PassengerManagementCN"));
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime lifetime, PassengerManagementDBContext dbContext)
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
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "PassengerManagement API - v1");
            });

            // auto migrate db
            using (var scope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                scope.ServiceProvider.GetService<PassengerManagementDBContext>().MigrateDB();
            }                     
        }
    }
}
