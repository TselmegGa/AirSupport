using AirSupport.Application.RentalManagementAPI.Domain.Core;

namespace AirSupport.Application.RentalManagementAPI.Domain.Entities
{
    public class Rental : Entity<int>
    {
        public string Name { get;  set; }
        public string Location { get;  set; }

        public Rental(int Id, string name) : base(Id)
        {
            Name = name;
            
        }
    }
}
