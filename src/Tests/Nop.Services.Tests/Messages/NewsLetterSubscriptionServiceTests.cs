using Moq;
using Nop.Core.Data;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Messages;
using Nop.Data;
using Nop.Services.Customers;
using Nop.Services.Events;
using Nop.Services.Messages;
using NUnit.Framework;

namespace Nop.Services.Tests.Messages 
{
    [TestFixture]
    public class NewsLetterSubscriptionServiceTests : ServiceTest
    {
        private Mock<IEventPublisher> _eventPublisher;
        private Mock<IRepository<NewsLetterSubscription>> _newsLetterSubscriptionRepository;
        private Mock<IRepository<Customer>> _customerRepository;
        private Mock<ICustomerService> _customerService;
        private Mock<IDbContext> _dbContext;

        [SetUp]
        public new void SetUp()
        {
            _eventPublisher = new Mock<IEventPublisher>();
            _newsLetterSubscriptionRepository = new Mock<IRepository<NewsLetterSubscription>>();
            _customerRepository = new Mock<IRepository<Customer>>();
            _customerService = new Mock<ICustomerService>();
            _dbContext = new Mock<IDbContext>();
        }

        /// <summary>
        /// Verifies the active insert triggers subscribe event.
        /// </summary>
        [Test]
        public void VerifyActiveInsertTriggersSubscribeEvent()
        {
            var service = new NewsLetterSubscriptionService(_customerService.Object, _dbContext.Object, _eventPublisher.Object,
                _customerRepository.Object, _newsLetterSubscriptionRepository.Object);

            var subscription = new NewsLetterSubscription { Active = true, Email = "test@test.com" };
            service.InsertNewsLetterSubscription(subscription);

            _eventPublisher.Verify(x => x.Publish(new EmailSubscribedEvent(subscription)));
        }

        /// <summary>
        /// Verifies the delete triggers unsubscribe event.
        /// </summary>
        [Test]
        public void VerifyDeleteTriggersUnsubscribeEvent()
        {
            var service = new NewsLetterSubscriptionService(_customerService.Object, _dbContext.Object, _eventPublisher.Object,
                _customerRepository.Object, _newsLetterSubscriptionRepository.Object);

            var subscription = new NewsLetterSubscription { Active = true, Email = "test@test.com" };
            service.DeleteNewsLetterSubscription(subscription);

            _eventPublisher.Verify(x => x.Publish(new EmailUnsubscribedEvent(subscription)));
        }
        
        /// <summary>
        /// Verifies the insert event is fired.
        /// </summary>
        [Test]
        [Ignore("Moq can't verify an extension method")]
        public void VerifyInsertEventIsFired()
        {
            var service = new NewsLetterSubscriptionService(_customerService.Object, _dbContext.Object, _eventPublisher.Object,
                _customerRepository.Object, _newsLetterSubscriptionRepository.Object);

            var subscription = new NewsLetterSubscription {Email = "test@test.com"};

            service.InsertNewsLetterSubscription(subscription);

            _eventPublisher.Verify(x => x.EntityInserted(It.IsAny<NewsLetterSubscription>()));
        }
    }
}