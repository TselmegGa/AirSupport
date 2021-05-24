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

                         case "RegisterPassenger":
                Log.Information("RegisterPassengerMessage");
                        await HandleAsync(messageObject.ToObject<RegisterPassenger>());
                        break;

                         case "LuggageClaimedByPassenger":
                Log.Information("LuggageClaimedByPassenger");
                        await HandleAsync(messageObject.ToObject<LuggageClaimedByPassenger>());
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
           //TODO
            return true;
        }
        private async Task<bool> HandleAsync(PlaneArrived e)
        {
            Log.Information(e.plane.Passengers.Count.ToString());
             foreach(Passenger passenger in e.plane.Passengers){
                Log.Information(passenger.Id+" - "+passenger.FirstName+" has arrived!");
                Passenger passengerFromDB = await _dbContext.Passengers.FirstOrDefaultAsync(c => c.Id == passenger.Id);
                passengerFromDB.Arrived=true;
            }
            await _dbContext.SaveChangesAsync();
            
            
            return true;
        }
         private async Task<bool> HandleAsync(LuggageDeliveredByPassenger e)
        {
            Log.Information("LuggageDeliverdBy: "+e.PassengerId.ToString());

           try{
                // Check if passenger exists in Local DB
                Passenger passenger = await _dbContext.Passengers.Include(x=> x.Luggage).FirstOrDefaultAsync(c => c.Id == e.PassengerId);
                if (passenger == null)
                {
                    //Not registerd passengers should not be able to add luggage
                    Log.Information(e.PassengerId+"Not found");
                    return false;
                }else{

                Log.Information("Passenger: "+ passenger.FirstName);
                
                //When passanger exist we add the Luggage to the existing passenger
                Log.Information("Amount of luggage allready present: "+ passenger.Luggage.Count);
                Luggage luggage = new Luggage{
                    Weight = e.Weight,
                    Brand = e.Brand,
                    Color = e.Color
                };
                passenger.Luggage.Add(luggage);
                
                _dbContext.Passengers.Update(passenger);
                await _dbContext.SaveChangesAsync();
                }

                
            }
            catch (DbUpdateException)
            {
                Log.Warning("Skipped adding Luggage");
            }
            return true;
        }
        private async Task<bool> HandleAsync(LuggageClaimedByPassenger e)
        {
            Log.Information("LuggageItemClaimed: "+e.LuggageId.ToString());
            //TODO Implement check to see if passenger actually arrived
           try{
                // Check if Luggage exist in DB
                Luggage luggage = await _dbContext.Luggage.FirstOrDefaultAsync(c => c.LuggageId == e.LuggageId);
                if (luggage == null)
                {
                    //Luggage not registerd
                    Log.Information(e.LuggageId+" Not found");
                    return false;
                }else{
                    Log.Information("LuggageIdFound: "+ luggage.LuggageId);
              
                //Remove Luggage, as passenger has retrieved it
                 _dbContext.Luggage.Remove(luggage);

    
              
                await _dbContext.SaveChangesAsync();
                }

                
            }
            catch (DbUpdateException)
            {
                Log.Warning("Skipped adding Luggage");
            }
            return true;
        }
    
      private async Task<bool> HandleAsync(RegisterPassenger e)
        {
            Log.Information(e.Id+" - "+e.FirstName+" - "+e.LastName+" registerd");

           try
            {
                //Adding the Passenger when a passenger is registered
                Passenger passenger = new Passenger
                    {
                        Id = e.Id, 
                        FirstName = e.FirstName,
                        LastName = e.LastName,
                        Arrived = false
                    };
                    Log.Information("AttempingtoAdd", e.Id);
                    await _dbContext.Passengers.AddAsync(passenger);

                     Log.Information("AttempingToSave", e.Id);

                     await _dbContext.SaveChangesAsync();
                     Log.Information("Done", e.Id);
            }
            catch (DbUpdateException)
            {
                //TODO fix duplicate Id's
                Log.Warning("Skipped adding passenger "+e.LastName, e.Id);
            }
            return true;
        }
    }
}

