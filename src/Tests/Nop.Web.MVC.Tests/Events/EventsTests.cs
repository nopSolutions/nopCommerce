using System;
using FluentAssertions;
using Nop.Services.Events;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Web.MVC.Tests.Events
{
    [TestFixture]
    public class EventsTests : BaseNopTest
    {
        private IEventPublisher _eventPublisher;

        [OneTimeSetUp]
        public void SetUp()
        {
            _eventPublisher = GetService<IEventPublisher>();
        }

        [Test]
        public void CanPublishEvent()
        {
            var oldDateTime = DateTime.Now.Subtract(TimeSpan.FromDays(7));
            DateTimeConsumer.DateTime = oldDateTime;

            var newDateTime = DateTime.Now.Subtract(TimeSpan.FromDays(5));
            _eventPublisher.Publish(newDateTime);
            newDateTime.Should().Be(DateTimeConsumer.DateTime);
        }

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
}