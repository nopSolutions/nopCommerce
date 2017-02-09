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
            var genericAttribute = this.GetTestGenericAttribute();

            var fromDb = SaveAndLoadEntity(this.GetTestGenericAttribute());
            fromDb.ShouldNotBeNull();
            fromDb.PropertiesShouldEqual(genericAttribute);
        }
    }
}