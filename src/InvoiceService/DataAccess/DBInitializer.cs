using Microsoft.EntityFrameworkCore;
using Polly;
using System;
using Serilog;

namespace Pitstop.InvoiceService.DataAccess
{
    public static class DBInitializer
    {
        public static void Initialize(InvoiceServiceDBContext context)
        {
            Log.Information("Ensure InvoiceManagement Database");

            Policy
            .Handle<Exception>()
            .WaitAndRetry(5, r => TimeSpan.FromSeconds(5), (ex, ts) => { Log.Error("Error connecting to DB. Retrying in 5 sec."); })
            .Execute(() => context.Database.Migrate());

            Log.Information("InvoiceManagement Database available");
        }

        public static void InitializeEventStore(InvoiceEventStoreDBContext context)
        {
            Log.Information("Ensure InvoiceManagement Database");

            Policy
            .Handle<Exception>()
            .WaitAndRetry(5, r => TimeSpan.FromSeconds(5), (ex, ts) => { Log.Error("Error connecting to DB. Retrying in 5 sec."); })
            .Execute(() => context.Database.Migrate());

            Log.Information("InvoiceManagement Database available");
        }
    }
}
