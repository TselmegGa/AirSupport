
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pitstop.LuggageManagment.Model
{
    public class Passenger
    {

        
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

       
        public virtual ICollection<Luggage> Luggage {get;set;}
    }
}
