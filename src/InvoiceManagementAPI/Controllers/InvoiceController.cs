using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Pitstop.InvoiceManagementAPI.Repositories;

namespace Pitstop.Application.CustomerManagementAPI.Controllers
{
    [Route("/api/[controller]")]
    public class InvoiceController : Controller
    {

        IInvoiceRepository _invoiceRepo;
        public InvoiceController(IInvoiceRepository repo)
        {
            _invoiceRepo = repo;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var invoices = await _invoiceRepo.GetInvoicesAsync();
            if (invoices == null)
            {
                return NotFound();
            }
            return Ok(invoices);
        }
    }
}
