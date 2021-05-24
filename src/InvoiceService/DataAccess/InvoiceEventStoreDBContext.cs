using Microsoft.EntityFrameworkCore;
using Pitstop.InvoiceService.Model;

namespace Pitstop.InvoiceService.DataAccess
{
    public class InvoiceEventStoreDBContext : DbContext
    {
        public InvoiceEventStoreDBContext(DbContextOptions<InvoiceEventStoreDBContext> options) : base(options)
        {

        }

        public DbSet<EventStore> Events { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<EventStore>().HasKey(m => m.Id);
            builder.Entity<EventStore>().ToTable("Events");

            base.OnModelCreating(builder);
        }
    }
}
