using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Services.Catalog;
using Nop.Services.Events;
using Nop.Services.Messages;
using Nop.Tests;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Linq;
using System.Collections.Generic;
using Nop.Core.Caching;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Infrastructure;
using Nop.Services.Common;

namespace Nop.Services.Tests.Catalog
{
    [TestFixture]
    public class BackInStockSubscriptionServiceTest : ServiceTest
    {
        private IBackInStockSubscriptionService _service;
        private IRepository<BackInStockSubscription> _backInStockSubscriptionRepository;
        private IWorkflowMessageService _workflowMessageService;
        private IEventPublisher _eventPublisher;

        [SetUp]
        public new void SetUp()
        {
            _backInStockSubscriptionRepository = MockRepository.GenerateMock<IRepository<BackInStockSubscription>>();
            _eventPublisher = MockRepository.GenerateMock<IEventPublisher>();
            _workflowMessageService = MockRepository.GenerateMock<IWorkflowMessageService>();

            _eventPublisher.Expect(x => x.Publish(Arg<object>.Is.Anything));
            _service = new BackInStockSubscriptionService(_backInStockSubscriptionRepository, _workflowMessageService, _eventPublisher);
        }

        [Test]
        public void Test_SendNotificationsToSubscribers_WhenProductIsNull_ShouldThrowArgumentNullException()
        {
            // arrange
            Product product = null;

            // assert
            ExceptionAssert.Throws(typeof(ArgumentNullException), () =>
            {
                // act
                _service.SendNotificationsToSubscribers(product);
            });
        }

        [Test]
        public void Test_SendNotificationsToSubscribers_WhenThereAreNoSubscriptions_ShouldReturnZero()
        {
            // arrange
            const int expectedResult = 0;
            Product product = new Product() { Id = 1, Name = "Foo" };
            _backInStockSubscriptionRepository.Expect(x => x.Table).Return(new List<BackInStockSubscription>().AsQueryable());

            // act
            int notificationsSent = _service.SendNotificationsToSubscribers(product);

            // assert
            notificationsSent.ShouldEqual(expectedResult);
        }

        [Test]
        public void Test_SendNotificationsToSubscribers_WhenNotificationSent_ShouldReturnCount()
        {
            // arrange
            Product product = new Product() { Id = 1, Name = "Foo" };
            var testBackInStockSubscriptionList = CreateTestBackInStockSubscriptionList(product);
            int expectedResult = testBackInStockSubscriptionList.Count();
            Singleton<IEngine>.Instance = MockRepository.GenerateMock<IEngine>();

            IRepository<GenericAttribute> genericAttrRepository = MockRepository.GenerateMock<IRepository<GenericAttribute>>();
            genericAttrRepository.Expect(x => x.Table).Return(new List<GenericAttribute>().AsQueryable());
            EngineContext.Current.Expect(x => x.Resolve<IGenericAttributeService>()).Return(new GenericAttributeService(MockRepository.GenerateMock<ICacheManager>(), genericAttrRepository, _eventPublisher));

            _backInStockSubscriptionRepository.Expect(x => x.Table).Return(testBackInStockSubscriptionList);

            // act
            int notificationsSent = _service.SendNotificationsToSubscribers(product);

            // assert
            notificationsSent.ShouldEqual(expectedResult);
        }

        private static IQueryable<BackInStockSubscription> CreateTestBackInStockSubscriptionList(Product product)
        {
            return new List<BackInStockSubscription>()
            {
                new BackInStockSubscription()
                {
                    Product = product,
                    ProductId = product.Id,
                    Customer = new Customer()
                    {
                        Active = true,
                        Deleted = false,
                        Email = "customer1@company1.com"
                    }
                },
                new BackInStockSubscription()
                {
                    Product = product,
                    ProductId = product.Id,
                    Customer = new Customer()
                    {

                        Active = true,
                        Deleted = false,
                        Email = "customer2@company2.com"
                    }
                },
                new BackInStockSubscription()
                {
                    Product = product,
                    ProductId = product.Id,
                    Customer = new Customer()
                    {
                        Active = true,
                        Deleted = false,
                        Email = "customer3@company3.com"
                    }
                }
            }.AsQueryable();

        }
    }
}
