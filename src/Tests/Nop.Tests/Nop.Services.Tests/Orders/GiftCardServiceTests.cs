using System.Threading.Tasks;
using FluentAssertions;
using Nop.Core.Domain.Orders;
using Nop.Services.Orders;
using NUnit.Framework;

namespace Nop.Tests.Nop.Services.Tests.Orders
{
    [TestFixture]
    public class GiftCardServiceTests : ServiceTest
    {
        private IGiftCardService _giftCardService;
        private GiftCard _giftCard1;
        private GiftCard _giftCard2;

        [OneTimeSetUp]
        public async Task SetUp()
        {
            _giftCardService = GetService<IGiftCardService>();

            _giftCard1 = new GiftCard {Amount = 100, IsGiftCardActivated = true};
            _giftCard2 = new GiftCard { Amount = 100 };

            await _giftCardService.InsertGiftCardAsync(_giftCard1);
            await _giftCardService.InsertGiftCardAsync(_giftCard2);
        }

        [OneTimeTearDown]
        public async Task TearDown()
        {
            await _giftCardService.DeleteGiftCardAsync(_giftCard1);
            await _giftCardService.DeleteGiftCardAsync(_giftCard2);
        }

        [Test]
        public async Task CanValidateGiftCard()
        {
            await _giftCardService.InsertGiftCardUsageHistoryAsync(
                new GiftCardUsageHistory {GiftCardId = _giftCard1.Id, UsedWithOrderId = 1, UsedValue = 30});

            await _giftCardService.InsertGiftCardUsageHistoryAsync(
                new GiftCardUsageHistory {GiftCardId = _giftCard1.Id, UsedWithOrderId = 1, UsedValue = 20});

            await _giftCardService.InsertGiftCardUsageHistoryAsync(
                new GiftCardUsageHistory {GiftCardId = _giftCard1.Id, UsedWithOrderId = 1, UsedValue = 5});

            //valid
            var isValid = await _giftCardService.IsGiftCardValidAsync(_giftCard1);
            isValid.Should().BeTrue();

            //mark as not active
            _giftCard1.IsGiftCardActivated = false;
            isValid = await _giftCardService.IsGiftCardValidAsync(_giftCard1);
            isValid.Should().BeFalse();

            //again active
            _giftCard1.IsGiftCardActivated = true;
            isValid = await _giftCardService.IsGiftCardValidAsync(_giftCard1);
            isValid.Should().BeTrue();

            //add usage history record
            await _giftCardService.InsertGiftCardUsageHistoryAsync(
                new GiftCardUsageHistory {GiftCardId = _giftCard1.Id, UsedWithOrderId = 1, UsedValue = 1000});

            isValid = await _giftCardService.IsGiftCardValidAsync(_giftCard1);
            isValid.Should().BeFalse();
        }

        [Test]
        public async Task CanCalculateGiftCardRemainingAmount()
        {
            await _giftCardService.InsertGiftCardUsageHistoryAsync(
                new GiftCardUsageHistory {GiftCardId = _giftCard2.Id, UsedWithOrderId = 1, UsedValue = 30});

            await _giftCardService.InsertGiftCardUsageHistoryAsync(
                new GiftCardUsageHistory {GiftCardId = _giftCard2.Id, UsedWithOrderId = 1, UsedValue = 20});

            await _giftCardService.InsertGiftCardUsageHistoryAsync(
                new GiftCardUsageHistory {GiftCardId = _giftCard2.Id, UsedWithOrderId = 1, UsedValue = 5});

            var remainingAmount = await _giftCardService.GetGiftCardRemainingAmountAsync(_giftCard2);
            remainingAmount.Should().Be(45);
        }
    }
}
