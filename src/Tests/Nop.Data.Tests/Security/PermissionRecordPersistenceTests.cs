using System.Linq;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Security
{
    [TestFixture]
    public class PermissionRecordPersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_permissionRecord()
        {
            var permissionRecord = this.GetTestPermissionRecord();

            var fromDb = SaveAndLoadEntity(this.GetTestPermissionRecord());
            fromDb.ShouldNotBeNull();
            fromDb.PropertiesShouldEqual(permissionRecord);
        }

        [Test]
        public void Can_save_and_load_permissionRecord_with_customerRoles()
        {
            var permissionRecord = this.GetTestPermissionRecord();
            permissionRecord.CustomerRoles.Add(this.GetTestCustomerRole());
            
            var fromDb = SaveAndLoadEntity(permissionRecord);
            fromDb.ShouldNotBeNull();
            fromDb.PropertiesShouldEqual(this.GetTestPermissionRecord());

            fromDb.CustomerRoles.ShouldNotBeNull();
            (fromDb.CustomerRoles.Count == 1).ShouldBeTrue();
            fromDb.CustomerRoles.First().PropertiesShouldEqual(this.GetTestCustomerRole());
        }
    }
}
