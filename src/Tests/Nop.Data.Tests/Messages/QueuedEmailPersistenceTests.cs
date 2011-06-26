using System;
using Nop.Core.Domain.Messages;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Messages
{
    [TestFixture]
    public class QueuedEmailPersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_queuedEmail()
        {
            var qe = new QueuedEmail()
            {
                Priority = 1,
                From = "From",
                FromName = "FromName",
                To = "To",
                ToName = "ToName",
                CC = "CC",
                Bcc = "Bcc",
                Subject = "Subject",
                Body = "Body",
                CreatedOnUtc = new DateTime(2010, 01, 01),
                SentTries = 5,
                SentOnUtc = new DateTime(2010, 02, 02),
                EmailAccount = new EmailAccount
                {
                    Email = "admin@yourstore.com",
                    DisplayName = "Administrator",
                    Host = "127.0.0.1",
                    Port = 125,
                    Username = "John",
                    Password = "111",
                    EnableSsl = true,
                    UseDefaultCredentials = true
                }

            };


            var fromDb = SaveAndLoadEntity(qe);
            fromDb.ShouldNotBeNull();
            fromDb.Priority.ShouldEqual(1);
            fromDb.From.ShouldEqual("From");
            fromDb.FromName.ShouldEqual("FromName");
            fromDb.To.ShouldEqual("To");
            fromDb.ToName.ShouldEqual("ToName");
            fromDb.CC.ShouldEqual("CC");
            fromDb.Bcc.ShouldEqual("Bcc");
            fromDb.Subject.ShouldEqual("Subject");
            fromDb.Body.ShouldEqual("Body");
            fromDb.CreatedOnUtc.ShouldEqual(new DateTime(2010, 01, 01));
            fromDb.SentTries.ShouldEqual(5);
            fromDb.SentOnUtc.Value.ShouldEqual(new DateTime(2010, 02, 02));

            fromDb.EmailAccount.ShouldNotBeNull();
            fromDb.EmailAccount.DisplayName.ShouldEqual("Administrator");
        }
    }
}