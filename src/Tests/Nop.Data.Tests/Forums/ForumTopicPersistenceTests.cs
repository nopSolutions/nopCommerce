using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Forums
{
    [TestFixture]
    public class ForumTopicPersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_forumtopic()
        {
            var customer = this.GetTestCustomer();
            var customerFromDb = SaveAndLoadEntity(customer);
            customerFromDb.ShouldNotBeNull();

            var forumGroup = this.GetTestForumGroup();

            var forumGroupFromDb = SaveAndLoadEntity(forumGroup);
            forumGroupFromDb.ShouldNotBeNull();
            forumGroupFromDb.PropertiesShouldEqual(this.GetTestForumGroup());

            var forum = this.GetTestForum();
            forum.ForumGroup = forumGroupFromDb;

            forumGroup.Forums.Add(forum);
            var forumFromDb = SaveAndLoadEntity(forum);
            forumFromDb.ShouldNotBeNull();
            forumFromDb.PropertiesShouldEqual(this.GetTestForum());
            forumFromDb.ForumGroupId.ShouldEqual(forumGroupFromDb.Id);

            var forumTopic = this.GetTestForumTopic();
            forumTopic.ForumId = forumFromDb.Id;
            forumTopic.CustomerId = customerFromDb.Id;

            var forumTopicFromDb = SaveAndLoadEntity(forumTopic);
            forumTopicFromDb.ShouldNotBeNull();
            forumTopicFromDb.PropertiesShouldEqual(this.GetTestForumTopic());
            forumTopicFromDb.ForumId.ShouldEqual(forumFromDb.Id);
        }
    }
}