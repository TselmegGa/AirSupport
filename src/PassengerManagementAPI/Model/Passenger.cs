using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirSupport.Application.PassengerManagement.Model
{
    public class Passenger
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MinLength(1)]
        public String FirstName { get; set; }
        [Required]

        [MinLength(1)]
        public String LastName { get; set; }
        [Required]
        [EmailAddress]
        public String Email { get; set; }
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime BirthDate { get; set; }
        [Required]
        public String PhoneNumber { get; set; }
        [Required]
        public String CellNumber { get; set; }
        [Required]
        public String Gender { get; set; }
        [Required]
        public String Nationality { get; set; }
        public bool CheckedIn { get; set; }

        [Required]
        public int FlightId { get; set; }
        [ForeignKey("FlightId")]
        public virtual Flight Flight { get; set; }

        [NotMapped]
        public string FullName
        {
            get
            {
                return FirstName + " " + LastName;
            }
        }

    }
}
