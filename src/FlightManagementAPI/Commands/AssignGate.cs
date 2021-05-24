using Pitstop.Infrastructure.Messaging;
using System;
using System.Collections.Generic;
using System.Text;

namespace AirSupport.Application.FlightManagement.Commands
{
    public class AssignGate : Command
    {
        public readonly int FlightId;
        public readonly string Gate;

        public AssignGate(Guid messageId,  int FlightId, string Gate) :
            base(messageId)
        {
            this.FlightId = FlightId;
            this.Gate = Gate;
        }
    }
}