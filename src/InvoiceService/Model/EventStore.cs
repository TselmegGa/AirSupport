using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pitstop.InvoiceService.Model
{
    public class EventStore
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }
        public string Event { get; set; }
        public string EventBody { get; set; }
        public DateTime EventDate { get; set; }
    }
}
