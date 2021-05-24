using AirSupport.Application.FlightManagement.Commands;
using Pitstop.Infrastructure.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AirSupport.Application.PassengerManagement.Events
{
    public class FlightRegistered : Event
    {
        public readonly int FlightId;
        public readonly DateTime DepartureDate;
        public readonly string Destination;
        public readonly string Origin;

        public FlightRegistered(Guid messageId, DateTime departureDate, string destination, string Origin) : base(messageId)
        {
            this.DepartureDate = departureDate;
            this.Destination = destination;
            this.Origin = Origin;
        }
    }
}
