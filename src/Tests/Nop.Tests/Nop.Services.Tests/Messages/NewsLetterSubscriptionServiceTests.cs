using System;
using System.Threading.Tasks;
using FluentAssertions;
using Nop.Core.Domain.Messages;
using Nop.Core.Events;
using Nop.Services.Events;
using Nop.Services.Messages;
using NUnit.Framework;

namespace Nop.Tests.Nop.Services.Tests.Messages 
{
    [TestFixture]
    public class NewsLetterSubscriptionServiceTests : ServiceTest
    {
        private INewsLetterSubscriptionService _newsLetterSubscriptionService;

        [OneTimeSetUp]
        public void SetUp()
        {
            _newsLetterSubscriptionService = GetService<INewsLetterSubscriptionService>();
        }

        /// <summary>
        /// Verifies the active insert triggers subscribe event.
        /// </summary>
        [Test]
        public async Task VerifyActiveInsertTriggersSubscribeEvent()
        {
            var subscription = new NewsLetterSubscription { Active = true, Email = "test@test.com" };
            await _newsLetterSubscriptionService.InsertNewsLetterSubscriptionAsync(subscription);

            var eventType = NewsLetterSubscriptionConsumer.LastEventType;

            await _newsLetterSubscriptionService.DeleteNewsLetterSubscriptionAsync(subscription);

            eventType.Should().Be(typeof(EmailSubscribedEvent));
        }

        /// <summary>
        /// Verifies the delete triggers unsubscribe event.
        /// </summary>
        [Test]
        public async Task VerifyDeleteTriggersUnsubscribeEvent()
        {
            var subscription = new NewsLetterSubscription { Active = true, Email = "test@test.com" };
            await _newsLetterSubscriptionService.InsertNewsLetterSubscriptionAsync(subscription);
            await _newsLetterSubscriptionService.DeleteNewsLetterSubscriptionAsync(subscription);

            NewsLetterSubscriptionConsumer.LastEventType.Should().Be(typeof(EmailUnsubscribedEvent));
        }
        
        /// <summary>
        /// Verifies the insert event is fired.
        /// </summary>
        [Test]
        public async Task VerifyInsertEventIsFired()
        {
            var subscription = new NewsLetterSubscription { Email = "test@test.com" };

            await _newsLetterSubscriptionService.InsertNewsLetterSubscriptionAsync(subscription);

            var eventType = NewsLetterSubscriptionConsumer.LastEventType;

            await _newsLetterSubscriptionService.DeleteNewsLetterSubscriptionAsync(subscription);

            eventType.Should().Be(typeof(EntityInsertedEvent<NewsLetterSubscription>));
        }

        public class NewsLetterSubscriptionConsumer : IConsumer<EmailSubscribedEvent>, IConsumer<EmailUnsubscribedEvent>, IConsumer<EntityInsertedEvent<NewsLetterSubscription>>
        {
            public static Type LastEventType { get; set; }
            
            public Task HandleEventAsync(EmailSubscribedEvent eventMessage)
            {
                LastEventType = typeof(EmailSubscribedEvent);

                return Task.CompletedTask;
            }

            public Task HandleEventAsync(EmailUnsubscribedEvent eventMessage)
            {
                LastEventType = typeof(EmailUnsubscribedEvent);

                return Task.CompletedTask;
            }

            public Task HandleEventAsync(EntityInsertedEvent<NewsLetterSubscription> eventMessage)
            {
                LastEventType = typeof(EntityInsertedEvent<NewsLetterSubscription>);

                return Task.CompletedTask;
            }
        }
    }
}