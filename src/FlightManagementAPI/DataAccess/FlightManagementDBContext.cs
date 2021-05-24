using AirSupport.Application.PassengerManagement.Model;
using AirSupport.Application.PassengerManagement.Model.Domain;
using AirSupport.Application.PassengerManagement.Model.EventModel;
using Microsoft.EntityFrameworkCore;
using Polly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AirSupport.Application.FlightManagement.DataAccess
{
    public class FlightManagementDBContext : DbContext
    {
        public FlightManagementDBContext(DbContextOptions<FlightManagementDBContext> options) : base(options)
        {
        }

        public DbSet<FlightEvent> Events;
        public DbSet<FlightAgregate> Agregates;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<FlightAgregate>().HasKey(f => new { f.FlightId, f.TimeStamp });
            builder.Entity<FlightAgregate>().ToTable("Flights");
            builder.Entity<FlightEvent>().HasNoKey();
            base.OnModelCreating(builder);
        }

        public void MigrateDB()
        {
            Policy
                .Handle<Exception>()
                .WaitAndRetry(10, r => TimeSpan.FromSeconds(10))
                .Execute(() => Database.Migrate());
        }
    }
}
