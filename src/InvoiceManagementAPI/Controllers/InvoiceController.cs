using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Pitstop.Infrastructure.Messaging;
using Serilog;
using System;
using Pitstop.InvoiceManagementAPI.DataAccess;

namespace Pitstop.Application.CustomerManagementAPI.Controllers
{
    [Route("/api/[controller]")]
    public class InvoiceController : Controller
    {
        IMessagePublisher _messagePublisher;
        InvoiceManagementDBContext _dbContext;

        public InvoiceController(InvoiceManagementDBContext dbContext, IMessagePublisher messagePublisher)
        {
            _dbContext = dbContext;
            _messagePublisher = messagePublisher;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            return Ok(await _dbContext.Renters.ToListAsync());
        }

        [HttpGet]
        [Route("{renterId}", Name = "GetByRenterId")]
        public async Task<IActionResult> GetByRenterId(string renterId)
        {
            var renter = await _dbContext.Renters.FirstOrDefaultAsync(c => c.RenterId == renterId);
            if (renter == null)
            {
                return NotFound();
            }
            return Ok(renter);
        }
    }
}
