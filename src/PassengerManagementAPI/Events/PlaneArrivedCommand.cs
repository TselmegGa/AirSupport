using AirSupport.Application.PassengerManagement.Model;
using Pitstop.Infrastructure.Messaging;
using System;
using System.Collections.Generic;
using System.Text;

namespace AirSupport.Application.PassengerManagement.Events
{
    public class PlaneArrivedCommand : Event
    {
        public readonly Flight Flight;
        public readonly DateTime ArrivalDate;
        public readonly String ArrivalGate;

        public PlaneArrivedCommand(Guid messageId,Flight flight,DateTime arrivalDate, String arrivalGate) : 
            base(messageId)
        {
            Flight = flight;
            ArrivalDate= arrivalDate;
            ArrivalGate = arrivalGate;
        }

        public static PlaneArrivedCommand FromFlight(Flight command, DateTime date, String arrivalGate)
        {
            return new PlaneArrivedCommand(
                Guid.NewGuid(),
                command,
                date,
                arrivalGate
            );
        }
    }
}
