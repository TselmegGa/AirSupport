using Pitstop.Infrastructure.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pitstop.TimeService.Events
{
    public class _15MinutesHasPassed : Event
    {
        public _15MinutesHasPassed(Guid messageId) : base(messageId)
        {
        }
    }
}