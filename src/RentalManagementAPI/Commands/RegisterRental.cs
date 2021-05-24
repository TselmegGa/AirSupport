using Pitstop.Infrastructure.Messaging;
using System;
using System.Collections.Generic;
using System.Text;

namespace AirSupport.Application.RentalManagementAPI.Commands
{
    public class RegisterRental : Command
    {
        public readonly int Id;
        public readonly string name;
        
        public RegisterRental(Guid messageId, int id, string name) :
            base(messageId)
        {
            Id = id;
            this.name = name;
        }
    }
}