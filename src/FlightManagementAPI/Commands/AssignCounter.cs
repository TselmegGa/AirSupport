using Pitstop.Infrastructure.Messaging;
using System;
using System.Collections.Generic;
using System.Text;

namespace AirSupport.Application.FlightManagement.Commands
{
    public class AssignCounter : Command
    {
        public readonly DateTime Recorded;
        public readonly int FlightId;
        public readonly string Counter;

        public AssignCounter(Guid messageId,DateTime Recorded, int FlightId, string Counter) :
            base(messageId)
        {
            this.Recorded = Recorded;
            this.FlightId = FlightId;
            this.Counter = Counter;
        }
    }
}