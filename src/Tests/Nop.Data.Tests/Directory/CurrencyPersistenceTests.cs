using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Directory
{
    [TestFixture]
    public class CurrencyPersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_currency()
        {
            var currency = this.GetTestCurrency();

            var fromDb = SaveAndLoadEntity(currency);
            fromDb.ShouldNotBeNull();
            fromDb.PropertiesShouldEqual(currency);
        }
    }
}
