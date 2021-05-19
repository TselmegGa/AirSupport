using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AirSupport.Application.PassengerManagement.Model;
using AirSupport.Application.PassengerManagement.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Pitstop.Infrastructure.Messaging;
using AirSupport.Application.PassengerManagement.Events;
using AirSupport.Application.PassengerManagement.Commands;
using AirSupport.PassengerManagementAPI.Mappers;
using System.Text.RegularExpressions;

namespace AirSupport.Application.PassengerManagement.Controllers
{

    [Route("/api/[controller]")]
    public class PassengerController : Controller
    {
        IMessagePublisher _messagePublisher;
        PassengerManagementDBContext _dbContext;

        public PassengerController(PassengerManagementDBContext dbContext, IMessagePublisher messagePublisher)
        {
            _dbContext = dbContext;
            _messagePublisher = messagePublisher;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            return Ok(await _dbContext.Passengers.ToListAsync());
        }

        [HttpGet]
        [Route("{id}", Name = "GetById")]
        public async Task<IActionResult> GetById(int id)
        {
            var passenger = await _dbContext.Passengers.FirstOrDefaultAsync(v => v.Id == id);
            if (passenger == null)
            {
                return NotFound();
            }
            return Ok(passenger);
        }

        [HttpPost]
        public async Task<IActionResult> RegisterAsync([FromBody] RegisterPassenger command)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // check invariants
                    // if (!Regex.IsMatch(command.LicenseNumber, NUMBER_PATTERN, RegexOptions.IgnoreCase))
                    // {
                    //     return BadRequest($"The specified license-number '{command.LicenseNumber}' was not in the correct format.");
                    // }

                    // insert vehicle
                    Passenger passenger = command.MapToVehicle();
                    _dbContext.Passengers.Add(passenger);
                    await _dbContext.SaveChangesAsync();

                    // send event
                    var e = PassengerRegistered.FromCommand(command);
                    await _messagePublisher.PublishMessageAsync(e.MessageType, e, "");

                    //return result
                    return CreatedAtRoute("GetById", new { id = passenger.Id }, passenger);
                }
                return BadRequest();
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Unable to save changes. " +
                    "Try again, and if the problem persists " +
                    "see your system administrator.");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
