using AirSupport.Application.RentalManagementAPI.Commands;
using Pitstop.Infrastructure.Messaging;
using System;
using System.Collections.Generic;
using System.Text;

namespace AirSupport.Application.RentalManagementAPI.Events
{
    public class ShopRegistered : Event
    {

        public readonly string Name;
        public readonly string Location;

        public ShopRegistered(Guid messageId, string name, string location) : 
            base(messageId)
        {

            Name = name;
            Location = location;
        }

        public static ShopRegistered FromCommand(RegisterShop command)
        {
            return new ShopRegistered(
                Guid.NewGuid(),
                command.Name,
                command.Location
            );
        }
    }
}
