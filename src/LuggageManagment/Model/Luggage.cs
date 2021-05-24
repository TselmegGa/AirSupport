
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Pitstop.LuggageManagment.Model
{
    public class Luggage
    {
     [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
     [Key]
        public int LuggageId { get; set; }
        [Required]
        public int Weight { get; set; }
        [Required]
        public string Brand { get; set; }
        [Required]
        public string Color { get; set; }
    }
}
