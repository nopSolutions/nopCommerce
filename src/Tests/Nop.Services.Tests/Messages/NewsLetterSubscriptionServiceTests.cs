using System;
using FluentAssertions;
using Nop.Core.Domain.Messages;
using Nop.Core.Events;
using Nop.Services.Events;
using Nop.Services.Messages;
using NUnit.Framework;

namespace Nop.Services.Tests.Messages 
{
    [TestFixture]
    public class NewsLetterSubscriptionServiceTests : ServiceTest
    {
        private INewsLetterSubscriptionService _newsLetterSubscriptionService;

        [SetUp]
        public void SetUp()
        {
            _newsLetterSubscriptionService = GetService<INewsLetterSubscriptionService>();
        }

        /// <summary>
        /// Verifies the active insert triggers subscribe event.
        /// </summary>
        [Test]
        public void VerifyActiveInsertTriggersSubscribeEvent()
        {
            var subscription = new NewsLetterSubscription { Active = true, Email = "test@test.com" };
            _newsLetterSubscriptionService.InsertNewsLetterSubscription(subscription);

            var eventType = NewsLetterSubscriptionConsumer.LastEventType;

            _newsLetterSubscriptionService.DeleteNewsLetterSubscription(subscription);

            eventType.Should().Be(typeof(EmailSubscribedEvent));
        }

        /// <summary>
        /// Verifies the delete triggers unsubscribe event.
        /// </summary>
        [Test]
        public void VerifyDeleteTriggersUnsubscribeEvent()
        {
            var subscription = new NewsLetterSubscription { Active = true, Email = "test@test.com" };
            _newsLetterSubscriptionService.InsertNewsLetterSubscription(subscription);
            _newsLetterSubscriptionService.DeleteNewsLetterSubscription(subscription);

            NewsLetterSubscriptionConsumer.LastEventType.Should().Be(typeof(EmailUnsubscribedEvent));
        }
        
        /// <summary>
        /// Verifies the insert event is fired.
        /// </summary>
        [Test]
        public void VerifyInsertEventIsFired()
        {
            var subscription = new NewsLetterSubscription { Email = "test@test.com" };

            _newsLetterSubscriptionService.InsertNewsLetterSubscription(subscription);

            var eventType = NewsLetterSubscriptionConsumer.LastEventType;

            _newsLetterSubscriptionService.DeleteNewsLetterSubscription(subscription);

            eventType.Should().Be(typeof(EntityInsertedEvent<NewsLetterSubscription>));
        }

        public class NewsLetterSubscriptionConsumer : IConsumer<EmailSubscribedEvent>, IConsumer<EmailUnsubscribedEvent>, IConsumer<EntityInsertedEvent<NewsLetterSubscription>>
        {
            public static Type LastEventType { get; set; }

            public void HandleEvent(EmailSubscribedEvent eventMessage)
            {
                LastEventType = typeof(EmailSubscribedEvent);
            }

            public void HandleEvent(EmailUnsubscribedEvent eventMessage)
            {
                LastEventType = typeof(EmailUnsubscribedEvent);
            }

            public void HandleEvent(EntityInsertedEvent<NewsLetterSubscription> eventMessage)
            {
                LastEventType = typeof(EntityInsertedEvent<NewsLetterSubscription>);
            }
        }
    }
}