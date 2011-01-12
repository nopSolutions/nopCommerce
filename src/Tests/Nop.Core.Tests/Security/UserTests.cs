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
    }
}
