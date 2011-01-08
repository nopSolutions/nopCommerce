using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Nop.Tests;
using Nop.Core.Domain;
using Nop.Core.Domain.Customers;

namespace Nop.Data.Tests
{
    [TestFixture]
    public class CustomerRolePersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_customerRole()
        {
            var customerRole = new CustomerRole
            {
                Name = "Administrators",
                FreeShipping = true,
                TaxExempt = true,
                Active = true,
                IsSystemRole = true,
                SystemName = "Administrators"
            };

            var fromDb = SaveAndLoadEntity(customerRole);
            fromDb.Name.ShouldEqual("Administrators");
            fromDb.FreeShipping.ShouldEqual(true);
            fromDb.TaxExempt.ShouldEqual(true);
            fromDb.Active.ShouldEqual(true);
            fromDb.IsSystemRole.ShouldEqual(true);
            fromDb.SystemName.ShouldEqual("Administrators");
        }

        [Test]
        public void Can_save_and_load_customerRole_with_customer()
        {
            var customerRole = new CustomerRole
            {
                Name = "Administrators",
                FreeShipping = true,
                TaxExempt = true,
                Active = true,
                IsSystemRole = true,
                SystemName = "Administrators",
                Customers = new List<Customer>()
                {
                    new Customer()
                    {
                        CustomerGuid = Guid.NewGuid(),
                        Email = "admin@yourStore.com",
                        Username = "admin@yourStore.com",
                        AdminComment = "some comment here",
                        Active = true,
                        CreatedOnUtc = new DateTime(2010, 01, 01)
                    }
                }
            };

            var fromDb = SaveAndLoadEntity(customerRole);
            fromDb.Name.ShouldEqual("Administrators");

            fromDb.Customers.ShouldNotBeNull();
            (fromDb.Customers.Count == 1).ShouldBeTrue();
            fromDb.Customers.First().Email.ShouldEqual("admin@yourStore.com");
        }
    }
}