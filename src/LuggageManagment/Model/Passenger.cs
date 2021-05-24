
using System.Collections.Generic;

namespace Pitstop.LuggageManagment.Model
{
    public class Passenger
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public virtual List<Luggage> Luggage {get;set;}
    }
}
