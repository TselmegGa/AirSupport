using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Runtime.ExceptionServices;
using Pitstop.InvoiceManagementAPI.Repositories;
using Pitstop.InvoiceService.Model;
using Dapper;

namespace Pitstop.InvoiceManagementAPI.Repositories
{
    public class SqlServerRefDataRepository : IInvoiceRepository, IRentersRepository
    {
        private string _connectionString;

        public SqlServerRefDataRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<IEnumerable<Invoice>> GetInvoicesAsync()
        {
            List<Invoice> invoices = new List<Invoice>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                try
                {
                    var invoicesSelection = await conn.QueryAsync<Invoice>("select * from Invoices");

                    if (invoicesSelection != null)
                    {
                        invoices.AddRange(invoicesSelection);
                    }
                }
                catch (SqlException ex)
                {
                    HandleSqlException(ex);
                }
            }

            return invoices;
        }

        public async Task<IEnumerable<Renter>> GetRentersAsync()
        {
            List<Renter> renters = new List<Renter>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                try
                {
                    var rentersSelection = await conn.QueryAsync<Renter>("select * from Renters");

                    if (rentersSelection != null)
                    {
                        renters.AddRange(rentersSelection);
                    }
                }
                catch (SqlException ex)
                {
                    HandleSqlException(ex);
                }
            }

            return renters;
        }

        private static void HandleSqlException(SqlException ex)
        {
            if (ex.Errors.Count > 0)
            {
                for (int i = 0; i < ex.Errors.Count; i++)
                {
                    if (ex.Errors[i].Number == 4060)
                    {
                        throw new DatabaseNotCreatedException("WorkshopManagement database not found. This database is automatically created by the WorkshopManagementEventHandler. Run this service first.");
                    }
                }
            }

            // rethrow original exception without poluting the stacktrace
            ExceptionDispatchInfo.Capture(ex).Throw();
        }
    }
}
