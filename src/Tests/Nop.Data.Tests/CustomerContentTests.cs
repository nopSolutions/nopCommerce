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
    public class CustomerContentTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_customerContent()
        {
            var customerContent = new CustomerContent
                               {
                                   IpAddress = "192.168.1.1",
                                   IsApproved = true,
                                   CreatedOnUtc = new DateTime(2010, 01, 01),
                                   UpdatedOnUtc = new DateTime(2010, 01, 02),
                                   Customer = new Customer()
                                   {
                                       CustomerGuid = Guid.NewGuid(),
                                       Email = "admin@yourStore.com",
                                       Username = "admin@yourStore.com",
                                       PasswordHash = "hash1",
                                       SaltKey = "SaltKey1",
                                       AdminComment = "some comment here",
                                       Active = true,
                                       Deleted = false,
                                       RegistrationDateUtc = new DateTime(2010, 01, 01)
                                   }
                               };

            var fromDb = SaveAndLoadEntity(customerContent);
            fromDb.IpAddress.ShouldEqual("192.168.1.1");
            fromDb.IsApproved.ShouldEqual(true);
            fromDb.CreatedOnUtc.ShouldEqual(new DateTime(2010, 01, 01));
            fromDb.UpdatedOnUtc.ShouldEqual(new DateTime(2010, 01, 02));

            fromDb.Customer.ShouldNotBeNull();
            fromDb.Customer.Email.ShouldEqual("admin@yourStore.com");
        }
    }
}