using AirSupport.Application.FlightManagement.DataAccess;
using Microsoft.AspNetCore.Mvc;
using Pitstop.Infrastructure.Messaging;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AirSupport.Application.FlightManagement.Commands;
using Microsoft.AspNetCore.Http;
using AirSupport.Application.PassengerManagement.Model;
using AirSupport.Application.FlightManagement.Mappers;
using AirSupport.Application.PassengerManagement.Events;
using AirSupport.Application.PassengerManagement.Model.Domain;

namespace AirSupport.Application.FlightManagement.Controllers
{
    [Route("/api/[controller]")]
    public class FlightController : Controller
    {
        IMessagePublisher _messagePublisher;
        FlightManagementDBContext _dbContext;

        public FlightController(FlightManagementDBContext dbContext, IMessagePublisher messagePublisher)
        {
            _dbContext = dbContext;
            _messagePublisher = messagePublisher;
        }
        
        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            return Ok(await _dbContext.Agregates.ToListAsync());
        }

        [HttpGet]
        [Route("{flightId}",Name = "GetByFlightId")]
        public IActionResult GetOne(string flightId)
        {
            int Id = int.Parse(flightId);
            return Ok(_dbContext.Agregates.FirstOrDefault((a) =>  a.FlightId == Id ));
        }

        [HttpPost]
        public async Task<IActionResult> RegisterFlight([FromBody] RegisterFlight command)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var f = command.MapToFlight();

                    await _messagePublisher.PublishMessageAsync(f.MessageType, f, "");

                    return CreatedAtRoute("GetByFlightId",new { flightId = f.FlightId },f);
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

        [Route("{flightId}/gate", Name = "Gate")]
        [HttpPost]
        public async Task<IActionResult> AssignGate([FromBody] AssignGate command)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var f = command.MapToGate();

                    await _messagePublisher.PublishMessageAsync(f.MessageType, f, "");

                    return CreatedAtRoute("GetByFlightId", new { flightId = f.FlightId }, f);
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

        [Route("{flightId}/counter", Name = "Counter")]
        [HttpPost]
        public async Task<IActionResult> AssignCounter([FromBody] AssignCounter command)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var f = command.MapToCounter();

                    await _messagePublisher.PublishMessageAsync(f.MessageType, f, "");

                    return CreatedAtRoute("GetByFlightId", new { flightId = f.FlightId }, f);
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
