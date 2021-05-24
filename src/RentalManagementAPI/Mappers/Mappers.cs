using AirSupport.Application.RentalManagementAPI.Commands;
using AirSupport.Application.RentalManagementAPI.Model;

namespace AirSupport.Application.RentalManagementAPI.Mappers
{
    public static class Mappers
    {
        public static Rental MapToRental(this RegisterRental command) => new Rental
        {
            Id = command.Id,
            Name = command.name,
        };
        public static Rental MapToRental(this StopRental command) => new Rental
        {
            Id = command.Id,

        };
        public static Rental MapToRental(this RegisterShop command) => new Rental
        {
            Name = command.Name,
            Location = command.Location
        };
        
    }
}