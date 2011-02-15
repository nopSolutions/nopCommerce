using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nop.Core.Domain.Security.Permissions;
using NUnit.Framework;
using Nop.Tests;
using Nop.Core.Domain;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Tax;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Orders;

namespace Nop.Data.Tests
{
    [TestFixture]
    public class PermissionRecordPersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_permissionRecord()
        {
            var permissionRecord = GetTestPermissionRecord();

            var fromDb = SaveAndLoadEntity(permissionRecord);
            fromDb.ShouldNotBeNull();
            fromDb.Name.ShouldEqual("Name 1");
            fromDb.SystemName.ShouldEqual("SystemName 2");
            fromDb.Category.ShouldEqual("Category 4");
        }

        [Test]
        public void Can_save_and_load_permissionRecord_with_customerRoles()
        {
            var permissionRecord = GetTestPermissionRecord();
            permissionRecord.CustomerRoles = new List<CustomerRole>()
            {
                new CustomerRole()
                {
                    Name = "Administrators",
                    SystemName = "Administrators"
                }
            };


            var fromDb = SaveAndLoadEntity(permissionRecord);
            fromDb.ShouldNotBeNull();

            fromDb.CustomerRoles.ShouldNotBeNull();
            (fromDb.CustomerRoles.Count == 1).ShouldBeTrue();
            fromDb.CustomerRoles.First().Name.ShouldEqual("Administrators");
        }

        protected PermissionRecord GetTestPermissionRecord()
        {
            return new PermissionRecord
            {
                Name = "Name 1",
                SystemName = "SystemName 2",
                Category = "Category 4",
            };
        }
    }
}
