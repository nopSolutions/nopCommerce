using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Nop.Tests;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Orders;

namespace Nop.Core.Tests.Entities.Orders
{
    [TestFixture]
    public class GiftCardTests
    {
        [Test]
        public void Can_validate_giftCard()
        {
            var gc = new GiftCard()
            {
                Amount = 100,
                IsGiftCardActivated = true,
                GiftCardUsageHistory = new List<GiftCardUsageHistory>()
                {
                    new GiftCardUsageHistory()
                    {
                         UsedValue = 30
                    },
                    new GiftCardUsageHistory()
                    {
                         UsedValue = 20
                    },
                    new GiftCardUsageHistory()
                    {
                         UsedValue = 5
                    }
                }
            };

            //valid
            gc.IsGiftCardValid().ShouldEqual(true);

            //mark as not active
            gc.IsGiftCardActivated = false;
            gc.IsGiftCardValid().ShouldEqual(false);

            //again active
            gc.IsGiftCardActivated = true;
            gc.IsGiftCardValid().ShouldEqual(true);

            //add usage history record
            gc.GiftCardUsageHistory.Add(new GiftCardUsageHistory()
            {
                UsedValue = 1000
            });
            gc.IsGiftCardValid().ShouldEqual(false);
        }

        [Test]
        public void Can_calculate_giftCard_remainingAmount()
        {
            var gc = new GiftCard()
            {
                Amount = 100,
                GiftCardUsageHistory = new List<GiftCardUsageHistory>()
                {
                    new GiftCardUsageHistory()
                    {
                         UsedValue = 30
                    },
                    new GiftCardUsageHistory()
                    {
                         UsedValue = 20
                    },
                    new GiftCardUsageHistory()
                    {
                         UsedValue = 5
                    }
                }
            };

            gc.GetGiftCardRemainingAmount().ShouldEqual(45);
        }
    }
}
