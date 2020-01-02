using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using Moq;
using Nop.Core;
using Nop.Core.Infrastructure;
using Nop.Services.Events;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Web.MVC.Tests.Events
{
    [TestFixture]
    public class EventsTests
    {
        private IEventPublisher _eventPublisher;

        [OneTimeSetUp]
        public void SetUp()
        {
            var hostingEnvironment = new Mock<IHostingEnvironment>();
            hostingEnvironment.Setup(x => x.ContentRootPath).Returns(System.Reflection.Assembly.GetExecutingAssembly().Location);
            hostingEnvironment.Setup(x => x.WebRootPath).Returns(System.IO.Directory.GetCurrentDirectory());
            CommonHelper.DefaultFileProvider = new NopFileProvider(hostingEnvironment.Object);

            var nopEngine = new Mock<NopEngine>();
            var serviceProvider = new TestServiceProvider();
            nopEngine.Setup(x => x.ServiceProvider).Returns(serviceProvider);
            nopEngine.Setup(x => x.ResolveAll<IConsumer<DateTime>>()).Returns(new List<IConsumer<DateTime>> { new DateTimeConsumer() });
            EngineContext.Replace(nopEngine.Object);
            _eventPublisher = new EventPublisher();
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