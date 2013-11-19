using Nop.Core.Domain.Topics;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Topics
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
                                   IsPasswordProtected = true,
                                   Password = "password",
                                   Title = "Title 1",
                                   Body = "Body 1",
                                   MetaKeywords = "Meta keywords",
                                   MetaDescription = "Meta description",
                                   MetaTitle = "Meta title",
                                   LimitedToStores = true
                               };

            var fromDb = SaveAndLoadEntity(topic);
            fromDb.ShouldNotBeNull();
            fromDb.SystemName.ShouldEqual("SystemName 1");
            fromDb.IncludeInSitemap.ShouldEqual(true);
            fromDb.IsPasswordProtected.ShouldEqual(true);
            fromDb.Password.ShouldEqual("password");
            fromDb.Title.ShouldEqual("Title 1");
            fromDb.Body.ShouldEqual("Body 1");
            fromDb.MetaKeywords.ShouldEqual("Meta keywords");
            fromDb.MetaDescription.ShouldEqual("Meta description");
            fromDb.MetaTitle.ShouldEqual("Meta title");
            fromDb.LimitedToStores.ShouldEqual(true);
        }
    }
}