using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Nop.Core;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Core.Plugins;
using Nop.Services.Events;
using NUnit.Framework;
using Rhino.Mocks;

namespace Nop.Web.MVC.Tests.Events
{
    [TestFixture]
    public class EventsTests
    {
        private IEventPublisher _eventPublisher;

        [OneTimeSetUp]
        public void SetUp()
        {
            var hostingEnvironment = MockRepository.GenerateMock<IHostingEnvironment>();
            hostingEnvironment.Expect(x => x.ContentRootPath).Return(System.Reflection.Assembly.GetExecutingAssembly().Location);
            hostingEnvironment.Expect(x => x.WebRootPath).Return(System.IO.Directory.GetCurrentDirectory());
            CommonHelper.DefaultFileProvider = new NopFileProvider(hostingEnvironment);
            PluginManager.Initialize(new ApplicationPartManager(), new NopConfig());

            var subscriptionService = MockRepository.GenerateMock<ISubscriptionService>();
            var consumers = new List<IConsumer<DateTime>> {new DateTimeConsumer()};
            subscriptionService.Expect(c => c.GetSubscriptions<DateTime>()).Return(consumers);
            _eventPublisher = new EventPublisher(subscriptionService);
        }

        [Test]
        public void Can_publish_event()
        {
            var oldDateTime = DateTime.Now.Subtract(TimeSpan.FromDays(7));
            DateTimeConsumer.DateTime = oldDateTime;

            var newDateTime = DateTime.Now.Subtract(TimeSpan.FromDays(5));
            _eventPublisher.Publish(newDateTime);
            Assert.AreEqual(DateTimeConsumer.DateTime, newDateTime);
        }
    }
}