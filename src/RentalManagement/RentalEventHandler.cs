using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using Pitstop.Infrastructure.Messaging;
using AirSupport.RentalManagement.DataAccess;
using AirSupport.RentalManagement.Model;
using Serilog;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AirSupport.RentalManagement
{
    public class RentalEventHandler : IHostedService, IMessageHandlerCallback
    {
        RentalManagementDBContext _dbContext;
        IMessageHandler _messageHandler;

        public RentalEventHandler(IMessageHandler messageHandler, RentalManagementDBContext dbContext)
        {
            _messageHandler = messageHandler;
            _dbContext = dbContext;
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
                    case "RentalStopped":
                        await HandleAsync(messageObject.ToObject<RentalStopped>());
                        break;
                    case "RentalRegistered":
                        await HandleAsync(messageObject.ToObject<RentalRegistered>());
                        break;
                    case "ShopRegistered":
                        await HandleAsync(messageObject.ToObject<ShopRegistered>());
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

        private async Task<bool> HandleAsync(RentalStopped e)
        {
            Log.Information("Stop Rental: {name}, {id}", e.name, e);
            try
            {
                var shop = await _dbContext.Rental.FirstOrDefaultAsync(j => j.Id == e.Id);
                shop.Name = e.name;
                await _dbContext.SaveChangesAsync();
            }
            catch(DbUpdateException)
            {
                Console.WriteLine($"Wrong location: {e.Id}.");
            }
            return true;
        }
        private async Task<bool> HandleAsync(ShopRegistered e)
        {
            Log.Information("Register Shop: {name}, {Location}", e.name, e);
            try
            {
                await _dbContext.Rental.AddAsync(new Rental
                {
                    Name = e.name,
                    Location = e.Location
                });
                await _dbContext.SaveChangesAsync();
            }
                catch(DbUpdateException)
            {
                Console.WriteLine($"Skipped adding rental with {e.name} at {e.Location}.");
            }
            return true;
        }
        private async Task<bool> HandleAsync(RentalRegistered e)
        {
            Log.Information("Register Rental: {name}, {id}", e.name, e);
            try
            {
                var shop = await _dbContext.Rental.FirstOrDefaultAsync(j => j.Id == e.Id);
                shop.Name = e.name;
                await _dbContext.SaveChangesAsync();
            }
            catch(DbUpdateException)
            {
                Console.WriteLine($"Wrong location: {e.Id}.");
            }
            return true;
        }
    }
}