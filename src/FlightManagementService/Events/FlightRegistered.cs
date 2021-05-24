using Pitstop.Infrastructure.Messaging;
using System;

namespace AirSupport.Application.PassengerManagement.Events
{
    public class FlightRegistered : Event
    {
        public readonly int FlightId;
        public readonly DateTime DepartureDate;
        public readonly string Destination;
        public readonly string Origin;

        public FlightRegistered(Guid messageId, DateTime departureDate, string destination) : base(messageId)
        {
            this.DepartureDate = departureDate;
            this.Destination = destination;
        }
    }
}
