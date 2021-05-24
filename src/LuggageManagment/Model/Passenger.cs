
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pitstop.LuggageManagment.Model
{
    public class Passenger
    {

        [Key]
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        [Required]
        public virtual ICollection<Luggage> Luggage {get;set;}
    }
}
