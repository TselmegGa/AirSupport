using System;
using System.Collections.Generic;
using Pitstop.Infrastructure.Messaging;
using Pitstop.LuggageManagment.Model;

namespace Pitstop.LuggageManagment.Events
{
    class LuggageDeliveredByPassenger : Event
    {
        public readonly int PassengerId;
        public readonly int Weight;
        public readonly string Brand;
        public readonly string Color;

        public LuggageDeliveredByPassenger(Guid messageId, int passengerId,int weight, string brand, string color) : base(messageId)
        {
        
            Console.WriteLine("LuggageDeliveredByPassenger");
            this.PassengerId = passengerId;
            this.Weight = weight;
            this.Brand =brand;
            this.Color = color;
        }
    }
}