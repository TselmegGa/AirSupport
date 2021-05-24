
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pitstop.LuggageManagment.Model
{
    public class Passenger
    {

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool Arrived {get; set;} 
       
        public virtual ICollection<Luggage> Luggage {get;set;}
    }
}

