﻿using FluentAssertions;
using Nop.Core.Domain.Orders;
﻿using System.Collections.Generic;
using System.Linq;
using Moq;
using Nop.Data;
using Nop.Services.Events;
using Nop.Services.Orders;
using NUnit.Framework;

namespace Nop.Services.Tests.Orders
{
    [TestFixture]
    public class GiftCardServiceTests : ServiceTest
    {
        private Mock<IRepository<GiftCard>> _giftCardRepository;
        private Mock<IRepository<GiftCardUsageHistory>> _giftCardUsageHistoryRepository;
        private Mock<IRepository<OrderItem>> _orderItemRepository;
        private Mock<IEventPublisher> _eventPublisher;
        private IGiftCardService _giftCardService;

        [SetUp]
        public new void SetUp()
        {
            _eventPublisher = new Mock<IEventPublisher>();

            _giftCardRepository = new Mock<IRepository<GiftCard>>();

            var giftCardStore = new List<GiftCard>();

            _giftCardRepository.Setup(r => r.Insert(It.IsAny<GiftCard>())).Callback((GiftCard gc) => giftCardStore.Add(gc));
            _giftCardRepository.Setup(r => r.Table).Returns(giftCardStore.AsQueryable());

            _giftCardRepository.Setup(r => r.GetById(It.IsAny<int>())).Returns((int id) => _giftCardRepository.Object.Table.FirstOrDefault(x => x.Id == id));

            _giftCardRepository.Object.Insert(
                new GiftCard
                {
                    Id = 1,
                    Amount = 100,
                    IsGiftCardActivated = true
                });

            _giftCardRepository.Object.Insert(
                new GiftCard
                {
                    Id = 2,
                    Amount = 100
                });

            _giftCardUsageHistoryRepository = new Mock<IRepository<GiftCardUsageHistory>>();

            var giftCardUsageHistoryStore = new List<GiftCardUsageHistory>();

            _giftCardUsageHistoryRepository.Setup(r => r.Insert(It.IsAny<GiftCardUsageHistory>())).Callback((GiftCardUsageHistory gcuh) => giftCardUsageHistoryStore.Add(gcuh));
            _giftCardUsageHistoryRepository.Setup(r => r.Table).Returns(giftCardUsageHistoryStore.AsQueryable());

            _orderItemRepository = new Mock<IRepository<OrderItem>>();

            _giftCardService = new GiftCardService(null, _eventPublisher.Object, _giftCardRepository.Object, _giftCardUsageHistoryRepository.Object, _orderItemRepository.Object);
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
