using System.Linq;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Customers
{
    [TestFixture]
    public class CustomerAttributePersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_customerAttribute()
        {
            var ca = new CustomerAttribute()
            {
                Name = "Name 1",
                IsEnabled = true,
                IsRequired = true,
                IsSystem = true,
                SystemName = "SystemName 1",
                AttributeControlType = AttributeControlType.Datepicker,
                DisplayOrder = 2
            };

            var fromDb = SaveAndLoadEntity(ca);
            fromDb.ShouldNotBeNull();
            fromDb.Name.ShouldEqual("Name 1");
            fromDb.IsEnabled.ShouldEqual(true);
            fromDb.IsRequired.ShouldEqual(true);
            fromDb.IsSystem.ShouldEqual(true);
            fromDb.SystemName.ShouldEqual("SystemName 1");
            fromDb.AttributeControlType.ShouldEqual(AttributeControlType.Datepicker);
            fromDb.DisplayOrder.ShouldEqual(2);
        }

        [Test]
        public void Can_save_and_load_customerAttribute_with_values()
        {
            var ca = new CustomerAttribute()
            {
                Name = "Name 1",
                IsEnabled = true,
                IsRequired = true,
                IsSystem = true,
                SystemName = "SystemName 1",
                AttributeControlType = AttributeControlType.Datepicker,
                DisplayOrder = 2
            };
            ca.CustomerAttributeValues.Add
                (
                    new CustomerAttributeValue()
                    {
                        Name = "Name 2",
                        IsPreSelected = true,
                        DisplayOrder = 1,
                    }
                );
            var fromDb = SaveAndLoadEntity(ca);
            fromDb.ShouldNotBeNull();
            fromDb.Name.ShouldEqual("Name 1");

            fromDb.CustomerAttributeValues.ShouldNotBeNull();
            (fromDb.CustomerAttributeValues.Count == 1).ShouldBeTrue();
            fromDb.CustomerAttributeValues.First().Name.ShouldEqual("Name 2");
        }
    }
}