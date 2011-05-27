using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nop.Core.Domain.Messages;
using NUnit.Framework;
using Nop.Tests;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;

namespace Nop.Data.Tests
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
                CreatedOnUtc = new DateTime(2010,01,02)
            };

            var fromDb = SaveAndLoadEntity(campaign);
            fromDb.ShouldNotBeNull();
            fromDb.Name.ShouldEqual("Name 1");
            fromDb.Subject.ShouldEqual("Subject 1");
            fromDb.Body.ShouldEqual("Body 1");
            fromDb.CreatedOnUtc.ShouldEqual(new DateTime(2010, 01, 02));
        }
    }
}