using Pitstop.Infrastructure.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AirSupport.Application.PassengerManagement.Events
{
    public class GateAssigned : Event
    {
        public readonly int FlightId;
        public readonly string Gate;
    }
}
