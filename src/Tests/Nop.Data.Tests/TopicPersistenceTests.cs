using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nop.Core.Domain.Topics;
using NUnit.Framework;
using Nop.Tests;

namespace Nop.Data.Tests
{
    [TestFixture]
    public class TopicPersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_topic()
        {
            var topic = new Topic
                               {
                                   SystemName = "SystemName 1",
                                   IncludeInSitemap = true,
                                   Title = "Title 1",
                                   Body = "Body 1",
                                   MetaKeywords = "Meta keywords",
                                   MetaDescription = "Meta description",
                                   MetaTitle = "Meta title",
                               };

            var fromDb = SaveAndLoadEntity(topic);
            fromDb.ShouldNotBeNull();
            fromDb.SystemName.ShouldEqual("SystemName 1");
            fromDb.IncludeInSitemap.ShouldEqual(true);
            fromDb.Title.ShouldEqual("Title 1");
            fromDb.Body.ShouldEqual("Body 1");
            fromDb.MetaKeywords.ShouldEqual("Meta keywords");
            fromDb.MetaDescription.ShouldEqual("Meta description");
            fromDb.MetaTitle.ShouldEqual("Meta title");
        }
    }
}