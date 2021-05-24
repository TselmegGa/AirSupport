using System;
using AirSupport.Application.RentalManagementAPI.Commands;
using AirSupport.Application.RentalManagementAPI.Domain.Entities;
using AirSupport.Application.RentalManagementAPI.Domain.Exceptions;
using AirSupport.Application.RentalManagementAPI.Domain.ValueObjects;

namespace AirSupport.Application.RentalManagementAPI.Domain.BusinessRules
{
    public static class RentalRules
    {
        public static void RentedShopMustHaveName(this RegisterRental rental)
        {
            if (rental.name != "none")
            {
                throw new BusinessRuleViolationException($"A rented shop must have a name.");
            }
        }

    }
}