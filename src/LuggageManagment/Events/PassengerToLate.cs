using System;
using System.Collections.Generic;
using Pitstop.Infrastructure.Messaging;
using Pitstop.LuggageManagment.Model;

namespace Pitstop.LuggageManagment.Events
{
    class PassengerToLate : Event
    {
        public readonly List<Passenger> passengers;

        public PassengerToLate(Guid messageId, List<Passenger> passengers) : base(messageId)
        {
            Console.WriteLine("PassengerToLateEvent");
            this.passengers = passengers;
        }
    }
}