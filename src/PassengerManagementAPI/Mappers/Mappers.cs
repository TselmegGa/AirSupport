using AirSupport.Application.PassengerManagement.Commands;
using AirSupport.Application.PassengerManagement.Model;

namespace AirSupport.PassengerManagementAPI.Mappers
{
    public static class Mappers
    {
        public static Passenger MapToPassenger(this RegisterPassenger command) => new Passenger
        {
            Id = command.Id,
            CheckedIn = command.CheckedIn
        };
    }
}