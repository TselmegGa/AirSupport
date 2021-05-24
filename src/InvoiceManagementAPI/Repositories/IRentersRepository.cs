using Pitstop.InvoiceService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pitstop.InvoiceManagementAPI.Repositories
{
    public interface IRentersRepository
    {
        Task<IEnumerable<Renter>> GetRentersAsync();

    }
}
