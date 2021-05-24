using Pitstop.Infrastructure.Messaging;
using AirSupport.Application.RentalManagementAPI.Commands;
using AirSupport.Application.RentalManagementAPI.Domain.BusinessRules;
using AirSupport.Application.RentalManagementAPI.Domain.Core;
using AirSupport.Application.RentalManagementAPI.Domain.Exceptions;
using AirSupport.Application.RentalManagementAPI.Domain.ValueObjects;
using AirSupport.Application.RentalManagementAPI.Events;
using AirSupport.Application.RentalManagementAPI.Model;
using AirSupport.Application.RentalManagementAPI.Mappers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AirSupport.Application.RentalManagementAPI.Domain.Entities
{
    public class RentalPlanning : AggregateRoot<string>
    {
        public  Rental _rental { get; private set; }
        public RentalPlanning(string location) : base(location) { }

        public RentalPlanning(string location, IEnumerable<Event> events) : base(location, events) { }

        /// <summary>
        /// Creates a new instance of a workshop-planning for the specified date.
        /// </summary>
        /// <param name="date">The date to create the planning for.</param>
        public static RentalPlanning Create(string location)
        {
            RentalPlanning planning = new RentalPlanning(location);
            ShopRegistered e = new ShopRegistered(Guid.NewGuid(),"none", location);
            planning.RaiseEvent(e);
            return planning;
        }

        public void StartRent(RegisterRental command)
        {
            if (_rental == null){
                throw new RentalNotFoundException($"Shop wasnt found.");
            }



            // handle event
            RentalRegistered e = RentalRegistered.FromCommand(command);
            RaiseEvent(e);
        }

        public void StopRent(StopRental command)
        {
            // find job
            if (_rental == null)
            {
                throw new RentalNotFoundException($"Shop wasnt found.");
            }
            if (_rental.Id != command.Id){
                throw new RentalNotFoundException($"Shop wasnt found at this location.");
            }
        

            // handle event
            RentalStopped e = RentalStopped.FromCommand(command);
            RaiseEvent(e);
        }

        /// <summary>
        /// Handles an event and updates the aggregate version.
        /// </summary>
        /// <remarks>Caution: this handles is also called while replaying events to restore state.
        /// So, do not execute any checks that could fail or introduce any side-effects in this handler.</remarks>
        protected override void When(dynamic @event)
        {
            Handle(@event);
        }

        private void Handle(ShopRegistered e)
        {
            _rental = new Rental(0,e.Name);
            _rental.Location = e.Location;
        }

        private void Handle(RentalRegistered e)
        {
            _rental.Name = e.name;
            _rental.Id = e.Id;
        }

        private void Handle(RentalStopped e)
        {
            _rental.Name = "none";
            _rental.Id = e.Id;
        }
    }
}
