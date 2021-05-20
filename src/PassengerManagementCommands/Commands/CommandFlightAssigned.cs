using Pitstop.Infrastructure.Messaging;
using System;
using System.Collections.Generic;
using System.Text;

namespace AirSupport.Application.PassengerManagementCommands.Commands
{
    public class CommandFlightAssigned : Command
    {
        public readonly int Id;

        public readonly String Origin;

        public readonly String Destination;
        public readonly String ArrivalGate;

        public readonly DateTime ArrivalDate;
        public CommandFlightAssigned(Guid messageId, int id, String origin, String destination, String arrivalGate, DateTime arrivalDate) :
            base(messageId)
        {
            Id = id;
            Origin = origin;
            Destination = destination;
            ArrivalGate = arrivalGate;
            ArrivalDate = arrivalDate;
        }
    }
}