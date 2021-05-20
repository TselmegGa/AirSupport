using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirSupport.Application.PassengerManagementCommands.Model
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
        public String ArrivalGate{get; set;}
        
        [Required]
        public DateTime ArrivalDate{get;set;}
    }
}
