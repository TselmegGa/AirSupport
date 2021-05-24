using Pitstop.Infrastructure.Messaging;
using AirSupport.Application.RentalManagementAPI.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AirSupport.Application.RentalManagementAPI.Repositories
{
    public interface IRentalPlanningRepository
    {
        void EnsureDatabase();
        Task<RentalPlanning> GetRentalPlanningAsync(string location);
        Task SaveRentalPlanningAsync(string planningId, int originalVersion, int newVersion, IEnumerable<Event> newEvents);
    }
}
