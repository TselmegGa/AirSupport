using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System;
using AirSupport.Application.RentalManagementAPI.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Pitstop.Infrastructure.Messaging;
using AirSupport.Application.RentalManagementAPI.Events;
using AirSupport.Application.RentalManagementAPI.Commands;
using AirSupport.Application.RentalManagementAPI.Mappers;
using AirSupport.Application.RentalManagementAPI.Repositories;
using AirSupport.Application.RentalManagementAPI.Domain.Entities;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace AirSupport.Application.RentalManagementAPI.Controllers
{

    [Route("/api/[controller]")]
    public class RentalController : Controller
    {
        IMessagePublisher _messagePublisher;
        RentalManagementDBContext _dbContext;
        IRentalPlanningRepository _rentalPlanningRepository;

        public RentalController(RentalManagementDBContext dbContext, IMessagePublisher messagePublisher, IRentalPlanningRepository rentalPlanningRepository)
        {
            _dbContext = dbContext;
            _messagePublisher = messagePublisher;
            _rentalPlanningRepository = rentalPlanningRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            return Ok(await _dbContext.Rental.ToListAsync());
        }

        [HttpGet]
        [Route("{id}", Name = "GetById")]
        public async Task<IActionResult> GetById(int id)
        {
            var shop = await _dbContext.Rental.FirstOrDefaultAsync(v => v.Id == id);
            if (shop == null)
            {
                return NotFound();
            }
            return Ok(shop);
        }
        
        [HttpPut]
        public async Task<IActionResult> RentAsync([FromBody] RegisterRental command)
        {
            try
            {
                if (ModelState.IsValid)
                {   
                    
                    // insert
                    Model.Rental rental = await _dbContext.Rental.FirstOrDefaultAsync(v => v.Id == command.Id);
                    if (rental == null)
                    {
                        return NotFound();
                    }
                    RentalPlanning planning = await _rentalPlanningRepository.GetRentalPlanningAsync(rental.Location);
                   
                    if (planning == null)
                    {
                        planning = RentalPlanning.Create(rental.Location);
                    }
                   Console.WriteLine("command: "+command.Id + command.name);
                   Console.WriteLine("rental: "+planning._rental + planning._rental.Id);
                    // handle command
                    planning.StartRent(command);
                    
                    Console.WriteLine(planning.Id);
                    // persist
                    IEnumerable<Event> events = planning.GetEvents();
                    Console.WriteLine("rental: "+planning._rental.Name + planning._rental.Id);
                    await  _rentalPlanningRepository.SaveRentalPlanningAsync(
                        planning.Id, planning.OriginalVersion, planning.Version, events);
                    // send event
                    var e = RentalRegistered.FromCommand(command);
                    await _messagePublisher.PublishMessageAsync(e.MessageType, e, "");

                    //return result
                    return CreatedAtRoute("GetById", new { id = rental.Id }, rental);
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
        
        [HttpDelete]
        public async Task<IActionResult> StopAsync( [FromBody] StopRental command)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    
                    // insert
                    Model.Rental rental = await _dbContext.Rental.FirstOrDefaultAsync(v => v.Id == command.Id);
                    if (rental == null)
                    {
                        return NotFound();
                    }
                    RentalPlanning planning = await _rentalPlanningRepository.GetRentalPlanningAsync(rental.Location);
                    if (planning == null)
                    {
                        planning = RentalPlanning.Create(rental.Location);
                    }
                   
                    // handle command
                    planning.StopRent(command);

                    // persist
                    IEnumerable<Event> events = planning.GetEvents();
                    await  _rentalPlanningRepository.SaveRentalPlanningAsync(
                        planning.Id, planning.OriginalVersion, planning.Version, events);


                    // send event
                    var e = RentalStopped.FromCommand(command);
                    await _messagePublisher.PublishMessageAsync(e.MessageType, e, "");

                    //return result
                    return CreatedAtRoute("GetById", new { id = rental.Id }, rental);
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


        [HttpPost]
        public async Task<IActionResult> RegisterAsync([FromBody] RegisterShop command)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // insert vehicle
                    Model.Rental rental = command.MapToRental();
                    RentalPlanning planning = RentalPlanning.Create(command.Location);
                    
                    Console.WriteLine("check 1: " + command.Location+ command.Name);
                    // persist
                    IEnumerable<Event> events = planning.GetEvents();
                    await  _rentalPlanningRepository.SaveRentalPlanningAsync(
                        planning.Id, planning.OriginalVersion, planning.Version, events);

                    // send event
                    var e = ShopRegistered.FromCommand(command);
                    await _messagePublisher.PublishMessageAsync(e.MessageType, e, "");

                    //return result
                    return CreatedAtRoute("GetById", new { id = rental.Id }, rental);
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
