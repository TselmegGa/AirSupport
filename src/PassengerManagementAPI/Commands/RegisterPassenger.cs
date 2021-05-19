using Pitstop.Infrastructure.Messaging;
using System;
using System.Collections.Generic;
using System.Text;

namespace AirSupport.Application.PassengerManagement.Commands
{
    public class RegisterPassenger : Command
    {
        public readonly int Id;
        public readonly bool CheckedIn;
        public RegisterPassenger(Guid messageId, int id, bool checkedIn) :
            base(messageId)
        {
            Id = id;
            CheckedIn = false;
        }
    }
}