using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pitstop.LuggageManagment.Model;
using System.ComponentModel.DataAnnotations.Schema;
using Polly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Pitstop.LuggageManagment.DataAccess
{
    public class LuggageManagmentDBContext : DbContext
    {
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
    
        public void MigrateDB()
        {
            Policy
                .Handle<Exception>()
                .WaitAndRetry(10, r => TimeSpan.FromSeconds(10))
                .Execute(() => Database.Migrate());
        }
    }
}