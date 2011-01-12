using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Nop.Tests;
using Nop.Core.Domain.Security;

namespace Nop.Data.Tests
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
                FirstName = "Test",
                LastName = "Test",
                Email = "a@b.com",
                SecurityQuestion = "",
                SecurityAnswer = "",
                IsApproved = true,
                IsLockedOut = false,
                CreatedOnUtc = DateTime.UtcNow
            };

            var fromDb = SaveAndLoadEntity(user);
            fromDb.ShouldNotBeNull();
        }
    }
}
