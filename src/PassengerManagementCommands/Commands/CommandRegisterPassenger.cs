using Pitstop.Infrastructure.Messaging;
using System;
using System.Collections.Generic;
using System.Text;

namespace AirSupport.Application.PassengerManagementCommands.Commands
{
    public class CommandRegisterPassenger : Command
    {
        public readonly String FirstName;
        public readonly String LastName;
        public readonly String Email;
        public readonly DateTime BirthDate;
        public readonly String PhoneNumber;
        public readonly String CellNumber;
        public readonly String Gender;
        public readonly String Nationality;

        public readonly bool CheckedIn;
        public readonly int FlightId;
        public CommandRegisterPassenger(Guid messageId, String firstName, String lastName, String email, DateTime birthDate, String phoneNumber, String cellNumber, String gender, String nationality, int flightId) :
            base(messageId)
        {
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            BirthDate = birthDate;
            PhoneNumber = phoneNumber;
            CellNumber = cellNumber;
            Gender = gender;
            CheckedIn = false;
            Nationality = nationality;
            FlightId = flightId;
        }
    }
}