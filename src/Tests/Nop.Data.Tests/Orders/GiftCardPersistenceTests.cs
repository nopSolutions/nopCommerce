using System;
using System.Linq;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Orders;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Orders
{
    [TestFixture]
    public class GiftCardPersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_giftCard()
        {
            var giftCard = new GiftCard()
            {
                GiftCardType = GiftCardType.Physical,
                Amount = 1.1M,
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

            var fromDb = SaveAndLoadEntity(giftCard);
            fromDb.ShouldNotBeNull();
            fromDb.GiftCardType.ShouldEqual(GiftCardType.Physical);
            fromDb.Amount.ShouldEqual(1.1M);
            fromDb.IsGiftCardActivated.ShouldEqual(true);
            fromDb.GiftCardCouponCode.ShouldEqual("Secret");
            fromDb.RecipientName.ShouldEqual("RecipientName 1");
            fromDb.RecipientEmail.ShouldEqual("a@b.c");
            fromDb.SenderName.ShouldEqual("SenderName 1");
            fromDb.SenderEmail.ShouldEqual("d@e.f");
            fromDb.Message.ShouldEqual("Message 1");
            fromDb.IsRecipientNotified.ShouldEqual(true);
            fromDb.CreatedOnUtc.ShouldEqual(new DateTime(2010, 01, 01));
        }

        [Test]
        public void Can_save_and_load_giftCard_with_usageHistory()
        {
            var giftCard = new GiftCard()
            {
                GiftCardType = GiftCardType.Physical,
                Amount = 1.1M,
                IsGiftCardActivated = true,
                GiftCardCouponCode = "Secret",
                RecipientName = "RecipientName 1",
                RecipientEmail = "a@b.c",
                SenderName = "SenderName 1",
                SenderEmail = "d@e.f",
                Message = "Message 1",
                IsRecipientNotified = true,
                CreatedOnUtc = new DateTime(2010, 01, 01)
            };
            giftCard.GiftCardUsageHistory.Add
                (
                    new GiftCardUsageHistory()
                    {
                        UsedValue = 1.1M,
                        CreatedOnUtc = new DateTime(2010, 01, 01),
                        UsedWithOrder = GetTestOrder()
                    }
                );
            var fromDb = SaveAndLoadEntity(giftCard);
            fromDb.ShouldNotBeNull();


            fromDb.GiftCardUsageHistory.ShouldNotBeNull();
            (fromDb.GiftCardUsageHistory.Count == 1).ShouldBeTrue();
            fromDb.GiftCardUsageHistory.First().UsedValue.ShouldEqual(1.1M);
        }
        
        [Test]
        public void Can_save_and_load_giftCard_with_associatedOrderProductVariant()
        {
            var giftCard = new GiftCard()
            {
                GiftCardType = GiftCardType.Physical,
                CreatedOnUtc = new DateTime(2010, 01, 01),
                PurchasedWithOrderProductVariant = GetTestOrderProductVariant()
            };

            var fromDb = SaveAndLoadEntity(giftCard);
            fromDb.ShouldNotBeNull();


            fromDb.PurchasedWithOrderProductVariant.ShouldNotBeNull();
            fromDb.PurchasedWithOrderProductVariant.ProductVariant.ShouldNotBeNull();
            fromDb.PurchasedWithOrderProductVariant.ProductVariant.Name.ShouldEqual("Product variant name 1");
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

        protected OrderProductVariant GetTestOrderProductVariant()
        {
            return new OrderProductVariant()
            {
                Order = GetTestOrder(),
                ProductVariant = GetTestProductVariant(),
            };
        }

        protected ProductVariant GetTestProductVariant()
        {
            return new ProductVariant
            {
                Name = "Product variant name 1",
                Sku = "sku 1",
                Description = "description",
                CreatedOnUtc = new DateTime(2010, 01, 03),
                UpdatedOnUtc = new DateTime(2010, 01, 04),
                Product = new Product()
                {
                    Name = "Name 1",
                    CreatedOnUtc = new DateTime(2010, 01, 01),
                    UpdatedOnUtc = new DateTime(2010, 01, 02)
                }
            };
        }

        protected Order GetTestOrder()
        {
            return new Order()
            {
                OrderGuid = Guid.NewGuid(),
                Customer = GetTestCustomer(),
                BillingAddress = new Address()
                {
                    Country = new Country()
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