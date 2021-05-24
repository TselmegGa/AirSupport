using Microsoft.EntityFrameworkCore;
using Pitstop.InvoiceService.Model;

namespace Pitstop.InvoiceService.DataAccess
{
    public class InvoiceServiceDBContext : DbContext
    {
        public InvoiceServiceDBContext(DbContextOptions<InvoiceServiceDBContext> options) : base(options)
        {

        }

        public DbSet<Renter> Renters { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<EventStore> Events { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Renter>().HasKey(m => m.RenterId);
            builder.Entity<Renter>().ToTable("Renters");

            builder.Entity<Invoice>().HasKey(m => m.InvoiceId);
            builder.Entity<Invoice>().ToTable("Invoices");

            builder.Entity<EventStore>().ToTable("Events");

            base.OnModelCreating(builder);
        }
    }
}
