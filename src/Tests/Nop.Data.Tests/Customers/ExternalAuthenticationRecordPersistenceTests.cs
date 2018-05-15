using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Customers
{
    [TestFixture]
    public class ExternalAuthenticationRecordPersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_externalAuthenticationRecord()
        {
            var externalAuthenticationRecord = this.GetTestExternalAuthenticationRecord();
            externalAuthenticationRecord.Customer = this.GetTestCustomer();

            var fromDb = SaveAndLoadEntity(externalAuthenticationRecord);
            fromDb.ShouldNotBeNull();
            fromDb.PropertiesShouldEqual(this.GetTestExternalAuthenticationRecord());

            fromDb.Customer.ShouldNotBeNull();
            fromDb.Customer.PropertiesShouldEqual(this.GetTestCustomer());
        }
    }
}