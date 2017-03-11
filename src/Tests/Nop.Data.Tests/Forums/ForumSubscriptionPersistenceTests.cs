using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Forums
{
    [TestFixture]
    public class ForumSubscriptionPersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_forum_subscription_forum_subscribed()
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

            var forumSubscription = this.GetTestForumSubscription();
            forumSubscription.ForumId = forumFromDb.Id;
            forumSubscription.CustomerId = customerFromDb.Id;

            var forumSubscriptionFromDb = SaveAndLoadEntity(forumSubscription);
            forumSubscriptionFromDb.ShouldNotBeNull();
            forumSubscriptionFromDb.PropertiesShouldEqual(this.GetTestForumSubscription(), "ForumId", "CustomerId");
            forumSubscriptionFromDb.ForumId.ShouldEqual(forumFromDb.Id);
            forumSubscriptionFromDb.CustomerId.ShouldEqual(customerFromDb.Id);
        }

        [Test]
        public void Can_save_and_load_forum_subscription_topic_subscribed()
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

            var forumSubscription = this.GetTestForumSubscription();
            forumSubscription.TopicId = forumTopicFromDb.Id;
            forumSubscription.CustomerId = customerFromDb.Id;

            var forumSubscriptionFromDb = SaveAndLoadEntity(forumSubscription);
            forumSubscriptionFromDb.ShouldNotBeNull();
            forumSubscriptionFromDb.PropertiesShouldEqual(this.GetTestForumSubscription(), "TopicId", "CustomerId");
            forumSubscriptionFromDb.TopicId.ShouldEqual(forumTopicFromDb.Id);
            forumSubscriptionFromDb.CustomerId.ShouldEqual(customerFromDb.Id);
        }
    }
}
