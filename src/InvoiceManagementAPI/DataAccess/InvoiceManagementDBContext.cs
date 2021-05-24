using Microsoft.EntityFrameworkCore;
using Pitstop.InvoiceService.Model;
using Polly;
using System;

namespace Pitstop.InvoiceManagementAPI.DataAccess
{
    public class InvoiceManagementDBContext : DbContext
    {
        public InvoiceManagementDBContext(DbContextOptions<InvoiceManagementDBContext> options) : base(options)
        {

        }

        public DbSet<Renter> Renters { get; set; }
        public DbSet<Invoice> Invoices { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Renter>().HasKey(m => m.RenterId);
            builder.Entity<Renter>().ToTable("Renters");
            builder.Entity<Invoice>().HasKey(m => m.InvoiceId);
            builder.Entity<Invoice>().ToTable("Invoices");
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
