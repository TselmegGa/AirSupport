using Pitstop.Infrastructure.Messaging;
using System;
using System.Collections.Generic;
using System.Text;

namespace AirSupport.Application.RentalManagementAPI.Commands
{
    public class RegisterShop : Command
    {
        public readonly string Name;
        public readonly string Location;

        public RegisterShop(Guid messageId, string location) :
            base(messageId)
        {

            Name = "none";
            Location = location;
        }
    }
}