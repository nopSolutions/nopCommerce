using System.Collections.Generic;
using Nop.Core.Domain.Orders;
using Nop.Services.Orders;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Services.Tests.Orders
{
    [TestFixture]
    public class CheckoutAttributeExtensionsTests : ServiceTest
    {
        [SetUp]
        public new void SetUp()
        {
        }

        [Test]
        public void Can_remove_shippable_attributes()
        {
            var attributes = new List<CheckoutAttribute>();
            attributes.Add(new CheckoutAttribute
            {
                Id = 1,
                Name = "Attribute 1",
                ShippableProductRequired = false,
            });
            attributes.Add(new CheckoutAttribute
            {
                Id = 2,
                Name = "Attribute 2",
                ShippableProductRequired = true,
            });
            attributes.Add(new CheckoutAttribute
            {
                Id = 3,
                Name = "Attribute 3",
                ShippableProductRequired = false,
            });
            attributes.Add(new CheckoutAttribute
            {
                Id = 4,
                Name = "Attribute 4",
                ShippableProductRequired = true,
            });
            var filtered = attributes.RemoveShippableAttributes();
            filtered.Count.ShouldEqual(2);
            filtered[0].Id.ShouldEqual(1);
            filtered[1].Id.ShouldEqual(3);
        }
    }
}
