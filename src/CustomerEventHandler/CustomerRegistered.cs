using System;
using Pitstop.Infrastructure.Messaging;

namespace CustomerEventHandler
{
    public class CustomerRegistered : Event
    {
        public string name;
        public string id;
        public CustomerRegistered(string id, string name) : base()
        {
            this.name = name;
            this.id = id;
        }

    }
}