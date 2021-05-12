using System;
using System.Threading.Tasks;
using Pitstop.Infrastructure.Messaging;
using Microsoft.Extensions.Hosting;
using Serilog;
using System.IO;
using System.Threading;

namespace CustomerEventHandler
{
    public class CustomerManager : IHostedService, IMessageHandlerCallback
    {
        IMessageHandler _messageHandler;
        private string _logPath;
        public CustomerManager(IMessageHandler messageHandler, CustomerManagerConfig config)
        {
            _messageHandler = messageHandler;
            _logPath = config.LogPath;

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
            string logMessage = $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:ffffff")} - {message}{Environment.NewLine}";
            string logFile = Path.Combine(_logPath, $"{DateTime.Now.ToString("yyyy-MM-dd")}-auditlog.txt");
            await File.AppendAllTextAsync(logFile, logMessage);
            Log.Information("{MessageType} - {Body}", messageType, message);
            Console.WriteLine("{MessageType} - {Body}", messageType, message);
            return true;
        }


    }
}