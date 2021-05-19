using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using AirSupport.Application.PassengerManagement.Model;
using Polly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AirSupport.Application.PassengerManagement.DataAccess
{
    public class PassengerManagementDBContext : DbContext
    {
        public PassengerManagementDBContext(DbContextOptions<PassengerManagementDBContext> options) : base(options)
        {
        }

        public DbSet<Passenger> Passengers { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Passenger>().ToTable("Passengers");
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