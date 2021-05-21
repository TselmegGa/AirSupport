using AirSupport.Application.PassengerManagementCommands.Model;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Pitstop.Infrastructure.Messaging;
using System;
using System.Collections.Generic;
using System.Text;

namespace AirSupport.Application.PassengerManagementCommands.Commands
{
    public class CommandRegisterFlight : Command
    {

        public readonly int Id;
        public readonly String Origin;
        public readonly String Destination;
        public readonly DateTime DepartureDate;
        public CommandRegisterFlight(Guid messageId, int id, String origin, String destination, DateTime departureDate) :
            base(messageId)
        {
            Id = id;
            Origin = origin;
            Destination = destination;
            DepartureDate = departureDate;
        }

        public bool isValid(){
            bool isValid = true;
            if(Id<=0){
                isValid =false;
            } else if(Origin.Equals("")){
                isValid = false;
            } else if(Destination.Equals("")){
                isValid = false;
            } else if(DepartureDate < new DateTime()){
                isValid = false;
            }
            return isValid;
        }
    }
}
