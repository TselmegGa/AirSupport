using System;
using System.Collections.Generic;
using Pitstop.Infrastructure.Messaging;
using Pitstop.LuggageManagment.Model;

namespace Pitstop.LuggageManagment.Events
{
    class LuggageClaimedByPassenger : Event
    {
        public readonly int LuggageId;

        public LuggageClaimedByPassenger(Guid messageId, int luggageId) : base(messageId)
        {
        
            Console.WriteLine("LuggageClaimedByPassenger");
            this.LuggageId = luggageId;
        }
    }
}