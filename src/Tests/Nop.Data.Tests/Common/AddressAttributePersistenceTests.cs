using System.Linq;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Common
{
    [TestFixture]
    public class AddressAttributePersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_addressAttribute()
        {
            var ca = new AddressAttribute
            {
                Name = "Name 1",
                IsRequired = true,
                AttributeControlType = AttributeControlType.Datepicker,
                DisplayOrder = 2
            };

            var fromDb = SaveAndLoadEntity(ca);
            fromDb.ShouldNotBeNull();
            fromDb.Name.ShouldEqual("Name 1");
            fromDb.IsRequired.ShouldEqual(true);
            fromDb.AttributeControlType.ShouldEqual(AttributeControlType.Datepicker);
            fromDb.DisplayOrder.ShouldEqual(2);
        }

        [Test]
        public void Can_save_and_load_addressAttribute_with_values()
        {
            var ca = new AddressAttribute
            {
                Name = "Name 1",
                IsRequired = true,
                AttributeControlType = AttributeControlType.Datepicker,
                DisplayOrder = 2
            };
            ca.AddressAttributeValues.Add
                (
                    new AddressAttributeValue
                    {
                        Name = "Name 2",
                        IsPreSelected = true,
                        DisplayOrder = 1,
                    }
                );
            var fromDb = SaveAndLoadEntity(ca);
            fromDb.ShouldNotBeNull();
            fromDb.Name.ShouldEqual("Name 1");

            fromDb.AddressAttributeValues.ShouldNotBeNull();
            (fromDb.AddressAttributeValues.Count == 1).ShouldBeTrue();
            fromDb.AddressAttributeValues.First().Name.ShouldEqual("Name 2");
        }
    }
}