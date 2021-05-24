using Microsoft.Extensions.Hosting;
using Pitstop.Infrastructure.Messaging;
using Pitstop.TimeService.Events;
using Serilog;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Pitstop.TimeService
{
    public class TimeManager : IHostedService
    {
        DateTime _lastCheck;
        DateTime _lastCheckHour;
        DateTime _lastCheckQuarter;
        CancellationTokenSource _cancellationTokenSource;
        Task _task;
        IMessagePublisher _messagePublisher;

        public TimeManager(IMessagePublisher messagePublisher)
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _lastCheck = DateTime.Now;
            _lastCheckHour = DateTime.Now;
            _lastCheckQuarter = DateTime.Now;
            _messagePublisher = messagePublisher;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _task = Task.Run(() => Worker(), _cancellationTokenSource.Token);
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _cancellationTokenSource.Cancel();
            return Task.CompletedTask;
        }

        private async void Worker()
        {
            while (true)
            {
                Log.Information("started");
                if (DateTime.Now.Subtract(_lastCheck).Days > 0)
                {
                    Log.Information($"Day has passed!");
                    _lastCheck = DateTime.Now;
                    DayHasPassed e = new DayHasPassed(Guid.NewGuid());
                    await _messagePublisher.PublishMessageAsync(e.MessageType, e, "");
                }
                if (DateTime.Now.Subtract(_lastCheckHour).Hours > 0)
	            {
                    Log.Information($"Hour has passed!");
                    _lastCheckHour = DateTime.Now;
                    HourHasPassed e = new HourHasPassed(Guid.NewGuid());
                    await _messagePublisher.PublishMessageAsync(e.MessageType, e, "");
	            }
                if (DateTime.Now.Subtract(_lastCheckQuarter).Minutes >= 15)
                {
                    Log.Information($"15 minutes has passed!");
                    _lastCheckQuarter = DateTime.Now;
                    _15MinutesHasPassed e = new _15MinutesHasPassed(Guid.NewGuid());
                    await _messagePublisher.PublishMessageAsync(e.MessageType, e, "");
                }
                Thread.Sleep(10000);
            }
        }
    }
}
