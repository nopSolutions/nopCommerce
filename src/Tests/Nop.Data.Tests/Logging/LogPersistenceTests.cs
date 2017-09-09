using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Logging
{
    [TestFixture]
    public class LogPersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_log()
        {
            var log = this.GetTestLog();

            var fromDb = SaveAndLoadEntity(this.GetTestLog());
            fromDb.ShouldNotBeNull();
            fromDb.PropertiesShouldEqual(log);
        }

        [Test]
        public void Can_save_and_load_log_with_customer()
        {
            var log = this.GetTestLog();
            log.Customer = this.GetTestCustomer();

            var fromDb = SaveAndLoadEntity(log);
            fromDb.ShouldNotBeNull();
            fromDb.PropertiesShouldEqual(this.GetTestLog());
            
            fromDb.Customer.ShouldNotBeNull();
            fromDb.Customer.PropertiesShouldEqual(this.GetTestCustomer());
        }
    }
}
