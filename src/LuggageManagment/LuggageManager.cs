using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Linq;
using Pitstop.Infrastructure.Messaging;
using Pitstop.LuggageManagment.Events;
using Serilog;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Pitstop.LuggageManagment
{
    public class LuggageManagment : IHostedService, IMessageHandlerCallback
    {
        IMessageHandler _messageHandler;

        public LuggageManagment(IMessageHandler messageHandler)
        {
            this._messageHandler = messageHandler;
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
                    case "PassengerToLate":
                        await HandleAsync(messageObject.ToObject<PassangerToLate>());
                        break;
                
                case "PlaneArrived":
                        await HandleAsync(messageObject.ToObject<PlaneArrived>());
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

        private async Task<bool> HandleAsync(PassangerToLate e)
        {
            Log.Information("List of Passangers: {name}, {id}", e.passangers, e);
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
            Log.Information("Arrived Passangenrs: {name}, {id}", e.passangers, e);
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
    }
}