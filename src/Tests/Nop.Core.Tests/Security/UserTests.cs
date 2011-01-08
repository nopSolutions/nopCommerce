using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Nop.Tests;
using Nop.Core.Domain.Security;

namespace Nop.Core.Tests.Security
{
    [TestFixture]
    public class UserTests {
        [Test]
        public void New_user_has_clear_password_type() {
            var user = new User();
            user.PasswordFormat.ShouldEqual(PasswordFormat.Clear);
        }

        [Test]
        public void Setting_username_sets_lowered_username() {
            var user = new User { Username = "TestUser" };
            user.LoweredUsername.ShouldEqual("testuser");
        }

        [Test]
        public void Setting_email_sets_lowered_email() {
            var user = new User { Email = "Test.User@SomeDomain.com" };
            user.LoweredEmail.ShouldEqual("test.user@somedomain.com");
        }
    }
}
