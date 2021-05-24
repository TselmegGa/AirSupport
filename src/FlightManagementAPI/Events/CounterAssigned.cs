using Pitstop.Infrastructure.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AirSupport.Application.PassengerManagement.Events
{
    public class CounterAssigned : Event
    {
        public readonly int FlightId;
        public readonly string Counter;

        public CounterAssigned(Guid messageId, int flightId, string counter) : base(messageId)
        {
            FlightId = flightId;
            Counter = counter;
        }
    }
}
