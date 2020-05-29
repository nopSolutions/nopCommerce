using System;
using Nop.Services.Events;

namespace Nop.Web.MVC.Tests.Events
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
