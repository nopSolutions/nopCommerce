using Nop.Core.Domain.Messages;
using Nop.Tests;

using NUnit.Framework;

namespace Nop.Data.Tests
{
    [TestFixture]
    public class MessageTemplatePersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_messageTemplate()
        {
            var mt = new MessageTemplate()
            {
                Name = "Template1",
                BccEmailAddresses = "Bcc",
                Subject = "Subj",
                Body = "Some text",
                IsActive = true,
                EmailAccount = new EmailAccount
                                {
                                    Email = "admin@yourstore.com",
                                    DisplayName = "Administrator",
                                    Host = "127.0.0.1",
                                    Port = 125,
                                    Username = "John",
                                    Password = "111",
                                    EnableSSL = true,
                                    UseDefaultCredentials = true
                                }
            };


            var fromDb = SaveAndLoadEntity(mt);
            fromDb.ShouldNotBeNull();
            fromDb.Name.ShouldEqual("Template1");
            fromDb.BccEmailAddresses.ShouldEqual("Bcc");
            fromDb.Subject.ShouldEqual("Subj");
            fromDb.Body.ShouldEqual("Some text");
            fromDb.IsActive.ShouldBeTrue();

            fromDb.EmailAccount.ShouldNotBeNull();
            fromDb.EmailAccount.DisplayName.ShouldEqual("Administrator");
        }
    }
}