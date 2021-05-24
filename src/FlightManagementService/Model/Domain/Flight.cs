using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AirSupport.Application.PassengerManagement.Model.Domain
{
    public class FlightAgregate
    {
        public DateTime TimeStamp { get; set; }
        public int FlightId { get; set; }
        public string Gate { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }
        public string Counter { get; set; }
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }

        public FlightAgregate()
        {
            TimeStamp = DateTime.MinValue;
        }
    }
}
