using System;
using Pitstop.Infrastructure.Messaging;

namespace Pitstop.CustomerEventHandler
{
    class CustomerRegistered : Event
    {
        public string name;
        public string CustomerId;

        public CustomerRegistered(Guid messageId, string CustomerId, string name) : base(messageId)
        {
            this.name = name;
            this.CustomerId = CustomerId;
        }
    }
}