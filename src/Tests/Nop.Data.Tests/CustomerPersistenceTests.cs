using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Nop.Tests;
using Nop.Core.Domain;

namespace Nop.Data.Tests
{
    [TestFixture]
    public class CustomerPersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_customer()
        {
            var customer = new Customer
            {
                CustomerGuid = Guid.NewGuid(),
                Email = "admin@yourStore.com",
                Username = "admin@yourStore.com",
                AdminComment = "some comment here",
                Active = true,
                Deleted= false,
                CreatedOnUtc = new DateTime(2010,01,01)
            };

            var fromDb = SaveAndLoadEntity(customer);
            fromDb.Email.ShouldEqual("admin@yourStore.com");
            fromDb.Username.ShouldEqual("admin@yourStore.com");
            fromDb.AdminComment.ShouldEqual("some comment here");
            fromDb.Active.ShouldEqual(true);
            fromDb.Deleted.ShouldEqual(false);
            fromDb.CreatedOnUtc.ShouldEqual(new DateTime(2010, 01, 01));
        }

        [Test]
        public void Can_save_and_load_customer_with_customerRoles()
        {
            var customer = new Customer
            {
                CustomerGuid = Guid.NewGuid(),
                Email = "admin@yourStore.com",
                Username = "admin@yourStore.com",
                AdminComment = "some comment here",
                Active = true,
                Deleted = false,
                CreatedOnUtc = new DateTime(2010, 01, 01),
                CustomerRoles = new List<CustomerRole>()
                {
                     new CustomerRole()
                     {
                         Name = "Administrators",
                         FreeShipping = true,
                         TaxExempt = true,
                         Active = true,
                         IsSystemRole = true,
                         SystemName = "Administrators"
                     }
                }
            };


            var fromDb = SaveAndLoadEntity(customer);
            fromDb.Email.ShouldEqual("admin@yourStore.com");

            fromDb.CustomerRoles.ShouldNotBeNull();
            (fromDb.CustomerRoles.Count == 1).ShouldBeTrue();
            fromDb.CustomerRoles.First().Name.ShouldEqual("Administrators");
        }
    }
}
