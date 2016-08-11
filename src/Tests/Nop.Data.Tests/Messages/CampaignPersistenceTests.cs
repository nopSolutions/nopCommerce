using System;
using Nop.Core.Domain.Messages;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Messages
{
    [TestFixture]
    public class CampaignPersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_campaign()
        {
            var campaign = new Campaign
            {
                Name = "Name 1",
                Subject = "Subject 1",
                Body = "Body 1",
                CreatedOnUtc = new DateTime(2010,01,02),
                DontSendBeforeDateUtc = new DateTime(2016, 2, 23),
                CustomerRoleId = 1,
                StoreId = 1
            };

            var fromDb = SaveAndLoadEntity(campaign);
            fromDb.ShouldNotBeNull();
            fromDb.Name.ShouldEqual("Name 1");
            fromDb.Subject.ShouldEqual("Subject 1");
            fromDb.Body.ShouldEqual("Body 1");
            fromDb.CreatedOnUtc.ShouldEqual(new DateTime(2010, 01, 02));
            fromDb.DontSendBeforeDateUtc.ShouldEqual(new DateTime(2016, 2, 23));
            fromDb.CustomerRoleId.ShouldEqual(1);
            fromDb.StoreId.ShouldEqual(1);
        }
    }
}