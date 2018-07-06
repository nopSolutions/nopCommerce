using System;
using System.Collections.Generic;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Forums;

namespace Nop.Services.Forums
{
    /// <summary>
    /// Forum service interface
    /// </summary>
    public partial interface IForumService
    {
        /// <summary>
        /// Deletes a forum group
        /// </summary>
        /// <param name="forumGroup">Forum group</param>
        void DeleteForumGroup(ForumGroup forumGroup);

        /// <summary>
        /// Gets a forum group
        /// </summary>
        /// <param name="forumGroupId">The forum group identifier</param>
        /// <returns>Forum group</returns>
        ForumGroup GetForumGroupById(int forumGroupId);

        /// <summary>
        /// Gets all forum groups
        /// </summary>
        /// <returns>Forum groups</returns>
        IList<ForumGroup> GetAllForumGroups();

        /// <summary>
        /// Inserts a forum group
        /// </summary>
        /// <param name="forumGroup">Forum group</param>
        void InsertForumGroup(ForumGroup forumGroup);

        /// <summary>
        /// Updates the forum group
        /// </summary>
        /// <param name="forumGroup">Forum group</param>
        void UpdateForumGroup(ForumGroup forumGroup);

        /// <summary>
        /// Deletes a forum
        /// </summary>
        /// <param name="forum">Forum</param>
        void DeleteForum(Forum forum);

        /// <summary>
        /// Gets a forum
        /// </summary>
        /// <param name="forumId">The forum identifier</param>
        /// <returns>Forum</returns>
        Forum GetForumById(int forumId);

        /// <summary>
        /// Gets forums by group identifier
        /// </summary>
        /// <param name="forumGroupId">The forum group identifier</param>
        /// <returns>Forums</returns>
        IList<Forum> GetAllForumsByGroupId(int forumGroupId);

        /// <summary>
        /// Inserts a forum
        /// </summary>
        /// <param name="forum">Forum</param>
        void InsertForum(Forum forum);

        /// <summary>
        /// Updates the forum
        /// </summary>
        /// <param name="forum">Forum</param>
        void UpdateForum(Forum forum);

        /// <summary>
        /// Deletes a forum topic
        /// </summary>
        /// <param name="forumTopic">Forum topic</param>
        void DeleteTopic(ForumTopic forumTopic);

        /// <summary>
        /// Gets a forum topic
        /// </summary>
        /// <param name="forumTopicId">The forum topic identifier</param>
        /// <returns>Forum Topic</returns>
        ForumTopic GetTopicById(int forumTopicId);

        /// <summary>
        /// Gets a forum topic
        /// </summary>
        /// <param name="forumTopicId">The forum topic identifier</param>
        /// <param name="increaseViews">The value indicating whether to increase forum topic views</param>
        /// <returns>Forum Topic</returns>
        ForumTopic GetTopicById(int forumTopicId, bool increaseViews);

        /// <summary>
        /// Gets all forum topics
        /// </summary>
        /// <param name="forumId">The forum identifier</param>
        /// <param name="customerId">The customer identifier</param>
        /// <param name="keywords">Keywords</param>
        /// <param name="searchType">Search type</param>
        /// <param name="limitDays">Limit by the last number days; 0 to load all topics</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Forum Topics</returns>
        IPagedList<ForumTopic> GetAllTopics(int forumId = 0,
            int customerId = 0, string keywords = "", ForumSearchType searchType = ForumSearchType.All,
            int limitDays = 0, int pageIndex = 0, int pageSize = int.MaxValue);

        /// <summary>
        /// Gets active forum topics
        /// </summary>
        /// <param name="forumId">The forum identifier</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Forum Topics</returns>
        IPagedList<ForumTopic> GetActiveTopics(int forumId = 0,
            int pageIndex = 0, int pageSize = int.MaxValue);

        /// <summary>
        /// Inserts a forum topic
        /// </summary>
        /// <param name="forumTopic">Forum topic</param>
        /// <param name="sendNotifications">A value indicating whether to send notifications to subscribed customers</param>
        void InsertTopic(ForumTopic forumTopic, bool sendNotifications);

        /// <summary>
        /// Updates the forum topic
        /// </summary>
        /// <param name="forumTopic">Forum topic</param>
        void UpdateTopic(ForumTopic forumTopic);

        /// <summary>
        /// Moves the forum topic
        /// </summary>
        /// <param name="forumTopicId">The forum topic identifier</param>
        /// <param name="newForumId">New forum identifier</param>
        /// <returns>Moved forum topic</returns>
        ForumTopic MoveTopic(int forumTopicId, int newForumId);

        /// <summary>
        /// Deletes a forum post
        /// </summary>
        /// <param name="forumPost">Forum post</param>
        void DeletePost(ForumPost forumPost);

        /// <summary>
        /// Gets a forum post
        /// </summary>
        /// <param name="forumPostId">The forum post identifier</param>
        /// <returns>Forum Post</returns>
        ForumPost GetPostById(int forumPostId);

        /// <summary>
        /// Gets all forum posts
        /// </summary>
        /// <param name="forumTopicId">The forum topic identifier</param>
        /// <param name="customerId">The customer identifier</param>
        /// <param name="keywords">Keywords</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Posts</returns>
        IPagedList<ForumPost> GetAllPosts(int forumTopicId = 0,
            int customerId = 0, string keywords = "",
            int pageIndex = 0, int pageSize = int.MaxValue);

        /// <summary>
        /// Gets all forum posts
        /// </summary>
        /// <param name="forumTopicId">The forum topic identifier</param>
        /// <param name="customerId">The customer identifier</param>
        /// <param name="keywords">Keywords</param>
        /// <param name="ascSort">Sort order</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Forum Posts</returns>
        IPagedList<ForumPost> GetAllPosts(int forumTopicId = 0, int customerId = 0,
            string keywords = "", bool ascSort = false,
            int pageIndex = 0, int pageSize = int.MaxValue);

        /// <summary>
        /// Inserts a forum post
        /// </summary>
        /// <param name="forumPost">The forum post</param>
        /// <param name="sendNotifications">A value indicating whether to send notifications to subscribed customers</param>
        void InsertPost(ForumPost forumPost, bool sendNotifications);

        /// <summary>
        /// Updates the forum post
        /// </summary>
        /// <param name="forumPost">Forum post</param>
        void UpdatePost(ForumPost forumPost);

        /// <summary>
        /// Deletes a private message
        /// </summary>
        /// <param name="privateMessage">Private message</param>
        void DeletePrivateMessage(PrivateMessage privateMessage);

        /// <summary>
        /// Gets a private message
        /// </summary>
        /// <param name="privateMessageId">The private message identifier</param>
        /// <returns>Private message</returns>
        PrivateMessage GetPrivateMessageById(int privateMessageId);

        /// <summary>
        /// Gets private messages
        /// </summary>
        /// <param name="storeId">The store identifier; pass 0 to load all messages</param>
        /// <param name="fromCustomerId">The customer identifier who sent the message</param>
        /// <param name="toCustomerId">The customer identifier who should receive the message</param>
        /// <param name="isRead">A value indicating whether loaded messages are read. false - to load not read messages only, 1 to load read messages only, null to load all messages</param>
        /// <param name="isDeletedByAuthor">A value indicating whether loaded messages are deleted by author. false - messages are not deleted by author, null to load all messages</param>
        /// <param name="isDeletedByRecipient">A value indicating whether loaded messages are deleted by recipient. false - messages are not deleted by recipient, null to load all messages</param>
        /// <param name="keywords">Keywords</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Private messages</returns>
        IPagedList<PrivateMessage> GetAllPrivateMessages(int storeId, int fromCustomerId,
            int toCustomerId, bool? isRead, bool? isDeletedByAuthor, bool? isDeletedByRecipient,
            string keywords, int pageIndex = 0, int pageSize = int.MaxValue);

        /// <summary>
        /// Inserts a private message
        /// </summary>
        /// <param name="privateMessage">Private message</param>
        void InsertPrivateMessage(PrivateMessage privateMessage);

        /// <summary>
        /// Updates the private message
        /// </summary>
        /// <param name="privateMessage">Private message</param>
        void UpdatePrivateMessage(PrivateMessage privateMessage);

        /// <summary>
        /// Deletes a forum subscription
        /// </summary>
        /// <param name="forumSubscription">Forum subscription</param>
        void DeleteSubscription(ForumSubscription forumSubscription);

        /// <summary>
        /// Gets a forum subscription
        /// </summary>
        /// <param name="forumSubscriptionId">The forum subscription identifier</param>
        /// <returns>Forum subscription</returns>
        ForumSubscription GetSubscriptionById(int forumSubscriptionId);

        /// <summary>
        /// Gets forum subscriptions
        /// </summary>
        /// <param name="customerId">The customer identifier</param>
        /// <param name="forumId">The forum identifier</param>
        /// <param name="topicId">The topic identifier</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Forum subscriptions</returns>
        IPagedList<ForumSubscription> GetAllSubscriptions(int customerId = 0, int forumId = 0,
            int topicId = 0, int pageIndex = 0, int pageSize = int.MaxValue);

        /// <summary>
        /// Inserts a forum subscription
        /// </summary>
        /// <param name="forumSubscription">Forum subscription</param>
        void InsertSubscription(ForumSubscription forumSubscription);

        /// <summary>
        /// Updates the forum subscription
        /// </summary>
        /// <param name="forumSubscription">Forum subscription</param>
        void UpdateSubscription(ForumSubscription forumSubscription);

        /// <summary>
        /// Check whether customer is allowed to create new topics
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="forum">Forum</param>
        /// <returns>True if allowed, otherwise false</returns>
        bool IsCustomerAllowedToCreateTopic(Customer customer, Forum forum);

        /// <summary>
        /// Check whether customer is allowed to edit topic
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="topic">Topic</param>
        /// <returns>True if allowed, otherwise false</returns>
        bool IsCustomerAllowedToEditTopic(Customer customer, ForumTopic topic);

        /// <summary>
        /// Check whether customer is allowed to move topic
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="topic">Topic</param>
        /// <returns>True if allowed, otherwise false</returns>
        bool IsCustomerAllowedToMoveTopic(Customer customer, ForumTopic topic);

        /// <summary>
        /// Check whether customer is allowed to delete topic
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="topic">Topic</param>
        /// <returns>True if allowed, otherwise false</returns>
        bool IsCustomerAllowedToDeleteTopic(Customer customer, ForumTopic topic);

        /// <summary>
        /// Check whether customer is allowed to create new post
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="topic">Topic</param>
        /// <returns>True if allowed, otherwise false</returns>
        bool IsCustomerAllowedToCreatePost(Customer customer, ForumTopic topic);

        /// <summary>
        /// Check whether customer is allowed to edit post
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="post">Topic</param>
        /// <returns>True if allowed, otherwise false</returns>
        bool IsCustomerAllowedToEditPost(Customer customer, ForumPost post);

        /// <summary>
        /// Check whether customer is allowed to delete post
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="post">Topic</param>
        /// <returns>True if allowed, otherwise false</returns>
        bool IsCustomerAllowedToDeletePost(Customer customer, ForumPost post);

        /// <summary>
        /// Check whether customer is allowed to set topic priority
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <returns>True if allowed, otherwise false</returns>
        bool IsCustomerAllowedToSetTopicPriority(Customer customer);

        /// <summary>
        /// Check whether customer is allowed to watch topics
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <returns>True if allowed, otherwise false</returns>
        bool IsCustomerAllowedToSubscribe(Customer customer);

        /// <summary>
        /// Calculates topic page index by post identifier
        /// </summary>
        /// <param name="forumTopicId">Topic identifier</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="postId">Post identifier</param>
        /// <returns>Page index</returns>
        int CalculateTopicPageIndex(int forumTopicId, int pageSize, int postId);

        /// <summary>
        /// Get a post vote 
        /// </summary>
        /// <param name="postId">Post identifier</param>
        /// <param name="customer">Customer</param>
        /// <returns>Post vote</returns>
        ForumPostVote GetPostVote(int postId, Customer customer);

        /// <summary>
        /// Get post vote made since the parameter date
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="сreatedFromUtc">Date</param>
        /// <returns>Post votes count</returns>
        int GetNumberOfPostVotes(Customer customer, DateTime сreatedFromUtc);

        /// <summary>
        /// Insert a post vote
        /// </summary>
        /// <param name="postVote">Post vote</param>
        void InsertPostVote(ForumPostVote postVote);

        /// <summary>
        /// Update a post vote
        /// </summary>
        /// <param name="postVote">Post vote</param>
        void UpdatePostVote(ForumPostVote postVote);

        /// <summary>
        /// Delete a post vote
        /// </summary>
        /// <param name="postVote">Post vote</param>
        void DeletePostVote(ForumPostVote postVote);

        /// <summary>
        /// Formats the forum post text
        /// </summary>
        /// <param name="forumPost">Forum post</param>
        /// <returns>Formatted text</returns>
        string FormatPostText(ForumPost forumPost);

        /// <summary>
        /// Strips the topic subject
        /// </summary>
        /// <param name="forumTopic">Forum topic</param>
        /// <returns>Formatted subject</returns>
        string StripTopicSubject(ForumTopic forumTopic);

        /// <summary>
        /// Formats the forum signature text
        /// </summary>
        /// <param name="text">Text</param>
        /// <returns>Formatted text</returns>
        string FormatForumSignatureText(string text);

        /// <summary>
        /// Formats the private message text
        /// </summary>
        /// <param name="pm">Private message</param>
        /// <returns>Formatted text</returns>
        string FormatPrivateMessageText(PrivateMessage pm);

        /// <summary>
        /// Get forum last topic
        /// </summary>
        /// <param name="forum">Forum</param>
        /// <returns>Forum topic</returns>
        ForumTopic GetLastTopic(Forum forum);

        /// <summary>
        /// Get forum last post
        /// </summary>
        /// <param name="forum">Forum</param>
        /// <returns>Forum topic</returns>
        ForumPost GetLastPost(Forum forum);

        /// <summary>
        /// Get first post
        /// </summary>
        /// <param name="forumTopic">Forum topic</param>
        /// <returns>Forum post</returns>
        ForumPost GetFirstPost(ForumTopic forumTopic);

        /// <summary>
        /// Get last post
        /// </summary>
        /// <param name="forumTopic">Forum topic</param>
        /// <returns>Forum post</returns>
        ForumPost GetLastPost(ForumTopic forumTopic);

        /// <summary>
        /// Gets ForumGroup SE (search engine) name
        /// </summary>
        /// <param name="forumGroup">ForumGroup</param>
        /// <returns>ForumGroup SE (search engine) name</returns>
        string GetForumGroupSeName(ForumGroup forumGroup);

        /// <summary>
        /// Gets Forum SE (search engine) name
        /// </summary>
        /// <param name="forum">Forum</param>
        /// <returns>Forum SE (search engine) name</returns>
        string GetForumSeName(Forum forum);

        /// <summary>
        /// Gets ForumTopic SE (search engine) name
        /// </summary>
        /// <param name="forumTopic">ForumTopic</param>
        /// <returns>ForumTopic SE (search engine) name</returns>
        string GetTopicSeName(ForumTopic forumTopic);
    }
}