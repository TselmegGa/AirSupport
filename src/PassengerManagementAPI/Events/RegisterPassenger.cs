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

        public PassengerRegistered(Guid messageId, int id) : 
            base(messageId)
        {
            Id = id;
        }

        public static PassengerRegistered FromCommand(RegisterPassenger command)
        {
            return new PassengerRegistered(
                Guid.NewGuid(),
                command.Id
            );
        }
    }
}
