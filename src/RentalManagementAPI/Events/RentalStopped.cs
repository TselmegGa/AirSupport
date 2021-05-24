using AirSupport.Application.RentalManagementAPI.Commands;
using Pitstop.Infrastructure.Messaging;
using System;
using System.Collections.Generic;
using System.Text;

namespace AirSupport.Application.RentalManagementAPI.Events
{
    public class RentalStopped : Event
    {
        public readonly int Id;


        public RentalStopped(Guid messageId, int id) : 
            base(messageId)
        {
            Id = id;
        }

        public static RentalStopped FromCommand(StopRental command)
        {
            return new RentalStopped(
                Guid.NewGuid(),
                command.Id
            );
        }
    }
}
