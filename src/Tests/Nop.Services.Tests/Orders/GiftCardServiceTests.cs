﻿using FluentAssertions;
using Nop.Core.Domain.Orders;
 using Nop.Services.Orders;
using NUnit.Framework;

namespace Nop.Services.Tests.Orders
{
    [TestFixture]
    public class GiftCardServiceTests : ServiceTest
    {
        private IGiftCardService _giftCardService;
        private GiftCard _giftCard1;
        private GiftCard _giftCard2;

        [SetUp]
        public void SetUp()
        {
            _giftCardService = GetService<IGiftCardService>();

            _giftCard1 = new GiftCard {Amount = 100, IsGiftCardActivated = true};
            _giftCard2 = new GiftCard { Amount = 100 };

            _giftCardService.InsertGiftCard(_giftCard1);
            _giftCardService.InsertGiftCard(_giftCard2);
        }

        [TearDown]
        public void TearDown()
        {
            _giftCardService.DeleteGiftCard(_giftCard1);
            _giftCardService.DeleteGiftCard(_giftCard2);
        }

        [Test]
        public void CanValidateGiftCard()
        {
            _giftCardService.InsertGiftCardUsageHistory(
                new GiftCardUsageHistory {GiftCardId = _giftCard1.Id, UsedWithOrderId = 1, UsedValue = 30});

            _giftCardService.InsertGiftCardUsageHistory(
                new GiftCardUsageHistory {GiftCardId = _giftCard1.Id, UsedWithOrderId = 1, UsedValue = 20});

            _giftCardService.InsertGiftCardUsageHistory(
                new GiftCardUsageHistory {GiftCardId = _giftCard1.Id, UsedWithOrderId = 1, UsedValue = 5});

            //valid
            _giftCardService.IsGiftCardValid(_giftCard1).Should().BeTrue();

            //mark as not active
            _giftCard1.IsGiftCardActivated = false;
            _giftCardService.IsGiftCardValid(_giftCard1).Should().BeFalse();

            //again active
            _giftCard1.IsGiftCardActivated = true;
            _giftCardService.IsGiftCardValid(_giftCard1).Should().BeTrue();

            //add usage history record
            _giftCardService.InsertGiftCardUsageHistory(
                new GiftCardUsageHistory {GiftCardId = _giftCard1.Id, UsedWithOrderId = 1, UsedValue = 1000});

            _giftCardService.IsGiftCardValid(_giftCard1).Should().BeFalse();
        }

        [Test]
        public void CanCalculateGiftCardRemainingAmount()
        {
            _giftCardService.InsertGiftCardUsageHistory(
                new GiftCardUsageHistory {GiftCardId = _giftCard2.Id, UsedWithOrderId = 1, UsedValue = 30});

            _giftCardService.InsertGiftCardUsageHistory(
                new GiftCardUsageHistory {GiftCardId = _giftCard2.Id, UsedWithOrderId = 1, UsedValue = 20});

            _giftCardService.InsertGiftCardUsageHistory(
                new GiftCardUsageHistory {GiftCardId = _giftCard2.Id, UsedWithOrderId = 1, UsedValue = 5});

            _giftCardService.GetGiftCardRemainingAmount(_giftCard2).Should().Be(45);
        }
    }
}
