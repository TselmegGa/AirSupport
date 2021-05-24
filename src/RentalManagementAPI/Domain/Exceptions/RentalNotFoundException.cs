using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AirSupport.Application.RentalManagementAPI.Domain.Exceptions
{
    public class RentalNotFoundException : Exception
    {
        public RentalNotFoundException()
        {
        }

        public RentalNotFoundException(string message) : base(message)
        {
        }

        public RentalNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
