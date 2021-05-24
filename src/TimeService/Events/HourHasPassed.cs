using Pitstop.Infrastructure.Messaging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pitstop.TimeService.Events
{
    public class HourHasPassed : Event
    {
        public HourHasPassed(Guid messageId) : base(messageId)
        {
        }
    }
}
