using AirSupport.Application.PassengerManagement.Commands;
using Pitstop.Infrastructure.Messaging;
using System;
using System.Collections.Generic;
using System.Text;

namespace AirSupport.Application.PassengerManagement.Events
{
    public class PassengerCheckedIn : Event
    {
        public readonly int Id;
        public readonly bool CheckedIn;

        public PassengerCheckedIn(Guid messageId, int id, bool checkedIn) : 
            base(messageId)
        {
            Id = id;
            CheckedIn = checkedIn;
        }

        public static PassengerCheckedIn FromCommand(SetPassengerCheckedIn command)
        {
            return new PassengerCheckedIn(
                Guid.NewGuid(),
                command.Id,
                command.CheckedIn
            );
        }
    }
}
