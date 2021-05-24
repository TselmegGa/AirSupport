using Pitstop.Infrastructure.Messaging;
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AirSupport.Application.PassengerManagement.Stubs
{
    public class FlightArrivedStub{
        
        [Required]
        public readonly int Id;
        [Required]
        public readonly String ArrivalGate;
        
        [Required]
        public readonly DateTime ArrivalDate;
        public FlightArrivedStub(int id,String arrivalGate, DateTime arrivalDate) : base()
        {
            Id = id;
            ArrivalGate = arrivalGate;
            ArrivalDate = arrivalDate;
        }

    }
}