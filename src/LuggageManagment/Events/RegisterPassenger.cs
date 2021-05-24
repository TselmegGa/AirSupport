using System;
using Pitstop.Infrastructure.Messaging;
using Pitstop.LuggageManagment.Model;

namespace Pitstop.LuggageManagment.Events
{
    class RegisterPassenger : Event
    {
        public Passenger passenger;

        public RegisterPassenger(Guid messageId, Passenger passenger) : base(messageId)
        {
            Console.WriteLine("RegisterPassenger Event");
            this.passenger = passenger;
        }
    }
}

