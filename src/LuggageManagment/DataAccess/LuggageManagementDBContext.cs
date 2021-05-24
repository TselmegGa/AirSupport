using Microsoft.EntityFrameworkCore;
using Pitstop.LuggageManagment.Model;

namespace Pitstop.LuggageManagment.DataAccess
{
    public class LuggageManagmentDBContext : DbContext
    {
        public LuggageManagmentDBContext()
        { }

        public LuggageManagmentDBContext(DbContextOptions<LuggageManagmentDBContext> options) : base(options)
        { }

        public DbSet<Passenger> Passengers { get; set; }
        public DbSet<Luggage> Luggage { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Passenger>().HasKey(entity => entity.Id);
            builder.Entity<Passenger>().ToTable("Passenger");

            builder.Entity<Luggage>().HasKey(entity => entity.LuggageId);
            builder.Entity<Luggage>().ToTable("Luggage");

            base.OnModelCreating(builder);
        }
    }
}

