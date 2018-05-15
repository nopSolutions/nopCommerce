using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Forums
{
    [TestFixture]
    public class ForumPostPersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_forumpost()
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
            forumTopicFromDb.PropertiesShouldEqual(this.GetTestForumTopic());
            forumTopicFromDb.ForumId.ShouldEqual(forumFromDb.Id);
            forumTopicFromDb.CustomerId.ShouldEqual(customerFromDb.Id);

            var forumPost = this.GetTestForumPost();
            forumPost.CustomerId = customerFromDb.Id;
            forumPost.TopicId = forumTopicFromDb.Id;

            var forumPostFromDb = SaveAndLoadEntity(forumPost);
            forumPostFromDb.ShouldNotBeNull();
            forumPostFromDb.PropertiesShouldEqual(this.GetTestForumPost(), "TopicId");
            forumPostFromDb.TopicId.ShouldEqual(forumTopicFromDb.Id);
            forumPostFromDb.CustomerId.ShouldEqual(customerFromDb.Id);
        }
    }
}