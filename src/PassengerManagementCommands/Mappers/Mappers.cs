using AirSupport.Application.PassengerManagementCommands.Commands;
using AirSupport.Application.PassengerManagementCommands.Model;

namespace AirSupport.PassengerManagementCommands.Mappers
{
    public static class Mappers
    {
        public static Passenger MapToPassenger(this CommandRegisterPassenger command) => new Passenger
        {
            FirstName = command.FirstName,
            LastName = command.LastName,
            Email = command.Email,
            BirthDate = command.BirthDate,
            PhoneNumber = command.PhoneNumber,
            CellNumber = command.CellNumber,
            Gender = command.Gender,
            Nationality = command.Nationality,
            CheckedIn = command.CheckedIn,
            FlightId = command.FlightId
        };

        public static Flight MapToFlight(this CommandFlightAssigned command)=> new Flight{
            Id =command.Id,
            Origin = command.Origin,
            Destination = command.Destination,
            ArrivalGate = command.ArrivalGate,
            ArrivalDate = command.ArrivalDate
        };
        public static Flight MapToFlight(this CommandRegisterFlight command)=> new Flight{
            Id =command.Id,
            Origin = command.Origin,
            Destination = command.Destination,
            DepartureDate = command.DepartureDate
        };
    }
}