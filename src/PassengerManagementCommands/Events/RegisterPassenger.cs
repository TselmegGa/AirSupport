using AirSupport.Application.PassengerManagementCommands.Commands;
using Pitstop.Infrastructure.Messaging;
using System;
using System.Collections.Generic;
using System.Text;

namespace AirSupport.Application.PassengerManagementCommands.Events
{
    public class RegisterPassenger : Event
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

        public RegisterPassenger(Guid messageId, String firstName, String lastName, String email, DateTime birthDate, String phoneNumber, String cellNumber, String gender, String nationality,bool checkedIn, int flightId) : 
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

        public static RegisterPassenger FromCommand(CommandRegisterPassenger command)
        {
            return new RegisterPassenger(
                Guid.NewGuid(),
                command.FirstName,
                command.LastName,
                command.Email,
                command.BirthDate,
                command.PhoneNumber,
                command.CellNumber,
                command.Gender,
                command.Nationality,
                command.CheckedIn,
                command.FlightId
            );
        }
    }
}
