using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Pitstop.Infrastructure.Messaging;

namespace AirSupport.Application.PassengerManagement.Model.EventModel
{
    public class FlightEvent
    {
        public Guid Id { get; set; }
        public int FlightId { get; set; }
        public string MessageType { get; set; }
        public string EventData { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}
