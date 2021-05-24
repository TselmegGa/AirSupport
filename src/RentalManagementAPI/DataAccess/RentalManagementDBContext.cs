using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using AirSupport.Application.RentalManagementAPI.Model;
using Polly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AirSupport.Application.RentalManagementAPI.DataAccess
{
    public class RentalManagementDBContext : DbContext
    {
        public RentalManagementDBContext(DbContextOptions<RentalManagementDBContext> options) : base(options)
        {
        }

        public DbSet<Rental> Rental { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Rental>().ToTable("Rentals");
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