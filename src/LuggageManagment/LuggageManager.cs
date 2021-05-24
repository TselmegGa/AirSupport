using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Linq;
using Pitstop.Infrastructure.Messaging;
using Pitstop.LuggageManagment.Events;
using Pitstop.LuggageManagment.Model;
using Pitstop.LuggageManagment.DataAccess;
using Serilog;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Pitstop.LuggageManagment
{
    public class LuggageManagment : IHostedService, IMessageHandlerCallback
    {
        IMessageHandler _messageHandler;
        LuggageManagmentDBContext _dbContext;

        public LuggageManagment(IMessageHandler messageHandler,LuggageManagmentDBContext dbContext)
        {
            this._messageHandler = messageHandler;
            this._dbContext = dbContext;
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
            Log.Information("Handling "+messageType);
            try
            {

                switch (messageType)
                {
                    case "PassangerToLate":
                    Log.Information("PassengersToLate Message");
                        await HandleAsync(messageObject.ToObject<PassangerToLate>());
                        break;
                
                case "PlaneArrived":
                Log.Information("PlaneArrived Message");
                        await HandleAsync(messageObject.ToObject<PlaneArrived>());
                        break;
                
                 case "LuggageDeliveredByPassenger":
                Log.Information("LuggageDeliveredByPassenger Message");
                        await HandleAsync(messageObject.ToObject<LuggageDeliveredByPassenger>());
                        break;
                
                }
                 Log.Information("UnknownMessageType");
            }
            catch (Exception e)
            {

                string messageId = messageObject.Property("MessageId") != null ? messageObject.Property("MessageId").Value<string>() : "[unknown]";
                Log.Error(e, "Error while handling {MessageType} message with id {MessageId}.", messageType, messageId);
            }
            return true;
        }

        private async Task<bool> HandleAsync(PassangerToLate e)
        {
            Log.Information(e.passengers.Count.ToString());
            foreach(Passenger passenger in e.passengers){
                Log.Information(passenger.Id+" - "+passenger.FirstName);
            }
            // voorbeeld van aan db toevoegen zie EventHandler van WorkshopManagementEventHandler voor meer informatie
            // try{
            //      await _dbContext.Customers.AddAsync(new Customer
            //     {
            //         CustomerId = e.id,
            //         Name = e.Name
            //     });
            //     await _dbContext.SaveChangesAsync();
            // } catch(DbUpdateException){
            //     Log.Warning("Skipped adding customer with customer id {CustomerId}.", e.CustomerId);
            // }
            return true;
        }
        private async Task<bool> HandleAsync(PlaneArrived e)
        {
            Log.Information(e.plane.Passengers.Count.ToString());
             foreach(Passenger passenger in e.plane.Passengers){
                Log.Information(passenger.Id+" - "+passenger.FirstName);
            }
            // voorbeeld van aan db toevoegen zie EventHandler van WorkshopManagementEventHandler voor meer informatie
            // try{
            //      await _dbContext.Customers.AddAsync(new Customer
            //     {
            //         CustomerId = e.id,
            //         Name = e.Name
            //     });
            //     await _dbContext.SaveChangesAsync();
            // } catch(DbUpdateException){
            //     Log.Warning("Skipped adding customer with customer id {CustomerId}.", e.CustomerId);
            // }
            return true;
        }
         private async Task<bool> HandleAsync(LuggageDeliveredByPassenger e)
        {
            Log.Information(e.passenger.FirstName);
            foreach(Luggage luggage in e.passenger.Luggage){
                Log.Information(luggage.LuggageId+" - "+luggage.Brand);
            }
           try
            {
                List<Luggage> luggageList = new List<Luggage>();
                //Adding the Luggage
                    

                // Check if passenger exists in Local DB
                Passenger passenger = await _dbContext.Passengers.FirstOrDefaultAsync(c => c.Id == e.passenger.Id);
                if (passenger == null)
                {
                    //If Passanger isn't known we create a new passenger
                    passenger = new Passenger
                    {
                        Id = e.passenger.Id,
                        FirstName = e.passenger.FirstName,
                        LastName = e.passenger.LastName,
                    };
                    foreach(Luggage luggageFromEvent in e.passenger.Luggage){
                        Luggage luggage = new Luggage{
                            Brand = luggageFromEvent.Brand,
                            Weight = luggageFromEvent.Weight,
                            Color = luggageFromEvent.Color
                        };
                        luggageList.Add(luggage);
                    }
                    passenger.Luggage = luggageList;
                     await _dbContext.Passengers.AddAsync(passenger);
                     await _dbContext.SaveChangesAsync();
                }else{

                Log.Information("Existing Passenger: "+ passenger.FirstName);
                //When passanger exist we add the Luggage to the existing passenger
                Log.Information("Amount of luggage allready present: "+ passenger.Luggage.Count);
                foreach(Luggage luggageFromEvent in e.passenger.Luggage){
                    passenger.Luggage.Add(luggageFromEvent);
                }
                _dbContext.Passengers.Update(passenger);
                await _dbContext.SaveChangesAsync();
                }

                
            }
            catch (DbUpdateException)
            {
                Log.Warning("Skipped adding Luggage", e.passenger.Luggage.Count);
            }
            return true;
        }
    }
}

