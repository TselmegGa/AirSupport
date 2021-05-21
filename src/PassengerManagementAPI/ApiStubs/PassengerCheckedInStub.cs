using Pitstop.Infrastructure.Messaging;
using System;
using System.Collections.Generic;
using System.Text;

namespace AirSupport.Application.PassengerManagement.Stubs
{
    public class PassengerCheckedInStub
    {
        public readonly int Id;
        public readonly bool CheckedIn;
        public PassengerCheckedInStub(int id, bool checkedIn) :
            base()
        {
            Id = id;
            CheckedIn = checkedIn;
        }
    }
}