using System;
using Pitstop.Infrastructure.Messaging;
using Pitstop.LuggageManagment.Model;

namespace Pitstop.LuggageManagment.Events
{
    class PlaneArrived : Event
    {
        public Plane plane;

        public PlaneArrived(Guid messageId, Plane plane) : base(messageId)
        {
            Console.WriteLine("ArrivedPlaneEvent");
            this.plane = plane;
        }
    }
}

