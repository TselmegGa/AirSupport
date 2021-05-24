using System;
using Pitstop.Infrastructure.Messaging;

namespace AirSupport.RentalManagement
{
    class RentalStopped : Event
    {
        public string name;
        public int Id;


        public RentalStopped(Guid messageId, int id) : base(messageId)
        {
            this.name = "None";
            Id = id;
        }
    }
}