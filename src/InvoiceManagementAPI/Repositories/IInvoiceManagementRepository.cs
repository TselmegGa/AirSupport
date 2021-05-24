using Pitstop.Infrastructure.Messaging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pitstop.InvoiceManagementAPI.Repositories
{
    public interface IInvoiceManagementRepository
    {
        void EnsureDatabase();
    }
}
