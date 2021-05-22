using System;
using Pitstop.Infrastructure.Messaging;

namespace Pitstop.LuggageManagment.Events
{
    class PlaneArrived : Event
    {
        //public List<Object> passangers;
        public string passangers;

        public PlaneArrived(Guid messageId, string passangers) : base(messageId)
        {
            Console.WriteLine("ArrivedPlaneEvent");
            this.passangers = passangers;
        }
    }
}

