//------------------------------------------------------------------------------
// The contents of this file are subject to the nopCommerce Public License Version 1.0 ("License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at  http://www.nopCommerce.com/License.aspx. 
// 
// Software distributed under the License is distributed on an "AS IS" basis, WITHOUT WARRANTY OF ANY KIND, either express or implied. 
// See the License for the specific language governing rights and limitations under the License.
// 
// The Original Code is nopCommerce.
// The Initial Developer of the Original Code is NopSolutions.
// All Rights Reserved.
// 
// Contributor(s): _______. 
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Data.Objects;
using System.Data.Objects.DataClasses;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using NopSolutions.NopCommerce.BusinessLogic.Caching;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;
using NopSolutions.NopCommerce.BusinessLogic.Data;
using NopSolutions.NopCommerce.BusinessLogic.Messages;
using NopSolutions.NopCommerce.BusinessLogic.Profile;
using NopSolutions.NopCommerce.BusinessLogic.Utils.Html;
using NopSolutions.NopCommerce.Common;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.Common.Utils.Html;
using NopSolutions.NopCommerce.BusinessLogic.Infrastructure;

namespace NopSolutions.NopCommerce.BusinessLogic.Content.Forums
{
    /// <summary>
    /// Forum service interface
    /// </summary>
    public partial interface IForumService
    {
        /// <summary>
        /// Deletes a forum group
        /// </summary>
        /// <param name="forumGroupId">The forum group identifier</param>
        void DeleteForumGroup(int forumGroupId);

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
        List<ForumGroup> GetAllForumGroups();

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
        /// <param name="forumId">The forum identifier</param>
        void DeleteForum(int forumId);

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
        List<Forum> GetAllForumsByGroupId(int forumGroupId);

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
        /// Deletes a topic
        /// </summary>
        /// <param name="forumTopicId">The topic identifier</param>
        void DeleteTopic(int forumTopicId);

        /// <summary>
        /// Gets a topic
        /// </summary>
        /// <param name="forumTopicId">The topic identifier</param>
        /// <returns>Topic</returns>
        ForumTopic GetTopicById(int forumTopicId);

        /// <summary>
        /// Gets a topic
        /// </summary>
        /// <param name="forumTopicId">The topic identifier</param>
        /// <param name="increaseViews">The value indicating whether to increase topic views</param>
        /// <returns>Topic</returns>
        ForumTopic GetTopicById(int forumTopicId, bool increaseViews);

        /// <summary>
        /// Gets all topics
        /// </summary>
        /// <param name="forumId">The forum group identifier</param>
        /// <param name="userId">The user identifier</param>
        /// <param name="keywords">Keywords</param>
        /// <param name="searchType">Search type</param>
        /// <param name="limitDays">Limit by the last number days; 0 to load all topics</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Topics</returns>
        PagedList<ForumTopic> GetAllTopics(int forumId,
            int userId, string keywords, ForumSearchTypeEnum searchType,
            int limitDays, int pageIndex, int pageSize);
        
        /// <summary>
        /// Gets active topics
        /// </summary>
        /// <param name="forumId">The forum group identifier</param>
        /// <param name="topicCount">Topic count</param>
        /// <returns>Topics</returns>
        List<ForumTopic> GetActiveTopics(int forumId, int topicCount);
        
        /// <summary>
        /// Inserts a topic
        /// </summary>
        /// <param name="forumTopic">Forum topic</param>
        /// <param name="sendNotifications">A value indicating whether to send notifications to users</param>
        void InsertTopic(ForumTopic forumTopic, bool sendNotifications);

        /// <summary>
        /// Updates the topic
        /// </summary>
        /// <param name="forumTopic">Forum topic</param>
        void UpdateTopic(ForumTopic forumTopic);

        /// <summary>
        /// Moves the topic
        /// </summary>
        /// <param name="forumTopicId">The forum topic identifier</param>
        /// <param name="newForumId">New forum identifier</param>
        /// <returns>Moved topic</returns>
        ForumTopic MoveTopic(int forumTopicId, int newForumId);

        /// <summary>
        /// Deletes a post
        /// </summary>
        /// <param name="forumPostId">The post identifier</param>
        void DeletePost(int forumPostId);

        /// <summary>
        /// Gets a post
        /// </summary>
        /// <param name="forumPostId">The post identifier</param>
        /// <returns>Post</returns>
        ForumPost GetPostById(int forumPostId);

        /// <summary>
        /// Gets all posts
        /// </summary>
        /// <param name="forumTopicId">The forum topic identifier</param>
        /// <param name="userId">The user identifier</param>
        /// <param name="keywords">Keywords</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Posts</returns>
        PagedList<ForumPost> GetAllPosts(int forumTopicId,
            int userId, string keywords, int pageIndex, int pageSize);

        /// <summary>
        /// Gets all posts
        /// </summary>
        /// <param name="forumTopicId">The forum topic identifier</param>
        /// <param name="userId">The user identifier</param>
        /// <param name="keywords">Keywords</param>
        /// <param name="ascSort">Sort order</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Posts</returns>
        PagedList<ForumPost> GetAllPosts(int forumTopicId, int userId,
            string keywords, bool ascSort,  int pageIndex, int pageSize);

        /// <summary>
        /// Inserts a post
        /// </summary>
        /// <param name="forumPost">The forum post</param>
        /// <param name="sendNotifications">A value indicating whether to send notifications to users</param>
        void InsertPost(ForumPost forumPost, bool sendNotifications);

        /// <summary>
        /// Updates the post
        /// </summary>
        /// <param name="forumPost">The forum post</param>
        void UpdatePost(ForumPost forumPost);

        /// <summary>
        /// Deletes a private message
        /// </summary>
        /// <param name="forumPrivateMessageId">The private message identifier</param>
        void DeletePrivateMessage(int forumPrivateMessageId);

        /// <summary>
        /// Gets a private message
        /// </summary>
        /// <param name="forumPrivateMessageId">The private message identifier</param>
        /// <returns>Private message</returns>
        PrivateMessage GetPrivateMessageById(int forumPrivateMessageId);

        /// <summary>
        /// Gets private messages
        /// </summary>
        /// <param name="fromUserId">The user identifier who sent the message</param>
        /// <param name="toUserId">The user identifier who should receive the message</param>
        /// <param name="isRead">A value indicating whether loaded messages are read. false - to load not read messages only, 1 to load read messages only, null to load all messages</param>
        /// <param name="isDeletedByAuthor">A value indicating whether loaded messages are deleted by author. false - messages are not deleted by author, null to load all messages</param>
        /// <param name="isDeletedByRecipient">A value indicating whether loaded messages are deleted by recipient. false - messages are not deleted by recipient, null to load all messages</param>
        /// <param name="keywords">Keywords</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Private messages</returns>
        PagedList<PrivateMessage> GetAllPrivateMessages(int fromUserId,
            int toUserId, bool? isRead, bool? isDeletedByAuthor, bool? isDeletedByRecipient,
            string keywords, int pageIndex, int pageSize);

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
        /// <param name="forumSubscriptionId">The forum subscription identifier</param>
        void DeleteSubscription(int forumSubscriptionId);

        /// <summary>
        /// Gets a forum subscription
        /// </summary>
        /// <param name="forumSubscriptionId">The forum subscription identifier</param>
        /// <returns>Forum subscription</returns>
        ForumSubscription GetSubscriptionById(int forumSubscriptionId);

        /// <summary>
        /// Gets forum subscriptions
        /// </summary>
        /// <param name="userId">The user identifier</param>
        /// <param name="forumId">The forum identifier</param>
        /// <param name="topicId">The topic identifier</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Forum subscriptions</returns>
        PagedList<ForumSubscription> GetAllSubscriptions(int userId, int forumId,
            int topicId, int pageIndex, int pageSize);

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
        /// Check whether user is allowed to create new topics
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="forum">Forum</param>
        /// <returns>True if allowed, otherwise false</returns>
        bool IsUserAllowedToCreateTopic(Customer customer, Forum forum);

        /// <summary>
        /// Check whether user is allowed to edit topic
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="topic">Topic</param>
        /// <returns>True if allowed, otherwise false</returns>
        bool IsUserAllowedToEditTopic(Customer customer, ForumTopic topic);

        /// <summary>
        /// Check whether user is allowed to move topic
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="topic">Topic</param>
        /// <returns>True if allowed, otherwise false</returns>
        bool IsUserAllowedToMoveTopic(Customer customer, ForumTopic topic);

        /// <summary>
        /// Check whether user is allowed to delete topic
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="topic">Topic</param>
        /// <returns>True if allowed, otherwise false</returns>
        bool IsUserAllowedToDeleteTopic(Customer customer, ForumTopic topic);

        /// <summary>
        /// Check whether user is allowed to create new post
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="topic">Topic</param>
        /// <returns>True if allowed, otherwise false</returns>
        bool IsUserAllowedToCreatePost(Customer customer, ForumTopic topic);

        /// <summary>
        /// Check whether user is allowed to edit post
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="post">Topic</param>
        /// <returns>True if allowed, otherwise false</returns>
        bool IsUserAllowedToEditPost(Customer customer, ForumPost post);

        /// <summary>
        /// Check whether user is allowed to delete post
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="post">Topic</param>
        /// <returns>True if allowed, otherwise false</returns>
        bool IsUserAllowedToDeletePost(Customer customer, ForumPost post);

        /// <summary>
        /// Check whether user is allowed to set topic priority
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <returns>True if allowed, otherwise false</returns>
        bool IsUserAllowedToSetTopicPriority(Customer customer);

        /// <summary>
        /// Check whether user is allowed to watch topics
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <returns>True if allowed, otherwise false</returns>
        bool IsUserAllowedToSubscribe(Customer customer);

        /// <summary>
        /// Calculates topic page index by post identifier
        /// </summary>
        /// <param name="forumTopicId">Topic identifier</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="postId">Post identifier</param>
        /// <returns>Page index</returns>
        int CalculateTopicPageIndex(int forumTopicId, int pageSize, int postId);

        /// <summary>
        /// Gets or sets a value indicating whether forums are enabled
        /// </summary>
        bool ForumsEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether relative date and time formatting is enabled  (e.g. 2 hours ago, a month ago)
        /// </summary>
        bool RelativeDateTimeFormattingEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether customers are allowed to edit posts that they created.
        /// </summary>
        bool AllowCustomersToEditPosts { get; set; }
        
        /// <summary>
        /// Gets or sets a value indicating whether customers are allowed to manage theirs subscriptions
        /// </summary>
        bool AllowCustomersToManageSubscriptions { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the guests are allowed to create posts.
        /// </summary>
        bool AllowGuestsToCreatePosts { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the guests are allowed to create topics.
        /// </summary>
        bool AllowGuestsToCreateTopics { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether customers are allowed to delete posts that they created.
        /// </summary>
        bool AllowCustomersToDeletePosts { get; set; }

        /// <summary>
        /// Gets or sets maximum length of topic subject
        /// </summary>
        int TopicSubjectMaxLength { get; set; }

        /// <summary>
        /// Gets or sets maximum length of post
        /// </summary>
        int PostMaxLength { get; set; }

        /// <summary>
        /// Gets or sets the page size for topics in forums
        /// </summary>
        int TopicsPageSize { get; set; }

        /// <summary>
        /// Gets or sets the page size for posts in topics
        /// </summary>
        int PostsPageSize { get; set; }

        /// <summary>
        /// Gets or sets the page size for search result
        /// </summary>
        int SearchResultsPageSize { get; set; }

        /// <summary>
        /// Gets or sets the page size for latest user post
        /// </summary>
        int LatestUserPostsPageSize { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to show customers forum post count
        /// </summary>
        bool ShowCustomersPostCount { get; set; }

        /// <summary>
        /// Gets or sets a forum editor type
        /// </summary>
        EditorTypeEnum ForumEditor { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether customers are allowed to specify signature.
        /// </summary>
        bool SignaturesEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether private messages are allowed
        /// </summary>
        bool AllowPrivateMessages { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether a customer should be notified about new private messages
        /// </summary>
        bool NotifyAboutPrivateMessages { get; set; }

        /// <summary>
        /// Gets or sets maximum length of pm subject
        /// </summary>
        int PMSubjectMaxLength { get; set; }

        /// <summary>
        /// Gets or sets maximum length of pm message
        /// </summary>
        int PMTextMaxLength { get; set; }
    }
}
