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
    public class CustomerSessionTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_customerSession()
        {
            var customerSession = new CustomerSession
            {
                CustomerSessionGuid = Guid.NewGuid(),
                IsExpired = true,
                LastAccessedUtc= new DateTime(2010, 01, 01),
            };

            var fromDb = SaveAndLoadEntity(customerSession);
            fromDb.IsExpired.ShouldEqual(true);
            fromDb.LastAccessedUtc.ShouldEqual(new DateTime(2010, 01, 01));
        }

        [Test]
        public void Can_save_and_load_customerSession_with_customer()
        {
            var customerSession = new CustomerSession
            {
                CustomerSessionGuid = Guid.NewGuid(),
                IsExpired = true,
                LastAccessedUtc = new DateTime(2010, 01, 01),
                Customer = new Customer()
                {
                    CustomerGuid = Guid.NewGuid(),
                    Email = "admin@yourStore.com",
                    Username = "admin@yourStore.com",
                    AdminComment = "some comment here",
                    Active = true,
                    Deleted = false,
                    CreatedOnUtc = new DateTime(2010, 01, 01)
                }
            };

            var fromDb = SaveAndLoadEntity(customerSession);
            fromDb.IsExpired.ShouldEqual(true);

            fromDb.Customer.ShouldNotBeNull();
            fromDb.Customer.Email.ShouldEqual("admin@yourStore.com");
        }
    }
}