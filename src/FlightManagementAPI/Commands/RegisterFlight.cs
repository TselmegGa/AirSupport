using Pitstop.Infrastructure.Messaging;
using System;
using System.Collections.Generic;
using System.Text;

namespace AirSupport.Application.FlightManagement.Commands
{
    public class RegisterFlight : Command
    {
        public readonly int FlightId;
        public readonly DateTime DepartureDate;
        public readonly string Destination;
        public readonly string Origin;

        public RegisterFlight(Guid messageId, int FlightId, DateTime DepartureDate, string Destination, string Origin) :
            base(messageId)
        {
            this.FlightId = FlightId;
            this.DepartureDate = DepartureDate;
            this.Destination = Destination;
            this.Origin = Origin;
        }
    }
}