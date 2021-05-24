using System;
using Pitstop.Infrastructure.Messaging;
using Pitstop.LuggageManagment.Model;

namespace Pitstop.LuggageManagment.Events
{
    class RegisterPassenger : Event
    {
        public readonly int Id;
        public readonly string FirstName;
        public readonly string LastName;

        public RegisterPassenger(Guid messageId, int id, string firstName, string lastName) : base(messageId)
        {
            Console.WriteLine("RegisterPassenger Event");
            Console.WriteLine(firstName+lastName);
            this.Id = id;
            this.FirstName = firstName;
            this.LastName = lastName;
        }
    }
}

