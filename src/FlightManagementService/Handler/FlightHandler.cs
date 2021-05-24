using AirSupport.Application.FlightManagement.DataAccess;
using AirSupport.Application.PassengerManagement.Events;
using AirSupport.Application.PassengerManagement.Model;
using AirSupport.Application.PassengerManagement.Model.Domain;
using AirSupport.Application.PassengerManagement.Model.EventModel;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Pitstop.Infrastructure.Messaging;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AirSupport.Application.FlightManagement.Handler
{
    public class FlightHandler : IHostedService, IMessageHandlerCallback
    {
        IMessageHandler _messageHandler;
        FlightManagementDBContext _dbContext;
        IMessagePublisher _messagePublisher;
        FlightAgregator _agregator;

        public FlightHandler(FlightManagementDBContext dbContext, FlightAgregator agregator, IMessageHandler messageHandler,IMessagePublisher messagePublisher)
        {
            _messageHandler = messageHandler;
            _dbContext = dbContext;
            _messagePublisher = messagePublisher;
            _agregator = agregator;
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
            JObject messageObject = MessageSerializer.Deserialize(message);
            try
            {
                switch (messageType)
                {
                    case "15MinutesHasPassed":
                        await HandleAsync(messageObject.ToObject<_15MinutesHasPassed>());
                        break;
                    case "FlightAssigned":
                        HandleAsync(messageObject.ToObject<FlightRegistered>());
                        break;
                    case "GateAssigned":
                        HandleAsync(messageObject.ToObject<GateAssigned>());
                        break;
                    case "CounterAssigned":
                        HandleAsync(messageObject.ToObject<CounterAssigned>());
                        break;
                }
            }
            catch(Exception e)
            {
                Log.Error(e.Message);
            }
            return true;
        }

        private async Task<bool> HandleAsync(_15MinutesHasPassed e)
        {
            var now = DateTime.Now;
            Log.Information("15 Minutes have passed");
            foreach (FlightAgregate f in _dbContext.Agregates.ToList())
            {
                var minutes = f.DepartureTime.Subtract(now).TotalMinutes;
                if (minutes < 15 && minutes > 0)
                {
                    Log.Information("Flight with id " + f.FlightId + "Is about to depart");
                    var departure = new FlightAboutToDepart(Guid.NewGuid(), f.FlightId);
                    await _messagePublisher.PublishMessageAsync(departure.MessageType, departure, "");
                }
            }
            return true;
        }

        private bool HandleAsync(FlightRegistered e)
        {
            Log.Information("Register Flight: {FlightId}. From {Origin} To {Destination} On {DepartureDate}", e.FlightId, e.Origin, e.Destination, e.DepartureDate);
            FlightEvent ev = new FlightEvent();
            ev.FlightId = e.FlightId;
            _agregator.AddEvent(e, e.FlightId);
            return true;
        }

        private bool HandleAsync(GateAssigned e)
        {
            Log.Information("Gate Assigned: {FlightId} is assigned to gate {Gate}. ", e.FlightId, e.Gate);
            FlightEvent ev = new FlightEvent();
            ev.FlightId = e.FlightId;
            _agregator.AddEvent(e, e.FlightId);
            return true;
        }

        private bool HandleAsync(CounterAssigned e)
        {
            Log.Information("Counter Assigned: {FlightId} is assigned to gate {Counter}. ", e.FlightId, e.Counter);
            FlightEvent ev = new FlightEvent();
            ev.FlightId = e.FlightId;
            _agregator.AddEvent(e, e.FlightId);
            return true;
        }
    }
}
