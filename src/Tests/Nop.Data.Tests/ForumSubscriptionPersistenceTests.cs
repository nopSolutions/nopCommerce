using System;
using System.Collections.Generic;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Forums;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests
{
    [TestFixture]
    public class ForumSubscriptionPersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_forum_subscription_forum_subscribed()
        {
            var customer = GetTestCustomer();
            var customerFromDb = SaveAndLoadEntity(customer);
            customerFromDb.ShouldNotBeNull();

            var forumGroup = new ForumGroup
            {
                Name = "Forum Group 1",
                Description = "Forum Group 1 Description",
                DisplayOrder = 1,
                CreatedOn = DateTime.Now,
                UpdatedOn = DateTime.Now,
                Forums = new List<Forum>(),
            };

            var forumGroupFromDb = SaveAndLoadEntity(forumGroup);
            forumGroupFromDb.ShouldNotBeNull();
            forumGroupFromDb.Name.ShouldEqual("Forum Group 1");
            forumGroupFromDb.Description.ShouldEqual("Forum Group 1 Description");
            forumGroupFromDb.DisplayOrder.ShouldEqual(1);

            var forum = new Forum
            {
                ForumGroup = forumGroupFromDb,
                Name = "Forum 1",
                Description = "Forum 1 Description",
                ForumGroupId = forumGroupFromDb.Id,
                DisplayOrder = 10,
                CreatedOn = DateTime.Now,
                UpdatedOn = DateTime.Now,
                NumPosts = 25,
                NumTopics = 15,
                ForumTopics = new List<ForumTopic>(),
            };

            forumGroup.Forums.Add(forum);
            var forumFromDb = SaveAndLoadEntity(forum);
            forumFromDb.ShouldNotBeNull();
            forumFromDb.Name.ShouldEqual("Forum 1");
            forumFromDb.Description.ShouldEqual("Forum 1 Description");
            forumFromDb.DisplayOrder.ShouldEqual(10);
            forumFromDb.NumTopics.ShouldEqual(15);
            forumFromDb.NumPosts.ShouldEqual(25);
            forumFromDb.ForumGroupId.ShouldEqual(forumGroupFromDb.Id);

            var forumTopic = new ForumTopic
            {
                Subject = "Forum Topic 1",
                Forum = forumFromDb,
                ForumId = forumFromDb.Id,
                TopicTypeId = (int)ForumTopicTypeEnum.Sticky,
                Views = 123,
                CreatedOn = DateTime.Now,
                UpdatedOn = DateTime.Now,
                NumPosts = 100,
                UserId = customerFromDb.Id,
            };
            forum.ForumTopics.Add(forumTopic);

            var forumTopicFromDb = SaveAndLoadEntity(forumTopic);
            forumTopicFromDb.ShouldNotBeNull();
            forumTopicFromDb.Subject.ShouldEqual("Forum Topic 1");
            forumTopicFromDb.Views.ShouldEqual(123);
            forumTopicFromDb.NumPosts.ShouldEqual(100);
            forumTopicFromDb.TopicTypeId.ShouldEqual((int)ForumTopicTypeEnum.Sticky);
            forumTopicFromDb.ForumId.ShouldEqual(forumFromDb.Id);

            var forumSubscription = new ForumSubscription
            {
                CreatedOn = DateTime.Now,
                SubscriptionGuid = new Guid("11111111-2222-3333-4444-555555555555"),
                ForumId = forumFromDb.Id,
                UserId = customerFromDb.Id,
            };

            var forumSubscriptionFromDb = SaveAndLoadEntity(forumSubscription);
            forumSubscriptionFromDb.ShouldNotBeNull();
            forumSubscriptionFromDb.SubscriptionGuid.ToString().ShouldEqual("11111111-2222-3333-4444-555555555555");
            forumSubscriptionFromDb.TopicId.ShouldEqual(0);
            forumSubscriptionFromDb.ForumId.ShouldEqual(forumFromDb.Id);
        }

        [Test]
        public void Can_save_and_load_forum_subscription_topic_subscribed()
        {
            var customer = GetTestCustomer();
            var customerFromDb = SaveAndLoadEntity(customer);
            customerFromDb.ShouldNotBeNull();

            var forumGroup = new ForumGroup
            {
                Name = "Forum Group 1",
                Description = "Forum Group 1 Description",
                DisplayOrder = 1,
                CreatedOn = DateTime.Now,
                UpdatedOn = DateTime.Now,
                Forums = new List<Forum>(),
            };

            var forumGroupFromDb = SaveAndLoadEntity(forumGroup);
            forumGroupFromDb.ShouldNotBeNull();
            forumGroupFromDb.Name.ShouldEqual("Forum Group 1");
            forumGroupFromDb.Description.ShouldEqual("Forum Group 1 Description");
            forumGroupFromDb.DisplayOrder.ShouldEqual(1);

            var forum = new Forum
            {
                ForumGroup = forumGroupFromDb,
                Name = "Forum 1",
                Description = "Forum 1 Description",
                ForumGroupId = forumGroupFromDb.Id,
                DisplayOrder = 10,
                CreatedOn = DateTime.Now,
                UpdatedOn = DateTime.Now,
                NumPosts = 25,
                NumTopics = 15,
                ForumTopics = new List<ForumTopic>(),
            };

            forumGroup.Forums.Add(forum);
            var forumFromDb = SaveAndLoadEntity(forum);
            forumFromDb.ShouldNotBeNull();
            forumFromDb.Name.ShouldEqual("Forum 1");
            forumFromDb.Description.ShouldEqual("Forum 1 Description");
            forumFromDb.DisplayOrder.ShouldEqual(10);
            forumFromDb.NumTopics.ShouldEqual(15);
            forumFromDb.NumPosts.ShouldEqual(25);
            forumFromDb.ForumGroupId.ShouldEqual(forumGroupFromDb.Id);

            var forumTopic = new ForumTopic
            {
                Subject = "Forum Topic 1",
                Forum = forumFromDb,
                ForumId = forumFromDb.Id,
                TopicTypeId = (int)ForumTopicTypeEnum.Sticky,
                Views = 123,
                CreatedOn = DateTime.Now,
                UpdatedOn = DateTime.Now,
                NumPosts = 100,
                UserId = customerFromDb.Id,
            };
            forum.ForumTopics.Add(forumTopic);

            var forumTopicFromDb = SaveAndLoadEntity(forumTopic);
            forumTopicFromDb.ShouldNotBeNull();
            forumTopicFromDb.Subject.ShouldEqual("Forum Topic 1");
            forumTopicFromDb.Views.ShouldEqual(123);
            forumTopicFromDb.NumPosts.ShouldEqual(100);
            forumTopicFromDb.TopicTypeId.ShouldEqual((int)ForumTopicTypeEnum.Sticky);
            forumTopicFromDb.ForumId.ShouldEqual(forumFromDb.Id);

            var forumSubscription = new ForumSubscription
            {
                CreatedOn = DateTime.Now,
                SubscriptionGuid = new Guid("11111111-2222-3333-4444-555555555555"),
                TopicId = forumTopicFromDb.Id,
                UserId = customerFromDb.Id,
            };

            var forumSubscriptionFromDb = SaveAndLoadEntity(forumSubscription);
            forumSubscriptionFromDb.ShouldNotBeNull();
            forumSubscriptionFromDb.SubscriptionGuid.ToString().ShouldEqual("11111111-2222-3333-4444-555555555555");
            forumSubscriptionFromDb.TopicId.ShouldEqual(forumTopicFromDb.Id);
            forumSubscriptionFromDb.ForumId.ShouldEqual(0);
        }

        protected Customer GetTestCustomer()
        {
            return new Customer
            {
                CustomerGuid = Guid.NewGuid(),
                AdminComment = "some comment here",
                Active = true,
                Deleted = false,
                CreatedOnUtc = new DateTime(2010, 01, 01)
            };
        }
    }
}
