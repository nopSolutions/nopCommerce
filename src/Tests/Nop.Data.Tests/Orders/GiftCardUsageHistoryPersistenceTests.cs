using System;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Orders;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Orders
{
    [TestFixture]
    public class GiftCardUsageHistoryPersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_giftCardUsageHistory()
        {
            var gcuh = new GiftCardUsageHistory
            {
                UsedValue = 1.1M,
                CreatedOnUtc = new DateTime(2010, 01, 01),
                GiftCard = GetTestGiftCard(),
                UsedWithOrder = GetTestOrder()
            };

            var fromDb = SaveAndLoadEntity(gcuh);
            fromDb.ShouldNotBeNull();
            fromDb.UsedValue.ShouldEqual(1.1M);
            fromDb.CreatedOnUtc.ShouldEqual(new DateTime(2010, 01, 01));

            fromDb.GiftCard.ShouldNotBeNull();
            fromDb.UsedWithOrder.ShouldNotBeNull();
        }


        protected Customer GetTestCustomer()
        {
            return new Customer
            {
                CustomerGuid = Guid.NewGuid(),
                AdminComment = "some comment here",
                Active = true,
                Deleted = false,
                CreatedOnUtc = new DateTime(2010, 01, 01),
                LastActivityDateUtc = new DateTime(2010, 01, 02)
            };
        }

        protected GiftCard GetTestGiftCard()
        {
            return new GiftCard
             {
                 Amount = 1,
                 IsGiftCardActivated = true,
                 GiftCardCouponCode = "Secret",
                 RecipientName = "RecipientName 1",
                 RecipientEmail = "a@b.c",
                 SenderName = "SenderName 1",
                 SenderEmail = "d@e.f",
                 Message = "Message 1",
                 IsRecipientNotified = true,
                 CreatedOnUtc = new DateTime(2010, 01, 01),
             };
        }

        protected Order GetTestOrder()
        {
            return new Order
            {
                OrderGuid = Guid.NewGuid(),
                Customer = GetTestCustomer(),
                BillingAddress = new Address
                {
                    Country = new Country
                    {
                        Name = "United States",
                        TwoLetterIsoCode = "US",
                        ThreeLetterIsoCode = "USA",
                    },
                    CreatedOnUtc = new DateTime(2010, 01, 01),
                },
                Deleted = true,
                CreatedOnUtc = new DateTime(2010, 01, 01)
            };
        }
    }
}