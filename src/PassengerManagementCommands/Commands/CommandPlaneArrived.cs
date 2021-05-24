using AirSupport.Application.PassengerManagementCommands.Model;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Pitstop.Infrastructure.Messaging;
using System;
using System.Collections.Generic;
using System.Text;

namespace AirSupport.Application.PassengerManagementCommands.Commands
{
    public class CommandPlaneArrived : Command
    {
        public readonly Flight Flight;
        [Required]
        public readonly DateTime ArrivalDate;
        [Required]
        public readonly String ArrivalGate;

        public CommandPlaneArrived(Guid messageId,Flight flight, DateTime arrivalDate, String arrivalGate) : 
            base(messageId)
        {
            Flight = flight;
            ArrivalDate = arrivalDate;
            ArrivalGate = arrivalGate;
        }
    }
}
