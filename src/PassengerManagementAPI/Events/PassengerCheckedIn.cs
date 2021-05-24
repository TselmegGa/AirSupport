using AirSupport.Application.PassengerManagement.Model;
using Pitstop.Infrastructure.Messaging;
using System;
using System.Collections.Generic;
using System.Text;

namespace AirSupport.Application.PassengerManagement.Events
{
    public class PassengerCheckedIn : Event
    {
        public readonly Passenger Passenger;
        public readonly bool CheckedIn;

        public PassengerCheckedIn(Guid messageId, Passenger passenger, bool checkedIn) :
            base(messageId)
        {
            Passenger = passenger;
            CheckedIn = checkedIn;
        }

        public static PassengerCheckedIn FromPassenger(Passenger command, bool CheckedIn)
        {
            return new PassengerCheckedIn(
                Guid.NewGuid(),
                command,
                CheckedIn
            );
        }
    }
}
