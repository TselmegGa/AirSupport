using Pitstop.Infrastructure.Messaging;
using AirSupport.Application.PassengerManagementCommands.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace AirSupport.Application.PassengerManagementCommands.Commands
{
    public class CommandPassengerCheckedIn : Command
    {
        public readonly Passenger Passenger;
        public readonly bool CheckedIn;
        public CommandPassengerCheckedIn(Guid messageId, Passenger passenger, bool checkedIn) :
            base(messageId)
        {
           Passenger = passenger;
           CheckedIn = checkedIn;
        }
    }
}