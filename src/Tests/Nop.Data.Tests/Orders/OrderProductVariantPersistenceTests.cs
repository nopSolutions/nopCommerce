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
    public class OrderProductVariantPersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_orderProductVariant()
        {
            var opv = new OrderProductVariant()
            {
                Order = GetTestOrder(),
                ProductVariant = GetTestProductVariant(),
                Quantity = 1, 
                UnitPriceInclTax= 1.1M,
                UnitPriceExclTax = 2.1M,
                PriceInclTax = 3.1M,
                PriceExclTax = 4.1M,
                DiscountAmountInclTax = 5.1M,
                DiscountAmountExclTax = 6.1M,
                AttributeDescription= "AttributeDescription1",
                AttributesXml= "AttributesXml1",
                DownloadCount= 7,
                IsDownloadActivated=true,
                LicenseDownloadId= 8,
                ItemWeight = 9.87M
            };

            var fromDb = SaveAndLoadEntity(opv);
            fromDb.ShouldNotBeNull();
            fromDb.Order.ShouldNotBeNull();
            fromDb.ProductVariant.ShouldNotBeNull();
            fromDb.UnitPriceInclTax.ShouldEqual(1.1M);
            fromDb.UnitPriceExclTax.ShouldEqual(2.1M);
            fromDb.PriceInclTax.ShouldEqual(3.1M);
            fromDb.PriceExclTax.ShouldEqual(4.1M);
            fromDb.DiscountAmountInclTax.ShouldEqual(5.1M);
            fromDb.DiscountAmountExclTax.ShouldEqual(6.1M);
            fromDb.AttributeDescription.ShouldEqual("AttributeDescription1");
            fromDb.AttributesXml.ShouldEqual("AttributesXml1");
            fromDb.DownloadCount.ShouldEqual(7);
            fromDb.IsDownloadActivated.ShouldEqual(true);
            fromDb.LicenseDownloadId.ShouldEqual(8);
            fromDb.ItemWeight.ShouldEqual(9.87M);

            fromDb.Order.ShouldNotBeNull();
        }

        [Test]
        public void Can_save_and_load_orderProductVariant_with_giftCard()
        {
            var opv = new OrderProductVariant()
            {
                Order = GetTestOrder(),
                ProductVariant = GetTestProductVariant(),
            };
            opv.AssociatedGiftCards.Add(GetTestGiftCard());

            var fromDb = SaveAndLoadEntity(opv);
            fromDb.ShouldNotBeNull();

            fromDb.AssociatedGiftCards.ShouldNotBeNull();
            (fromDb.AssociatedGiftCards.Count == 1).ShouldBeTrue();
            fromDb.AssociatedGiftCards.First().Amount.ShouldEqual(10);
        }

        protected GiftCard GetTestGiftCard()
        {
            return new GiftCard
            {
                Amount = 10,
                CreatedOnUtc = DateTime.UtcNow
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