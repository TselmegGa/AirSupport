using AirSupport.Application.PassengerManagement.Commands;
using Pitstop.Infrastructure.Messaging;
using System;
using System.Collections.Generic;
using System.Text;

namespace AirSupport.Application.PassengerManagement.Events
{
    public class PassengerRegistered : Event
    {
        public readonly int Id;
        public readonly bool CheckedIn;

        public PassengerRegistered(Guid messageId, int id, bool checkedIn) : 
            base(messageId)
        {
            Id = id;
            CheckedIn = checkedIn;
        }

        public static PassengerRegistered FromCommand(RegisterPassenger command)
        {
            return new PassengerRegistered(
                Guid.NewGuid(),
                command.Id,
                command.CheckedIn
            );
        }
    }
}
