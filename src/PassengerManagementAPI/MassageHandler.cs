using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Linq;
using Pitstop.Infrastructure.Messaging;
using AirSupport.Application.PassengerManagement;
using AirSupport.Application.PassengerManagement.DataAccess;
using AirSupport.Application.PassengerManagement.Stubs;
using AirSupport.Application.PassengerManagement.Events;
using AirSupport.Application.PassengerManagement.Model;
using Serilog;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

namespace AirSupport.Application.PassengerManagement
{
    public class MassageHandler : IHostedService, IMessageHandlerCallback
    {
        IMessageHandler _messageHandler;
        PassengerManagementDBContext _dbContext;
        IMessagePublisher _messagePublisher;

        public MassageHandler(PassengerManagementDBContext dbContex, IMessageHandler messageHandler, IMessagePublisher messagePublisher)
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
                    case "FlightAboutToDepart":
                        Log.Information(messageType);
                        await HandleAsync(messageObject.ToObject<FlightDepartingStub>());
                        break;
                    case "PlaneArrived":
                        Log.Information(messageType);
                        await HandleAsync(messageObject.ToObject<FlightArrivedStub>());
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

        private async Task<bool> HandleAsync(FlightDepartingStub stub)
        {
            List<Passenger> Passengers = await _dbContext.Passengers.Where(e => e.FlightId == stub.Id && e.CheckedIn == false).ToListAsync();
            PassengerToLate e = PassengerToLate.FromPassengerList(Passengers);
            await _messagePublisher.PublishMessageAsync(e.MessageType, e, "");
            return true;
        }

        private async Task<bool> HandleAsync(FlightArrivedStub stub)
        {
            Flight flight = await _dbContext.Flights.FirstAsync(v => v.Id == stub.Id);
            if (flight != null)
            {
                var e = PlaneArrivedCommand.FromFlight(flight, stub.ArrivalDate, stub.ArrivalGate);
                await _messagePublisher.PublishMessageAsync(e.MessageType, e, "");
            }

            return true;
        }

    }
}