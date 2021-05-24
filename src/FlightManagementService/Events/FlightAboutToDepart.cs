using Pitstop.Infrastructure.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AirSupport.Application.PassengerManagement.Events
{
    public class FlightAboutToDepart : Event
    {
        public readonly int FlightId;

        public FlightAboutToDepart(Guid messageId, int FlightId) : base(messageId)
        {
            this.FlightId = FlightId;
        }
    }
}
