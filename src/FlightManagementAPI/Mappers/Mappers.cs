using AirSupport.Application.FlightManagement.Commands;
using AirSupport.Application.PassengerManagement.Events;
using System;

namespace AirSupport.Application.FlightManagement.Mappers
{
    public static class Mappers
    {
        public static FlightRegistered MapToFlight(this RegisterFlight command) => new FlightRegistered
        (
            System.Guid.NewGuid(),
            command.DepartureDate,
            command.Destination,
            command.Origin
        );

        public static GateAssigned MapToGate(this AssignGate command) => new GateAssigned
        (
            Guid.NewGuid(),
            command.FlightId,
            command.Gate
        );

        public static CounterAssigned MapToCounter(this AssignCounter command) => new CounterAssigned
        (
            Guid.NewGuid(),
            command.FlightId,
            command.Counter
        );
    }
}
