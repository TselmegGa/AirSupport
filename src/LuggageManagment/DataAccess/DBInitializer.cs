using Microsoft.EntityFrameworkCore;
using Polly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Serilog;

namespace Pitstop.LuggageManagment.DataAccess
{
    public static class DBInitializer
    {
        public static void Initialize(LuggageManagmentDBContext context)
        {
            Log.Information("Ensure LuggageManagment Database");

            Policy
            .Handle<Exception>()
            .WaitAndRetry(5, r => TimeSpan.FromSeconds(5), (ex, ts) => { Log.Error("Error connecting to DB. Retrying in 5 sec."); })
            .Execute(() => context.Database.Migrate());

            Log.Information("LuggageManagment Database available");
        }
    }
}
