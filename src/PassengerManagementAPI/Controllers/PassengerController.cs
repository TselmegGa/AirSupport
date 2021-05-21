using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using AirSupport.Application.PassengerManagement.Model;
using AirSupport.Application.PassengerManagement.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Pitstop.Infrastructure.Messaging;
using AirSupport.Application.PassengerManagement.Events;
using AirSupport.Application.PassengerManagement.Stubs;
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
        public async Task<IActionResult> RegisterAsync([FromBody] Passenger passenger)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // 

                    // check invariants
                    // if (!Regex.IsMatch(command.LicenseNumber, NUMBER_PATTERN, RegexOptions.IgnoreCase))
                    // {
                    //     return BadRequest($"The specified license-number '{command.LicenseNumber}' was not in the correct format.");
                    // }

                    // send event
                    var e = PassengerRegistered.FromPassenger(passenger);
                    await _messagePublisher.PublishMessageAsync(e.MessageType, e, "");
                    return Ok("Prosessing");
                    //return result
                    // return CreatedAtRoute("GetById", new { id = passenger.Id }, passenger);
                }
                return BadRequest(ModelState);
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Unable to save changes. " +
                    "Try again, and if the problem persists " +
                    "see your system administrator.");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost]
        [Route("CheckedIn", Name = "CheckedIn")]
        public async Task<IActionResult> SetCheckedInAsync([FromBody] PassengerCheckedInStub command)
        {
            try
            {
                Passenger passenger = await _dbContext.Passengers.FirstOrDefaultAsync(v => v.Id == command.Id);
                var e = PassengerCheckedIn.FromPassenger(passenger, command.CheckedIn);
                await _messagePublisher.PublishMessageAsync(e.MessageType, e, "");
                return Ok("Prosessing");

            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Unable to save changes. " +
                    "Try again, and if the problem persists " +
                    "see your system administrator.");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost]
        [Route("Arrived", Name = "GetArrived")]
        public async Task<IActionResult> GetArrivedAsync([FromBody] ArrivedStub stub)
        {

            List<Passenger> passengers = await _dbContext.Passengers.Where(e => e.Flight.ArrivalDate >= stub.ArrivalDate).ToListAsync();
            if (passengers == null)
            {
                return NotFound();
            }
            return Ok(passengers);
        }

        [HttpPost]
        [Route("CheckedInPassengers", Name = "CheckedInPassengers")]
        public async Task<IActionResult> GetCheckedInPassengersAsync([FromBody] ArrivedStub stub)
        {

            // List<Flight> flights = await _dbContext.Flights.Where(e=> e.ArrivalDate == null).ToListAsync();
            List<Passenger> passengers = await _dbContext.Passengers.Where(e => e.Flight.ArrivalDate == null && e.Flight.DepartureDate > stub.ArrivalDate && e.CheckedIn == true).ToListAsync();
            if (passengers == null)
            {
                return NotFound();
            }
            return Ok(passengers);
        }

    }
}
