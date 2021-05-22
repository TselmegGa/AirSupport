using System;
using Pitstop.Infrastructure.Messaging;

namespace Pitstop.LuggageManagment.Events
{
    class PassangerToLate : Event
    {
        //public List<Object> passangers;
        public string passangers;

        public PassangerToLate(Guid messageId, string passangers) : base(messageId)
        {
            Console.WriteLine("PassangerToLateEvent");
            this.passangers = passangers;
        }
    }
}