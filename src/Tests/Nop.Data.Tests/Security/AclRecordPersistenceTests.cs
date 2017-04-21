using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Security
{
    [TestFixture]
    public class AclRecordPersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_urlRecord()
        {
            var aclRecord = this.GetTestAclRecord();
            aclRecord.CustomerRole = this.GetTestCustomerRole();

            var fromDb = SaveAndLoadEntity(aclRecord);
            fromDb.ShouldNotBeNull();
            fromDb.PropertiesShouldEqual(this.GetTestAclRecord());
            fromDb.CustomerRole.ShouldNotBeNull();
            fromDb.CustomerRole.PropertiesShouldEqual(this.GetTestCustomerRole());
        }
    }
}
