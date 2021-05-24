using System;
using Pitstop.Infrastructure.Messaging;

namespace AirSupport.RentalManagement
{
    class RentalRegistered : Event
    {
        public string name;
        public int Id;


        public RentalRegistered(Guid messageId, int id, string name) : base(messageId)
        {
            this.name = name;
            Id = id;
        }
    }
}