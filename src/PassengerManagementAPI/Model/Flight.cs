using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirSupport.Application.PassengerManagement.Model
{
    public class Flight
    {
        [Key]
        [Required]
        public int Id { get; set; }
        
        [Required]
        public String Origin{get;set;}
        
        [Required]
        public String Destination{get;set;}
        [Required]
        public DateTime DepartureDate{get;set;}
        public String? ArrivalGate{get; set;}
        
        public DateTime? ArrivalDate{get;set;}

        public virtual ICollection<Passenger> Passengers { get; set; }
    }
}
