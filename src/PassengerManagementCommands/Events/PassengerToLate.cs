using AirSupport.Application.PassengerManagementCommands.Commands;
using AirSupport.Application.PassengerManagementCommands.Model;
using Pitstop.Infrastructure.Messaging;
using System;
using System.Collections.Generic;
using System.Text;

namespace AirSupport.Application.PassengerManagementCommands.Events
{
    public class PassengerToLate : Event
    {
        public readonly List<Passenger> Passengers;

        public PassengerToLate(Guid messageId, List<Passenger> passengers) : 
            base(messageId)
        {
            Passengers = passengers;
        }

        public static PassengerToLate FromPassengerList(List<Passenger> command)
        {
            return new PassengerToLate(
                Guid.NewGuid(),
                command
            );
        }
    }
}
