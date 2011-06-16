using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Nop.Tests;
using Nop.Core.Domain;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;

namespace Nop.Data.Tests.Catalog
{
    [TestFixture]
    public class TierPricePersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_tierPrice()
        {
            var tierPrice = new TierPrice
                      {
                          Quantity = 1,
                          Price = 2.1M,
                          ProductVariant = GetTestProductVariant(),
                      };

            var fromDb = SaveAndLoadEntity(tierPrice);
            fromDb.ShouldNotBeNull();
            fromDb.Quantity.ShouldEqual(1);
            fromDb.Price.ShouldEqual(2.1M);

            fromDb.ProductVariant.ShouldNotBeNull();
            fromDb.ProductVariant.Name.ShouldEqual("Product variant name 1");
        }

        [Test]
        public void Can_save_and_load_tierPriceWithCustomerRole()
        {
            var tierPrice = new TierPrice
            {
                Quantity = 1,
                Price = 2,
                ProductVariant = GetTestProductVariant(),
                CustomerRole = new CustomerRole()
                {
                    Name = "Administrators",
                    FreeShipping = true,
                    TaxExempt = true,
                    Active = true,
                    IsSystemRole = true,
                    SystemName = "Administrators"
                }
            };

            var fromDb = SaveAndLoadEntity(tierPrice);
            fromDb.ShouldNotBeNull();

            fromDb.CustomerRole.ShouldNotBeNull();
            fromDb.CustomerRole.Name.ShouldEqual("Administrators");
        }

        protected ProductVariant GetTestProductVariant()
        {
            return new ProductVariant()
            {
                Name = "Product variant name 1",
                CreatedOnUtc = new DateTime(2010, 01, 03),
                UpdatedOnUtc = new DateTime(2010, 01, 04),
                Product = new Product()
                {
                    Name = "Name 1",
                    Published = true,
                    Deleted = false,
                    CreatedOnUtc = new DateTime(2010, 01, 01),
                    UpdatedOnUtc = new DateTime(2010, 01, 02),
                }
            };
        }
    }
}
