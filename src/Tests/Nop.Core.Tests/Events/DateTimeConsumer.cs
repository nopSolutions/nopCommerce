using System;
using Nop.Core.Events;

namespace Nop.Core.Tests.Events
{
    public class DateTimeConsumer : IConsumer<DateTime>
    {
        public void HandleEvent(DateTime eventMessage)
        {
            DateTime = eventMessage;
        }

        // For testing
        public static DateTime DateTime { get; set; }
    }
}
