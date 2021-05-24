using Pitstop.Infrastructure.Messaging;
using System;

namespace Pitstop.InvoiceService.Events
{
    public class RentalRegistered : Event
    {
        public readonly string Id;
        public readonly string Name;


        public RentalRegistered(Guid messageId, string id, string name) : base(messageId)
        {
            Id = id;
            Name = name;
        }
    }
}
