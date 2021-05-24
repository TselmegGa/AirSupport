using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using AirSupport.Application.PassengerManagementCommands.Model;
using System.ComponentModel.DataAnnotations.Schema;
using Polly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AirSupport.Application.PassengerManagementCommands.DataAccess
{
    public class PassengerManagementDBContext : DbContext
    {
        public PassengerManagementDBContext(DbContextOptions<PassengerManagementDBContext> options) : base(options)
        {
        }

        public DbSet<Passenger> Passengers { get; set; }
        public DbSet<Flight> Flights { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Passenger>().ToTable("Passengers");
            builder.Entity<Flight>().ToTable("Flights");
            builder.Entity<Flight>().Property(e => e.Id).ValueGeneratedNever();
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