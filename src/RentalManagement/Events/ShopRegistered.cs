using System;
using Pitstop.Infrastructure.Messaging;

namespace AirSupport.RentalManagement
{
    class ShopRegistered : Event
    {
        public string name;
        public string Location;
        public ShopRegistered(Guid messageId, string location) : base(messageId)
        {
            this.name = "None";
            this.Location = location;
        }
    }
}