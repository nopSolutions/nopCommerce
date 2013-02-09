using Nop.Core.Domain.Common;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Common
{
    [TestFixture]
    public class GenericAttributePersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_genericAttribute()
        {
            var genericAttribute = new GenericAttribute
                               {
                                   EntityId = 1,
                                   KeyGroup = "KeyGroup 1",
                                   Key = "Key 1",
                                   Value = "Value 1",
                                   StoreId = 2,
                               };

            var fromDb = SaveAndLoadEntity(genericAttribute);
            fromDb.ShouldNotBeNull();
            fromDb.EntityId.ShouldEqual(1);
            fromDb.KeyGroup.ShouldEqual("KeyGroup 1");
            fromDb.Key.ShouldEqual("Key 1");
            fromDb.Value.ShouldEqual("Value 1");
            fromDb.StoreId.ShouldEqual(2);
        }
    }
}