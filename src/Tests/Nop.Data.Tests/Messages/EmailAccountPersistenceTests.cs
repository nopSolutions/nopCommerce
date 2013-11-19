using Nop.Core.Domain.Messages;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Messages
{
    [TestFixture]
    public class EmailAccountPersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_emailAccount()
        {
            var emailAccount = new EmailAccount
            {
                Email = "admin@yourstore.com",
                DisplayName = "Administrator",
                Host = "127.0.0.1",
                Port = 125,
                Username = "John",
                Password = "111",
                EnableSsl = true,
                UseDefaultCredentials = true
            };

            var fromDb = SaveAndLoadEntity(emailAccount);
            fromDb.ShouldNotBeNull();
            fromDb.Email.ShouldEqual("admin@yourstore.com");
            fromDb.DisplayName.ShouldEqual("Administrator");
            fromDb.Host.ShouldEqual("127.0.0.1");
            fromDb.Port.ShouldEqual(125);
            fromDb.Username.ShouldEqual("John");
            fromDb.Password.ShouldEqual("111");
            fromDb.EnableSsl.ShouldBeTrue();
            fromDb.UseDefaultCredentials.ShouldBeTrue();
        }

    }
}
