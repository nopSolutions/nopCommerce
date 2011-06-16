using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nop.Core.Domain.Customers;
using NUnit.Framework;
using Nop.Tests;
using Nop.Core.Domain.Security;

namespace Nop.Data.Tests.Security
{
    [TestFixture]
    public class UserPersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_user()
        {
            var user = new User
            {
                Username = "a@b.com",
                Password = "password",
                PasswordFormat = PasswordFormat.Clear,
                PasswordSalt = "",
                Email = "a@b.com",
                IsApproved = true,
                IsLockedOut = false,
                CreatedOnUtc = DateTime.UtcNow
            };

            var fromDb = SaveAndLoadEntity(user);
            fromDb.ShouldNotBeNull();
        }

        [Test]
        public void Can_save_and_load_user_with_associatedCustomer()
        {
            var user = new User
            {
                Username = "a@b.com",
                Password = "password",
                PasswordFormat = PasswordFormat.Clear,
                PasswordSalt = "",
                Email = "a@b.com",
                IsApproved = true,
                IsLockedOut = false,
                CreatedOnUtc = DateTime.UtcNow,
                AssociatedCustomer = GetTestCustomer()
            };

            var fromDb = SaveAndLoadEntity(user);
            fromDb.ShouldNotBeNull();
            fromDb.AssociatedCustomer.ShouldNotBeNull();
            fromDb.AssociatedCustomer.AdminComment.ShouldEqual("some comment here");
        }

        protected Customer GetTestCustomer()
        {
            return new Customer
            {
                CustomerGuid = Guid.NewGuid(),
                AdminComment = "some comment here",
                Active = true,
                Deleted = false,
                CreatedOnUtc = new DateTime(2010, 01, 01)
            };
        }

    }
}
