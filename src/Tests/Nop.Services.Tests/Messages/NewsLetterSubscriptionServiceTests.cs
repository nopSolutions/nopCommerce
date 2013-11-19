using Nop.Core.Data;
using Nop.Core.Domain.Messages;
using Nop.Data;
using Nop.Services.Events;
using Nop.Services.Messages;
using NUnit.Framework;
using Rhino.Mocks;

namespace Nop.Services.Tests.Messages {
    [TestFixture]
    public class NewsLetterSubscriptionServiceTests
    {
        /// <summary>
        /// Verifies the active insert triggers subscribe event.
        /// </summary>
        [Test]
        public void VerifyActiveInsertTriggersSubscribeEvent()
        {
            var eventPublisher = MockRepository.GenerateStub<IEventPublisher>();
            var repo = MockRepository.GenerateStub<IRepository<NewsLetterSubscription>>();
            var context = MockRepository.GenerateStub<IDbContext>();

            var subscription = new NewsLetterSubscription { Active = true, Email = "skyler@csharpwebdeveloper.com" };

            var service = new NewsLetterSubscriptionService(context, repo, eventPublisher);
            service.InsertNewsLetterSubscription(subscription, true);

            eventPublisher.AssertWasCalled(x => x.Publish(new EmailSubscribedEvent(subscription.Email)));
        }

        /// <summary>
        /// Verifies the delete triggers unsubscribe event.
        /// </summary>
        [Test]
        public void VerifyDeleteTriggersUnsubscribeEvent()
        {
            var eventPublisher = MockRepository.GenerateStub<IEventPublisher>();
            var repo = MockRepository.GenerateStub<IRepository<NewsLetterSubscription>>();
            var context = MockRepository.GenerateStub<IDbContext>();

            var subscription = new NewsLetterSubscription { Active = true, Email = "skyler@csharpwebdeveloper.com" };

            var service = new NewsLetterSubscriptionService(context, repo, eventPublisher);
            service.DeleteNewsLetterSubscription(subscription, true);

            eventPublisher.AssertWasCalled(x => x.Publish(new EmailUnsubscribedEvent(subscription.Email)));
        }

        /// <summary>
        /// Verifies the email update triggers unsubscribe and subscribe event.
        /// </summary>
        [Test]
        [Ignore("Ignoring until a solution to the IDbContext methods are found. -SRS")]
        public void VerifyEmailUpdateTriggersUnsubscribeAndSubscribeEvent()
        {
            var eventPublisher = MockRepository.GenerateStub<IEventPublisher>();
            var repo = MockRepository.GenerateStub<IRepository<NewsLetterSubscription>>();
            var context = MockRepository.GenerateStub<IDbContext>();

            //Prepare the original result
            var originalSubscription = new NewsLetterSubscription { Active = true, Email = "skyler@csharpwebdeveloper.com" };
            repo.Stub(m => m.GetById(Arg<object>.Is.Anything)).Return(originalSubscription);

            var subscription = new NewsLetterSubscription { Active = true, Email = "skyler@tetragensoftware.com" };

            var service = new NewsLetterSubscriptionService(context, repo, eventPublisher);
            service.UpdateNewsLetterSubscription(subscription, true);

            eventPublisher.AssertWasCalled(x => x.Publish(new EmailUnsubscribedEvent(originalSubscription.Email)));
            eventPublisher.AssertWasCalled(x => x.Publish(new EmailSubscribedEvent(subscription.Email)));
        }

        /// <summary>
        /// Verifies the inactive to active update triggers subscribe event.
        /// </summary>
        [Test]
        [Ignore("Ignoring until a solution to the IDbContext methods are found. -SRS")]
        public void VerifyInactiveToActiveUpdateTriggersSubscribeEvent()
        {
            var eventPublisher = MockRepository.GenerateStub<IEventPublisher>();
            var repo = MockRepository.GenerateStub<IRepository<NewsLetterSubscription>>();
            var context = MockRepository.GenerateStub<IDbContext>();

            //Prepare the original result
            var originalSubscription = new NewsLetterSubscription { Active = false, Email = "skyler@csharpwebdeveloper.com" };
            repo.Stub(m => m.GetById(Arg<object>.Is.Anything)).Return(originalSubscription);

            var subscription = new NewsLetterSubscription { Active = true, Email = "skyler@csharpwebdeveloper.com" };

            var service = new NewsLetterSubscriptionService(context, repo, eventPublisher);
            service.UpdateNewsLetterSubscription(subscription, true);

            eventPublisher.AssertWasCalled(x => x.Publish(new EmailSubscribedEvent(subscription.Email)));
        }

        /// <summary>
        /// Verifies the insert event is fired.
        /// </summary>
        [Test]
        public void VerifyInsertEventIsFired()
        {
            var eventPublisher = MockRepository.GenerateStub<IEventPublisher>();
            var repo = MockRepository.GenerateStub<IRepository<NewsLetterSubscription>>();
            var context = MockRepository.GenerateStub<IDbContext>();

            var service = new NewsLetterSubscriptionService(context, repo, eventPublisher);
            service.InsertNewsLetterSubscription(new NewsLetterSubscription { Email = "skyler@csharpwebdeveloper.com" });

            eventPublisher.AssertWasCalled(x => x.EntityInserted(Arg<NewsLetterSubscription>.Is.Anything));
        }

        /// <summary>
        /// Verifies the update event is fired.
        /// </summary>
        [Test]
        [Ignore("Ignoring until a solution to the IDbContext methods are found. -SRS")]
        public void VerifyUpdateEventIsFired()
        {
            var eventPublisher = MockRepository.GenerateStub<IEventPublisher>();
            var repo = MockRepository.GenerateStub<IRepository<NewsLetterSubscription>>();
            var context = MockRepository.GenerateStub<IDbContext>();

            //Prepare the original result
            var originalSubscription = new NewsLetterSubscription { Active = false, Email = "skyler@csharpwebdeveloper.com" };
            repo.Stub(m => m.GetById(Arg<object>.Is.Anything)).Return(originalSubscription);

            var service = new NewsLetterSubscriptionService(context, repo, eventPublisher);
            service.UpdateNewsLetterSubscription(new NewsLetterSubscription { Email = "skyler@csharpwebdeveloper.com" });

            eventPublisher.AssertWasCalled(x => x.EntityUpdated(Arg<NewsLetterSubscription>.Is.Anything));
        }
    }
}