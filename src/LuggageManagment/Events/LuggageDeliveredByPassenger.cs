using System;
using System.Collections.Generic;
using Pitstop.Infrastructure.Messaging;
using Pitstop.LuggageManagment.Model;

namespace Pitstop.LuggageManagment.Events
{
    class LuggageDeliveredByPassenger : Event
    {
        public Passenger passenger;

        public LuggageDeliveredByPassenger(Guid messageId, Passenger passenger) : base(messageId)
        {
        
            Console.WriteLine("LuggageDeliveredByPassenger");
            this.passenger = passenger;
        }
    }
}