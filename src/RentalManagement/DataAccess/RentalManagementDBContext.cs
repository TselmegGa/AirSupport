using Microsoft.EntityFrameworkCore;
using AirSupport.RentalManagement.Model;


namespace AirSupport.RentalManagement.DataAccess
{
    public class RentalManagementDBContext : DbContext
    {
        public RentalManagementDBContext(DbContextOptions<RentalManagementDBContext> options) : base(options)
        { }

        public DbSet<Rental> Rental { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {

            builder.Entity<Rental>().HasKey(entity => entity.Id);
            builder.Entity<Rental>().ToTable("Rentals");
            builder.Entity<Rental>().HasIndex(u => u.Location).IsUnique();
        

            base.OnModelCreating(builder);
        }
    }
}
