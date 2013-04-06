using System;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Stores;
using Nop.Core.Domain.Tax;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Orders
{
    [TestFixture]
    public class ShoppingCartItemPeristenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_shoppingCartItem()
        {
            var sci = new ShoppingCartItem
            {
                StoreId = 1,
                ShoppingCartType = ShoppingCartType.ShoppingCart,
                AttributesXml = "AttributesXml 1",
                CustomerEnteredPrice = 1.1M,
                Quantity= 2,
                CreatedOnUtc = new DateTime(2010, 01, 01),
                UpdatedOnUtc = new DateTime(2010, 01, 02),
                Customer = GetTestCustomer(),
                ProductVariant = GetTestProductVariant(),
            };

            var fromDb = SaveAndLoadEntity(sci);
            fromDb.ShouldNotBeNull();
            
            fromDb.StoreId.ShouldEqual(1);
            fromDb.ShoppingCartType.ShouldEqual(ShoppingCartType.ShoppingCart);
            fromDb.AttributesXml.ShouldEqual("AttributesXml 1");
            fromDb.CustomerEnteredPrice.ShouldEqual(1.1M);
            fromDb.Quantity.ShouldEqual(2);
            fromDb.CreatedOnUtc.ShouldEqual(new DateTime(2010, 01, 01));
            fromDb.UpdatedOnUtc.ShouldEqual(new DateTime(2010, 01, 02));

            fromDb.Customer.ShouldNotBeNull();

            fromDb.ProductVariant.ShouldNotBeNull();

        }

        protected Customer GetTestCustomer()
        {
            return new Customer
            {
                CustomerGuid = Guid.NewGuid(),
                AdminComment = "some comment here",
                IsTaxExempt = true,
                Active = true,
                Deleted = false,
                CreatedOnUtc = new DateTime(2010, 01, 01),
                LastActivityDateUtc = new DateTime(2010, 01, 02)
            };
        }

        protected ProductVariant GetTestProductVariant()
        {
            return new ProductVariant
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
            };
        }
    }
}
