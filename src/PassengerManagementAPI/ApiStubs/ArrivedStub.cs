using Pitstop.Infrastructure.Messaging;
using System;
using System.Collections.Generic;
using System.Text;

namespace AirSupport.Application.PassengerManagement.Stubs
{
    public class ArrivedStub
    {
        public readonly DateTime ArrivalDate;
        public ArrivedStub(DateTime arrivalDate) :
            base()
        {
            ArrivalDate = arrivalDate;
        }
    }
}