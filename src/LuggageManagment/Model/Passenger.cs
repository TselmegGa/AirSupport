
using System.Collections.Generic;

namespace Pitstop.LuggageManagment.Model
{
    public class Passenger
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public List<Luggage> Luggage {get;set;}
    }
}
