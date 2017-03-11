using System.Linq;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Customers
{
    [TestFixture]
    public class CustomerRolePersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_customerRole()
        {
            var customerRole = this.GetTestCustomerRole();

            var fromDb = SaveAndLoadEntity(this.GetTestCustomerRole());
            fromDb.ShouldNotBeNull();
            fromDb.PropertiesShouldEqual(customerRole);
        }

        [Test]
        public void Can_save_and_load_customerRole_with_permissions()
        {
            var customerRole = this.GetTestCustomerRole();
            customerRole.PermissionRecords.Add(this.GetTestPermissionRecord());

            var fromDb = SaveAndLoadEntity(customerRole);
            fromDb.ShouldNotBeNull();
            fromDb.PropertiesShouldEqual(this.GetTestCustomerRole());

            fromDb.PermissionRecords.ShouldNotBeNull();
            (fromDb.PermissionRecords.Count == 1).ShouldBeTrue();
            fromDb.PermissionRecords.First().PropertiesShouldEqual(this.GetTestPermissionRecord());
        }
    }
}