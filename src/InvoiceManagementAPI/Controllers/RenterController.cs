using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Pitstop.InvoiceManagementAPI.Repositories;

namespace Pitstop.Application.CustomerManagementAPI.Controllers
{
    [Route("/api/[controller]")]
    public class RenterController : Controller
    {
        IRentersRepository _renterRepo;
        public RenterController(IRentersRepository repo)
        {
            _renterRepo = repo;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var renters = await _renterRepo.GetRentersAsync();
            if (renters == null)
            {
                return NotFound();
            }
            return Ok(renters);
        }
    }
}
