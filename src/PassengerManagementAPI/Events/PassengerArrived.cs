using AirSupport.Application.PassengerManagement.Commands;
using AirSupport.Application.PassengerManagement.Model;
using Pitstop.Infrastructure.Messaging;
using System;
using System.Collections.Generic;
using System.Text;

namespace AirSupport.Application.PassengerManagement.Events
{
    public class PassengerArrived : Event
    {
        public readonly Passenger Passenger;

        public PassengerArrived(Guid messageId,Passenger passenger) : 
            base(messageId)
        {
            Passenger = passenger;
        }

        public static PassengerArrived FromPassenger(Passenger command)
        {
            return new PassengerArrived(
                Guid.NewGuid(),
                command
            );
        }
    }
}
