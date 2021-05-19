using AirSupport.Application.PassengerManagement.Commands;
using AirSupport.Application.PassengerManagement.Model;

namespace AirSupport.PassengerManagementAPI.Mappers
{
    public static class Mappers
    {
        public static Passenger MapToVehicle(this RegisterPassenger command) => new Passenger
        {
            Id = command.Id
            // LicenseNumber = command.LicenseNumber,
            // Brand = command.Brand,
            // Type = command.Type,
            // OwnerId = command.OwnerId
        };
    }
}