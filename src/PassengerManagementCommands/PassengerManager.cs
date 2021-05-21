using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Linq;
using Pitstop.Infrastructure.Messaging;
using AirSupport.Application.PassengerManagementCommands;
using AirSupport.Application.PassengerManagementCommands.DataAccess;
using AirSupport.Application.PassengerManagementCommands.Commands;
using AirSupport.Application.PassengerManagementCommands.Events;
using AirSupport.Application.PassengerManagementCommands.Model;
using AirSupport.PassengerManagementCommands.Mappers;
using Serilog;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

namespace AirSupport.Application.PassengerManagementCommands
{
    public class PassengerManager : IHostedService, IMessageHandlerCallback
    {
        IMessageHandler _messageHandler;
        PassengerManagementDBContext _dbContext;
        IMessagePublisher _messagePublisher;

        public PassengerManager(PassengerManagementDBContext dbContex, IMessageHandler messageHandler, IMessagePublisher messagePublisher)
        {
            this._messageHandler = messageHandler;
            this._dbContext = dbContex;
            this._messagePublisher = messagePublisher;
        }
        public void Start()
        {
            _messageHandler.Start(this);
        }

        public void Stop()
        {
            _messageHandler.Stop();
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _messageHandler.Start(this);
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _messageHandler.Stop();
            return Task.CompletedTask;
        }
        public async Task<bool> HandleMessageAsync(string messageType, string message)
        {
            Log.Information(messageType);
            JObject messageObject = MessageSerializer.Deserialize(message);
            try
            {

                switch (messageType)
                {
                    case "PassengerRegistered":
                        Log.Information(messageType);
                        await HandleAsync(messageObject.ToObject<CommandRegisterPassenger>());
                        break;
                    case "FlightAssigned":
                        Log.Information(messageType);
                        await HandleAsync(messageObject.ToObject<CommandRegisterFlight>());
                        break;
                    case "PassengerCheckedIn":
                        Log.Information(messageType);
                        await HandleAsync(messageObject.ToObject<CommandPassengerCheckedIn>());
                        break;
                    case "PlaneArrivedCommand":
                        Log.Information(messageType);
                        await HandleAsync(messageObject.ToObject<CommandPlaneArrived>());
                        break;
                }
            }
            catch (Exception e)
            {
                string messageId = messageObject.Property("MessageId") != null ? messageObject.Property("MessageId").Value<string>() : "[unknown]";
                Log.Error(e, "Error while handling {MessageType} message with id {MessageId}.", messageType, messageId);
            }
            return true;
        }
        private async Task<bool> HandleAsync(CommandRegisterFlight command)
        {
            try
            {
                if (command.isValid())
                {
                    Flight flight = command.MapToFlight();
                    if (_dbContext.Flights.Any(e => e.Id = flight.Id))
                    {
                        _dbContext.Flights.Update(flight);
                    }
                    else
                    {
                        _dbContext.Flights.Add(flight);
                    }
                    await _dbContext.SaveChangesAsync();
                }
            }
            catch (DbUpdateException)
            {
                Log.Information("Unable to save changes. " +
                    "Try again, and if the problem persists " +
                    "see your system administrator.");
            }
            return true;
        }
        private async Task<bool> HandleAsync(CommandRegisterPassenger command)
        {
            try
            {
                if (command.isValid)
                {
                    Passenger passenger = command.MapToPassenger();
                    _dbContext.Passengers.Add(passenger);
                    await _dbContext.SaveChangesAsync();

                    RegisterPassenger e = RegisterPassenger.FromCommand(command);
                    await _messagePublisher.PublishMessageAsync(e.MessageType, e, "");
                }


            }
            catch (DbUpdateException)
            {
                Log.Information("Unable to save changes. " +
                    "Try again, and if the problem persists " +
                    "see your system administrator.");
            }

            return true;
        }

        private async Task<bool> HandleAsync(CommandFlightAssigned command)
        {
            try
            {
                Flight flight = command.MapToFlight();
                _dbContext.Flights.Add(flight);
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                Log.Information("Unable to save changes. " +
                    "Try again, and if the problem persists " +
                    "see your system administrator.");
            }
            return true;
        }

        private async Task<bool> HandleAsync(CommandPassengerCheckedIn command)
        {
            try
            {
                Passenger p = command.Passenger;
                p.CheckedIn = command.CheckedIn;
                _dbContext.Passengers.Update(p);
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                Log.Information("Unable to save changes. " +
                    "Try again, and if the problem persists " +
                    "see your system administrator.");
            }
            return true;
        }

        private async Task<bool> HandleAsync(CommandPlaneArrived command)
        {
            try
            {
                Flight flight = command.Flight;
                flight.ArrivalDate = command.ArrivalDate;
                flight.ArrivalGate = command.ArrivalGate;
                _dbContext.Flights.Update(flight);
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                Log.Information("Unable to save changes. " +
                    "Try again, and if the problem persists " +
                    "see your system administrator.");
            }
            return true;
        }
    }
}