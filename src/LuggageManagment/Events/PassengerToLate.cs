using System;
using System.Collections.Generic;
using Pitstop.Infrastructure.Messaging;
using Pitstop.LuggageManagment.Model;

namespace Pitstop.LuggageManagment.Events
{
    class PassangerToLate : Event
    {
        public List<Passenger> passengers;

        public PassangerToLate(Guid messageId, List<Passenger> passengers) : base(messageId)
        {
            Console.WriteLine("PassangerToLateEvent");
            this.passengers = passengers;
        }
    }
}