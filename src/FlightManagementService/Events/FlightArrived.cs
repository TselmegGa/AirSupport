using Pitstop.Infrastructure.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AirSupport.Application.PassengerManagement.Events
{
    public class FlightArrived : Event
    {
        public readonly int FlightId;
        public readonly string Gate;
        public readonly DateTime ArrivalDate;

        public FlightArrived(Guid messageId, int flightId, string gate, DateTime arrivalDate) : base(messageId)
        {
            FlightId = flightId;
            Gate = gate;
            ArrivalDate = arrivalDate;
        }
    }
}
