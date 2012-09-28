using Nop.Core.Domain.Seo;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Seo
{
    [TestFixture]
    public class UrlRecordPersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_urlRecord()
        {
            var urlRecord = new UrlRecord
            {
                EntityId = 1,
                EntityName = "EntityName 1",
                Slug = "Slug 1",
                LanguageId = 2,
            };

            var fromDb = SaveAndLoadEntity(urlRecord);
            fromDb.ShouldNotBeNull();
            fromDb.EntityId.ShouldEqual(1);
            fromDb.EntityName.ShouldEqual("EntityName 1");
            fromDb.Slug.ShouldEqual("Slug 1");
            fromDb.LanguageId.ShouldEqual(2);
        }
    }
}
