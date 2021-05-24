using AirSupport.Application.RentalManagementAPI.Commands;
using Pitstop.Infrastructure.Messaging;
using System;
using System.Collections.Generic;
using System.Text;

namespace AirSupport.Application.RentalManagementAPI.Events
{
    public class RentalRegistered : Event
    {
        public readonly int Id;
        public readonly string name;

        public RentalRegistered(Guid messageId, int id, string name) : 
            base(messageId)
        {
            Id = id;
            this.name = name;
        }

        public static RentalRegistered FromCommand(RegisterRental command)
        {
            return new RentalRegistered(
                Guid.NewGuid(),
                command.Id,
                command.name
            );
        }
    }
}
