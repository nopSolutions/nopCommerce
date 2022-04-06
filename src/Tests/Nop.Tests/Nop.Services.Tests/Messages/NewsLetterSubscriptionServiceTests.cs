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

        [Test]
        public async Task CanCRUD()
        {
            var guid = Guid.NewGuid();

            var subscription = new NewsLetterSubscription
            {
                Active = true,
                Email = NopTestsDefaults.AdminEmail,
                NewsLetterSubscriptionGuid = guid
            };

            await _newsLetterSubscriptionService.InsertNewsLetterSubscriptionAsync(subscription);
            subscription.Id.Should().BeGreaterThan(0);
            subscription = await _newsLetterSubscriptionService.GetNewsLetterSubscriptionByIdAsync(subscription.Id);
            subscription.Active.Should().BeTrue();
            subscription.Active = false;
            await _newsLetterSubscriptionService.UpdateNewsLetterSubscriptionAsync(subscription);
            subscription = await _newsLetterSubscriptionService.GetNewsLetterSubscriptionByGuidAsync(guid);
            subscription.Active.Should().BeFalse();

            subscription =
                await _newsLetterSubscriptionService.GetNewsLetterSubscriptionByEmailAndStoreIdAsync(
                    NopTestsDefaults.AdminEmail, 0);
            subscription.Should().NotBeNull();

            await _newsLetterSubscriptionService.DeleteNewsLetterSubscriptionAsync(subscription);
            subscription = await _newsLetterSubscriptionService.GetNewsLetterSubscriptionByGuidAsync(guid);
            subscription.Should().BeNull();
            subscription =
                await _newsLetterSubscriptionService.GetNewsLetterSubscriptionByEmailAndStoreIdAsync(
                    NopTestsDefaults.AdminEmail, 1);
            subscription.Should().BeNull();
        }

        [Test]
        public async Task CanGetAllNewsLetterSubscriptions()
        {
            var guid = Guid.NewGuid();

            var subscription = new NewsLetterSubscription
            {
                Active = true,
                Email = NopTestsDefaults.AdminEmail,
                NewsLetterSubscriptionGuid = guid
            };
            await _newsLetterSubscriptionService.InsertNewsLetterSubscriptionAsync(subscription);

            var subscriptions = await _newsLetterSubscriptionService.GetAllNewsLetterSubscriptionsAsync();
            subscriptions.Count.Should().Be(1);
            subscriptions = await _newsLetterSubscriptionService.GetAllNewsLetterSubscriptionsAsync(NopTestsDefaults.AdminEmail);
            subscriptions.Count.Should().Be(1);
            subscriptions = await _newsLetterSubscriptionService.GetAllNewsLetterSubscriptionsAsync(isActive: true);
            subscriptions.Count.Should().Be(1);
            subscriptions = await _newsLetterSubscriptionService.GetAllNewsLetterSubscriptionsAsync(isActive: false);
            subscriptions.Count.Should().Be(0);

            subscriptions = await _newsLetterSubscriptionService.GetAllNewsLetterSubscriptionsAsync(customerRoleId: 1);
            subscriptions.Count.Should().Be(1);
            subscriptions = await _newsLetterSubscriptionService.GetAllNewsLetterSubscriptionsAsync(customerRoleId: 1, storeId: 1);
            subscriptions.Count.Should().Be(0);
            subscriptions = await _newsLetterSubscriptionService.GetAllNewsLetterSubscriptionsAsync(NopTestsDefaults.AdminEmail, customerRoleId: 4);
            subscriptions.Count.Should().Be(0);


            await _newsLetterSubscriptionService.DeleteNewsLetterSubscriptionAsync(subscription);
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