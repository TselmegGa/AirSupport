using AirSupport.Application.PassengerManagement.Commands;
using Pitstop.Infrastructure.Messaging;
using System;
using System.Collections.Generic;
using System.Text;

namespace AirSupport.Application.PassengerManagement.Events
{
    public class PassengerUpdated : Event
    {
        public readonly int Id;
        public readonly bool CheckedIn;

        public PassengerUpdated(Guid messageId, int id, bool checkedIn) : 
            base(messageId)
        {
            Id = id;
            CheckedIn = checkedIn;
        }

        public static PassengerUpdated FromCommand(UpdatePassenger command)
        {
            return new PassengerUpdated(
                Guid.NewGuid(),
                command.Id,
                command.CheckedIn
            );
        }
    }
}
