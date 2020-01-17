using FluentAssertions;
using Nop.Core.Domain.Orders;
using Nop.Services.Orders;
using NUnit.Framework;

namespace Nop.Services.Tests.Orders
{
    [TestFixture]
    public class GiftCardServiceTests : ServiceTest
    {
        private IGiftCardService _giftCardService;

        [SetUp]
        public new void SetUp()
        {
            _giftCardService = new GiftCardService(null, null, null, null);
        }

        [Test]
        public void Can_validate_giftCard()
        {

            var gc = new GiftCard
            {
                Amount = 100,
                IsGiftCardActivated = true
            };
            gc.GiftCardUsageHistory.Add
                (
                    new GiftCardUsageHistory
                    {
                        UsedValue = 30
                    }

                );
            gc.GiftCardUsageHistory.Add
                (
                    new GiftCardUsageHistory
                    {
                        UsedValue = 20
                    }

                );
            gc.GiftCardUsageHistory.Add
                (
                    new GiftCardUsageHistory
                    {
                        UsedValue = 5
                    }

                );

            //valid
            _giftCardService.IsGiftCardValid(gc).Should().BeTrue();

            //mark as not active
            gc.IsGiftCardActivated = false;
            _giftCardService.IsGiftCardValid(gc).Should().BeFalse();

            //again active
            gc.IsGiftCardActivated = true;
            _giftCardService.IsGiftCardValid(gc).Should().BeTrue();

            //add usage history record
            gc.GiftCardUsageHistory.Add(new GiftCardUsageHistory
            {
                UsedValue = 1000
            });
            _giftCardService.IsGiftCardValid(gc).Should().BeFalse();
        }

        [Test]
        public void Can_calculate_giftCard_remainingAmount()
        {
            var gc = new GiftCard
            {
                Amount = 100
            };
            gc.GiftCardUsageHistory.Add
                (
                    new GiftCardUsageHistory
                    {
                        UsedValue = 30
                    }

                );
            gc.GiftCardUsageHistory.Add
                (
                    new GiftCardUsageHistory
                    {
                        UsedValue = 20
                    }

                );
            gc.GiftCardUsageHistory.Add
                (
                    new GiftCardUsageHistory
                    {
                        UsedValue = 5
                    }

                );

            _giftCardService.GetGiftCardRemainingAmount(gc).Should().Be(45);
        }
    }
}
