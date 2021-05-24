using Pitstop.InvoiceService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pitstop.InvoiceManagementAPI.Repositories
{
    public interface IInvoiceRepository
    {
        Task<IEnumerable<Invoice>> GetInvoicesAsync();

    }
}
