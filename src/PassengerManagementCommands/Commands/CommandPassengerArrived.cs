using Pitstop.Infrastructure.Messaging;
using AirSupport.Application.PassengerManagementCommands.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace AirSupport.Application.PassengerManagementCommands.Commands
{
    public class CommandArrivalPassenger : Command
    {
        public readonly Passenger Passenger;
        public CommandArrivalPassenger(Guid messageId, Passenger passenger) :
            base(messageId)
        {
           Passenger = passenger;
        }
    }
}