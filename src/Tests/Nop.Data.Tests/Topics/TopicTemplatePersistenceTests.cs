using Nop.Core.Domain.Topics;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Topics
{
    [TestFixture]
    public class TopicTemplatePersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_topicTemplate()
        {
            var topicTemplate = new TopicTemplate
            {
                Name = "Name 1",
                ViewPath = "ViewPath 1",
                DisplayOrder = 1,
            };

            var fromDb = SaveAndLoadEntity(topicTemplate);
            fromDb.ShouldNotBeNull();
            fromDb.Name.ShouldEqual("Name 1");
            fromDb.ViewPath.ShouldEqual("ViewPath 1");
            fromDb.DisplayOrder.ShouldEqual(1);
        }
    }
}
