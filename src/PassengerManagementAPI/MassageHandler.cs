using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Linq;
using Pitstop.Infrastructure.Messaging;
using AirSupport.Application.PassengerManagement;
using AirSupport.Application.PassengerManagement.DataAccess;
using AirSupport.Application.PassengerManagement.Commands;
using AirSupport.Application.PassengerManagement.Events;
using AirSupport.Application.PassengerManagement.Model;
using AirSupport.PassengerManagementAPI.Mappers;
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
    public class ResultConv
    {
        public readonly int Id;
        public ResultConv(int id): base()
        {
            Id = id;
        }
    }
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
                        await HandleAsync(messageObject.ToObject<ResultConv>().Id);
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

        private async Task<bool> HandleAsync(int FlightId)
        {
            try
            {
                List<Passenger> Passengers = await _dbContext.Passengers.Where(e => e.FlightId == FlightId && e.CheckedIn == false).ToListAsync();
                PassengerToLate e = PassengerToLate.FromPassengerList(Passengers);
                await _messagePublisher.PublishMessageAsync(e.MessageType, e, "");
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