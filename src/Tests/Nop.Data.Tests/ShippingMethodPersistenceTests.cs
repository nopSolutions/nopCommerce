using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nop.Core.Domain;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Discounts;
using Nop.Tests;
using NUnit.Framework;
using Nop.Core.Domain.Tax;
using Nop.Core.Domain.Shipping;

namespace Nop.Data.Tests
{
    [TestFixture]
    public class ShippingMethodPersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_shippingMethod()
        {
            var shippingMethod = new ShippingMethod
                               {
                                   Name = "Name 1",
                                   Description = "Description 1",
                                   DisplayOrder = 1
                               };

            var fromDb = SaveAndLoadEntity(shippingMethod);
            fromDb.ShouldNotBeNull();
            fromDb.Name.ShouldEqual("Name 1");
            fromDb.Description.ShouldEqual("Description 1");
            fromDb.DisplayOrder.ShouldEqual(1);
        }
    }
}