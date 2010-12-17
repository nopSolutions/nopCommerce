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
                PasswordHash = "hash1",
                SaltKey = "SaltKey1",
                AdminComment = "some comment here",
                Active = true,
                Deleted= false,
                RegistrationDateUtc = new DateTime(2010,01,01)
            };

            var fromDb = SaveAndLoadEntity(customer);
            fromDb.Email.ShouldEqual("admin@yourStore.com");
            fromDb.Username.ShouldEqual("admin@yourStore.com");
            fromDb.PasswordHash.ShouldEqual("hash1");
            fromDb.SaltKey.ShouldEqual("SaltKey1");
            fromDb.AdminComment.ShouldEqual("some comment here");
            fromDb.Active.ShouldEqual(true);
            fromDb.Deleted.ShouldEqual(false);
            fromDb.RegistrationDateUtc.ShouldEqual(new DateTime(2010, 01, 01));
        }
    }
}
