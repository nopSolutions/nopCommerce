using System;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Stores;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Catalog
{
    [TestFixture]
    public class BackInStockSubscriptionPersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_backInStockSubscription()
        {
            var backInStockSubscription = new BackInStockSubscription()
                                     {
                                         ProductVariant = new ProductVariant
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
                                                 UpdatedOnUtc = new DateTime(2010, 01, 02)
                                             }
                                         },
                                         Customer = new Customer
                                         {
                                             CustomerGuid = Guid.NewGuid(),
                                             AdminComment = "some comment here",
                                             Active = true,
                                             Deleted = false,
                                             CreatedOnUtc = new DateTime(2010, 01, 01),
                                             LastActivityDateUtc = new DateTime(2010, 01, 02)
                                         },
                                         CreatedOnUtc = new DateTime(2010, 01, 02)
                                     };

            var fromDb = SaveAndLoadEntity(backInStockSubscription);
            fromDb.ShouldNotBeNull();

            fromDb.ProductVariant.ShouldNotBeNull();
            fromDb.ProductVariant.Name.ShouldEqual("Product variant name 1");

            fromDb.Customer.ShouldNotBeNull();

            fromDb.CreatedOnUtc.ShouldEqual(new DateTime(2010, 01, 02));
        }
    }
}
