using Pitstop.Infrastructure.Messaging;
using System;
using System.Collections.Generic;
using System.Text;

namespace AirSupport.Application.PassengerManagement.Commands
{
    public class SetPassengerCheckedIn : Command
    {
        public readonly int Id;
        public readonly bool CheckedIn;
        public SetPassengerCheckedIn(Guid messageId, int id, bool checkedIn) :
            base(messageId)
        {
            Id = id;
            CheckedIn = checkedIn;
        }
    }
}