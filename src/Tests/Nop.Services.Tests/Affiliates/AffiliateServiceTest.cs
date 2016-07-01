using System;
using Nop.Core.Data;
using Nop.Core.Domain.Affiliates;
using Nop.Core.Domain.Orders;
using Nop.Services.Affiliates;
using Nop.Services.Events;
using Nop.Tests;
using NUnit.Framework;
using Rhino.Mocks;

namespace Nop.Services.Tests.Affiliates
{
    public class AffiliateServiceTest : ServiceTest
    {
        private IAffiliateService _affiliateService;
        private IRepository<Affiliate> _affiliateRepository;
        private IRepository<Order> _orderRepository;
        private IEventPublisher _eventPublisher;

        [SetUp]
        public new void SetUp()
        {
            _affiliateRepository = MockRepository.GenerateMock<IRepository<Affiliate>>();
            _orderRepository = MockRepository.GenerateMock<IRepository<Order>>();
            _eventPublisher = MockRepository.GenerateMock<IEventPublisher>();

            _eventPublisher.Expect(x => x.Publish(Arg<object>.Is.Anything));

            _affiliateService = new AffiliateService(_affiliateRepository, _orderRepository, _eventPublisher);
        }

        [Test]
        public void Test_InsertAffiliate_WhenAffiliateInserted_ShouldCallEntityInserted()
        {
            // arrange
            Affiliate affiliate = new Affiliate();

            // act
            _affiliateService.InsertAffiliate(affiliate);

            // assert
            _eventPublisher.AssertWasCalled(x => x.EntityInserted(Arg<Affiliate>.Is.Anything));
        }

        [Test]
        public void Test_InsertAffiliate_WhenAffiliateArgumentIsNull_ShouldThrowArgumentNullException()
        {
            // arrange
            Affiliate affiliate = null;

            // assert
            ExceptionAssert.Throws(typeof(ArgumentNullException), () =>
            {
                // act
                _affiliateService.InsertAffiliate(affiliate);
            });
        }

        [Test]
        public void Test_GetAffiliateById_WhenIdIsZero_ShouldReturnNullAffiliate()
        {
            // arrange
            int affiliateId = 0;

            // act
            Affiliate affiliate = _affiliateService.GetAffiliateById(affiliateId);

            // assert
            affiliate.ShouldEqual(null);
        }

        [Test]
        public void Test_DeleteAffiliate_ShouldSetAffiliateAsDeleted()
        {
            // arrange
            Affiliate affiliate = new Affiliate();
            affiliate.Deleted = false;

            // act
            _affiliateService.DeleteAffiliate(affiliate);

            // assert
            affiliate.Deleted.ShouldBeTrue();
        }

        [Test]
        public void Test_UpdateAffiliate_WhenAffiliateUpdated_ShouldCallEntityUpdated()
        {
            // arrange
            Affiliate affiliate = new Affiliate();

            // act
            _affiliateService.UpdateAffiliate(affiliate);

            // assert
            _eventPublisher.AssertWasCalled(x => x.EntityUpdated(Arg<Affiliate>.Is.Anything));
        }
    }
}
