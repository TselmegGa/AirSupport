using AirSupport.Application.PassengerManagementCommands.Commands;
using AirSupport.Application.PassengerManagementCommands.Model;
using Pitstop.Infrastructure.Messaging;
using System;
using System.Collections.Generic;
using System.Text;

namespace AirSupport.Application.PassengerManagementCommands.Events
{
    public class RegisterPassenger : Event
    {
        public readonly int Id;
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

        public RegisterPassenger(Guid messageId, int id, String firstName, String lastName, String email, DateTime birthDate, String phoneNumber, String cellNumber, String gender, String nationality,bool checkedIn, int flightId) : 
            base(messageId)
        {
            Id = id;
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

public static RegisterPassenger FromPassenger(Passenger command)
        {
            return new RegisterPassenger(
                Guid.NewGuid(),
                command.Id,
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
