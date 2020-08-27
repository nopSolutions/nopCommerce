﻿using FluentAssertions;
using Nop.Core.Domain.Orders;
 using Moq;
 using Nop.Core.Events;
 using Nop.Services.Orders;
 using Nop.Tests;
 using NUnit.Framework;

namespace Nop.Services.Tests.Orders
{
    [TestFixture]
    public class GiftCardServiceTests : ServiceTest
    {
        private FakeRepository<GiftCard> _giftCardRepository;
        private FakeRepository<GiftCardUsageHistory> _giftCardUsageHistoryRepository;
        private FakeRepository<OrderItem> _orderItemRepository;
        private Mock<IEventPublisher> _eventPublisher;
        private IGiftCardService _giftCardService;

        [SetUp]
        public new void SetUp()
        {
            base.SetUp();

            _eventPublisher = new Mock<IEventPublisher>();

            _giftCardRepository = new FakeRepository<GiftCard>();

            _giftCardRepository.Insert(
                new GiftCard
                {
                    Id = 1,
                    Amount = 100,
                    IsGiftCardActivated = true
                });

            _giftCardRepository.Insert(
                new GiftCard
                {
                    Id = 2,
                    Amount = 100
                });

            _giftCardUsageHistoryRepository = new FakeRepository<GiftCardUsageHistory>();

            _orderItemRepository = new FakeRepository<OrderItem>();

            _giftCardService = new GiftCardService(null, _eventPublisher.Object, _giftCardRepository, _giftCardUsageHistoryRepository, _orderItemRepository);
        }

        [Test]
        public void Can_validate_giftCard()
        {
            RunWithTestServiceProvider(()=>{
            var gc = _giftCardService.GetGiftCardById(1);

            _giftCardService.InsertGiftCardUsageHistory(
                new GiftCardUsageHistory
                {
                    GiftCardId = gc.Id,
                    UsedValue = 30
                });

            _giftCardService.InsertGiftCardUsageHistory(
                new GiftCardUsageHistory
                {
                    GiftCardId = gc.Id,
                    UsedValue = 20
                });

            _giftCardService.InsertGiftCardUsageHistory(
                new GiftCardUsageHistory
                {
                    GiftCardId = gc.Id,
                    UsedValue = 5
                });


            //valid
            _giftCardService.IsGiftCardValid(gc).Should().BeTrue();

            //mark as not active
            gc.IsGiftCardActivated = false;
            _giftCardService.IsGiftCardValid(gc).Should().BeFalse();

            //again active
            gc.IsGiftCardActivated = true;
            _giftCardService.IsGiftCardValid(gc).Should().BeTrue();

            //add usage history record
            _giftCardService.InsertGiftCardUsageHistory(
                new GiftCardUsageHistory
                {
                    GiftCardId = gc.Id,
                    UsedValue = 1000
                });

            _giftCardService.IsGiftCardValid(gc).Should().BeFalse();
            });
        }

        [Test]
        public void Can_calculate_giftCard_remainingAmount()
        {
            RunWithTestServiceProvider(()=>{
            var gc = _giftCardService.GetGiftCardById(2);

            _giftCardService.InsertGiftCardUsageHistory(
                new GiftCardUsageHistory
                {
                    GiftCardId = gc.Id,
                    UsedValue = 30
                });

            _giftCardService.InsertGiftCardUsageHistory(
                new GiftCardUsageHistory
                {
                    GiftCardId = gc.Id,
                    UsedValue = 20
                });

            _giftCardService.InsertGiftCardUsageHistory(
                new GiftCardUsageHistory
                {
                    GiftCardId = gc.Id,
                    UsedValue = 5
                });

            _giftCardService.GetGiftCardRemainingAmount(gc).Should().Be(45);
            });
        }
    }
}
