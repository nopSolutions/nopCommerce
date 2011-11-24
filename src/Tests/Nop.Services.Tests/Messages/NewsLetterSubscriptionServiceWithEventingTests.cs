using NUnit.Framework;
using Nop.Core.Data;
using Nop.Core.Domain.Messages;
using Nop.Core.Events;
using Nop.Services.Messages;
using Rhino.Mocks;

namespace Nop.Services.Tests.Messages {
    [TestFixture]
    public class NewsLetterSubscriptionServiceWithEventingTests {
        /// <summary>
        /// Verifies the insert event is fired.
        /// </summary>
        [Test]
        public void VerifyInsertEventIsFired() {
            var eventPublisher = MockRepository.GenerateStub<IEventPublisher>();
            var repo = MockRepository.GenerateStub<IRepository<NewsLetterSubscription>>();

            var service = new NewsLetterSubscriptionServiceWithEventing(repo, eventPublisher);
            service.InsertNewsLetterSubscription(new NewsLetterSubscription {Email = "skyler@csharpwebdeveloper.com"});

            eventPublisher.AssertWasCalled(x => x.EntityInserted(Arg<NewsLetterSubscription>.Is.Anything));
        }

        /// <summary>
        /// Verifies the update event is fired.
        /// </summary>
        [Test]
        public void VerifyUpdateEventIsFired() {
            var eventPublisher = MockRepository.GenerateStub<IEventPublisher>();
            var repo = MockRepository.GenerateStub<IRepository<NewsLetterSubscription>>();

            var service = new NewsLetterSubscriptionServiceWithEventing(repo, eventPublisher);
            service.UpdateNewsLetterSubscription(new NewsLetterSubscription {Email = "skyler@csharpwebdeveloper.com"});

            eventPublisher.AssertWasCalled(x => x.EntityUpdated(Arg<NewsLetterSubscription>.Is.Anything));
        }
    }
}