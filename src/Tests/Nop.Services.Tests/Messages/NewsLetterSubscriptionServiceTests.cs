using Moq;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Messages;
using Nop.Core.Events;
using Nop.Services.Customers;
using Nop.Services.Messages;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Services.Tests.Messages 
{
    [TestFixture]
    public class NewsLetterSubscriptionServiceTests : ServiceTest
    {
        private Mock<IEventPublisher> _eventPublisher;
        private FakeRepository<NewsLetterSubscription> _newsLetterSubscriptionRepository;
        private FakeRepository<Customer> _customerRepository;
        private FakeRepository<CustomerCustomerRoleMapping> _customerCustomerRoleMappingRepository;
        private Mock<ICustomerService> _customerService;

        [SetUp]
        public new void SetUp()
        {
            _eventPublisher = new Mock<IEventPublisher>();
            _newsLetterSubscriptionRepository = new FakeRepository<NewsLetterSubscription>();
            _customerRepository = new FakeRepository<Customer>();
            _customerCustomerRoleMappingRepository = new FakeRepository<CustomerCustomerRoleMapping>();
            _customerService = new Mock<ICustomerService>();
        }

        /// <summary>
        /// Verifies the active insert triggers subscribe event.
        /// </summary>
        [Test]
        public void VerifyActiveInsertTriggersSubscribeEvent()
        {
            var service = new NewsLetterSubscriptionService(_customerService.Object, _eventPublisher.Object,
                _customerRepository, _customerCustomerRoleMappingRepository, _newsLetterSubscriptionRepository);

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
            var service = new NewsLetterSubscriptionService(_customerService.Object, _eventPublisher.Object,
                _customerRepository, _customerCustomerRoleMappingRepository, _newsLetterSubscriptionRepository);

            var subscription = new NewsLetterSubscription { Active = true, Email = "test@test.com" };
            service.DeleteNewsLetterSubscription(subscription);

            _eventPublisher.Verify(x => x.Publish(new EmailUnsubscribedEvent(subscription)));
        }
        
        /// <summary>
        /// Verifies the insert event is fired.
        /// </summary>
        [Test]
        public void VerifyInsertEventIsFired()
        {
            var service = new NewsLetterSubscriptionService(_customerService.Object, _eventPublisher.Object,
                _customerRepository, _customerCustomerRoleMappingRepository, _newsLetterSubscriptionRepository);

            var subscription = new NewsLetterSubscription { Active = true, Email = "test@test.com"};

            service.InsertNewsLetterSubscription(subscription);

            _eventPublisher.Verify(x => x.Publish(new EmailSubscribedEvent(subscription)));
        }
    }
}