using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Nop.Tests;
using Nop.Core.Domain;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Discounts;

namespace Nop.Data.Tests
{
    [TestFixture]
    public class OrderPersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_order()
        {
            var order = new Order
            {
                OrderGuid = Guid.NewGuid(),
                Customer = GetTestCustomer(),
                Deleted = false,
                CreatedOnUtc = new DateTime(2010, 01, 01)
            };

            var fromDb = SaveAndLoadEntity(order);
            fromDb.ShouldNotBeNull();
            fromDb.Deleted.ShouldEqual(false);
            fromDb.CreatedOnUtc.ShouldEqual(new DateTime(2010, 01, 01));

            fromDb.Customer.ShouldNotBeNull();
        }

        [Test]
        public void Can_save_and_load_order_with_usedRewardPoints()
        {
            var order = new Order
            {
                OrderGuid = Guid.NewGuid(),
                Customer = GetTestCustomer(),
                RedeemedRewardPointsEntry = new RewardPointsHistory()
                {
                    Customer = GetTestCustomer(),
                    Points = -1,
                    Message = "Used with order",
                    PointsBalance = 2,
                    UsedAmount = 3,
                    UsedAmountInCustomerCurrency = 4,
                    CustomerCurrencyCode = "USD",
                    CreatedOnUtc = new DateTime(2010, 01, 01)
                },
                Deleted = false,
                CreatedOnUtc = new DateTime(2010, 01, 01)
            };

            var fromDb = SaveAndLoadEntity(order);
            fromDb.ShouldNotBeNull();
            fromDb.Deleted.ShouldEqual(false);
            fromDb.CreatedOnUtc.ShouldEqual(new DateTime(2010, 01, 01));

            fromDb.RedeemedRewardPointsEntry.ShouldNotBeNull();
            fromDb.RedeemedRewardPointsEntry.Points.ShouldEqual(-1);
        }

        [Test]
        public void Can_save_and_load_order_with_discountUsageHistory()
        {
            var testCustomer = GetTestCustomer();
            var order = new Order
            {
                OrderGuid = Guid.NewGuid(),
                Customer = testCustomer,
                DiscountUsageHistory = new List<DiscountUsageHistory>()
                {
                    new DiscountUsageHistory()
                    {
                        Discount = GetTestDiscount(),
                        CreatedOnUtc = new DateTime(2010, 01, 01)
                    }
                },
                Deleted = false,
                CreatedOnUtc = new DateTime(2010, 01, 01)
            };

            var fromDb = SaveAndLoadEntity(order);
            fromDb.ShouldNotBeNull();

            fromDb.DiscountUsageHistory.ShouldNotBeNull();
            fromDb.DiscountUsageHistory.ShouldNotBeNull();
            fromDb.DiscountUsageHistory.Count.ShouldEqual(1);
            fromDb.DiscountUsageHistory.First().CreatedOnUtc.ShouldEqual(new DateTime(2010, 01, 01));
        }

        [Test]
        public void Can_save_and_load_order_with_giftCardUsageHistory()
        {
            var testCustomer = GetTestCustomer();
            var order = new Order
            {
                OrderGuid = Guid.NewGuid(),
                Customer = testCustomer,
                GiftCardUsageHistory = new List<GiftCardUsageHistory>()
                {
                    new GiftCardUsageHistory()
                    {
                        UsedValue = 1.1M,
                        UsedValueInCustomerCurrency = 2.2M,
                        CreatedOnUtc = new DateTime(2010, 01, 01),
                        GiftCard = GetTestGiftCard()
                    }
                },
                Deleted = false,
                CreatedOnUtc = new DateTime(2010, 01, 01)
            };

            var fromDb = SaveAndLoadEntity(order);
            fromDb.ShouldNotBeNull();

            fromDb.GiftCardUsageHistory.ShouldNotBeNull();
            fromDb.GiftCardUsageHistory.Count.ShouldEqual(1);
            fromDb.GiftCardUsageHistory.First().CreatedOnUtc.ShouldEqual(new DateTime(2010, 01, 01));
        }

        protected GiftCard GetTestGiftCard()
        {
            return new GiftCard()
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

        protected Customer GetTestCustomer()
        {
            return new Customer
            {
                CustomerGuid = Guid.NewGuid(),
                AdminComment = "some comment here",
                Active = true,
                Deleted = false,
                CreatedOnUtc = new DateTime(2010, 01, 01)
            };
        }

        protected Discount GetTestDiscount()
        {
            return new Discount
            {
                Name = "Discount 1",
                DiscountType = DiscountType.AssignedToCategories,
                UsePercentage = true,
                DiscountPercentage = 1,
                DiscountAmount = 2,
                StartDateUtc = new DateTime(2010, 01, 01),
                EndDateUtc = new DateTime(2010, 01, 02),
                RequiresCouponCode = true,
                CouponCode = "SecretCode",
                DiscountLimitation = DiscountLimitationType.Unlimited,
                LimitationTimes = 3,
            };
        }
    }
}
