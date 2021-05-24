using Pitstop.Infrastructure.Messaging;
using System;
using System.Threading.Tasks;
using System.Reflection;
using System.Text;
using Newtonsoft.Json.Linq;
using System.IO;
using Serilog;
using Microsoft.Extensions.Hosting;
using System.Threading;

namespace EventManagment
{
    public class EventManager : IHostedService, IMessageHandlerCallback
    {
        private IMessageHandler _messageHandler;
        private string _logPath;
        private string[] _acceptedEvents;

        public EventManager(IMessageHandler messageHandler, EventManagerConfig config)
        {
            _messageHandler = messageHandler;
            _logPath = config.LogPath;
            _acceptedEvents = new string[] { 
                "FlightAboutToDepart",
                "PassengerArrived",
                "FlightAssigned",
                "RegisterPassenger",
                "PassengerToLate",
                "InvoiceCreated",
                "RentalRegistered",
                "RentalStopped",
                "ShopRegistered",
                "FlightAboutToDepart",
                "PlaneArrived",
                "FlightAssigned",
                "PassengerToLate",
                "PlaneArrived",
                "LuggageDeliverdToPlane",
                "LuggageDelivedToGate" };

            if (!Directory.Exists(_logPath))
            {
                Directory.CreateDirectory(_logPath);
            }
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
            try
            {
                JObject messageObject = MessageSerializer.Deserialize(message);

                foreach(string acceptedEvent in _acceptedEvents)
                {
                    if(acceptedEvent == messageType)
                    {
                        await HandleWriteLogAsync(messageType, message);
                        break;
                    }    
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error while handling {messageType} event.");
            }

            return true;
        }

        public async Task<bool> HandleWriteLogAsync(string messageType, string message)
        {
            string logMessage = $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:ffffff")} - {message}{Environment.NewLine}";
            string logFile = Path.Combine(_logPath, $"{DateTime.Now.ToString("yyyy-MM-dd")}-event_log.txt");
            await File.AppendAllTextAsync(logFile, logMessage);
            Log.Information("Event: {MessageType} - {Body}", messageType, message);
            return true;
        }
    }
}
