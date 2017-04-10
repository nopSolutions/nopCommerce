using System;
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
            var campaign = this.GetTestCampaign();

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