using Pitstop.Infrastructure.Messaging;
using System;
using System.Collections.Generic;
using System.Text;

namespace AirSupport.Application.RentalManagementAPI.Commands
{
    public class StopRental : Command
    {
        public readonly int Id;
        public StopRental(Guid messageId, int id) :
            base(messageId)
        {
            Id = id;
            
        }
    }
}