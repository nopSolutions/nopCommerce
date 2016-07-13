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
            var qe = new QueuedEmail
            {
                PriorityId = 5,
                From = "From",
                FromName = "FromName",
                To = "To",
                ToName = "ToName",
                ReplyTo = "ReplyTo",
                ReplyToName = "ReplyToName",
                CC = "CC",
                Bcc = "Bcc",
                Subject = "Subject",
                Body = "Body",
                AttachmentFilePath = "some file path",
                AttachmentFileName = "some file name",
                AttachedDownloadId = 3,
                CreatedOnUtc = new DateTime(2010, 01, 01),
                SentTries = 5,
                SentOnUtc = new DateTime(2010, 02, 02),
                DontSendBeforeDateUtc = new DateTime(2016, 2 , 23),
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
            fromDb.PriorityId.ShouldEqual(5);
            fromDb.From.ShouldEqual("From");
            fromDb.FromName.ShouldEqual("FromName");
            fromDb.To.ShouldEqual("To");
            fromDb.ToName.ShouldEqual("ToName");
            fromDb.ReplyTo.ShouldEqual("ReplyTo");
            fromDb.ReplyToName.ShouldEqual("ReplyToName");
            fromDb.CC.ShouldEqual("CC");
            fromDb.Bcc.ShouldEqual("Bcc");
            fromDb.Subject.ShouldEqual("Subject");
            fromDb.Body.ShouldEqual("Body");
            fromDb.AttachmentFilePath.ShouldEqual("some file path");
            fromDb.AttachmentFileName.ShouldEqual("some file name");
            fromDb.AttachedDownloadId.ShouldEqual(3);
            fromDb.CreatedOnUtc.ShouldEqual(new DateTime(2010, 01, 01));
            fromDb.SentTries.ShouldEqual(5);
            fromDb.SentOnUtc.ShouldNotBeNull();
            fromDb.SentOnUtc.Value.ShouldEqual(new DateTime(2010, 02, 02));
            fromDb.DontSendBeforeDateUtc.ShouldEqual(new DateTime(2016, 2, 23));
            fromDb.EmailAccount.ShouldNotBeNull();
            fromDb.EmailAccount.DisplayName.ShouldEqual("Administrator");
        }
    }
}