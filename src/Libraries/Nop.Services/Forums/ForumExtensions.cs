using System;
using System.Linq;
using Nop.Core.Domain.Forums;
using Nop.Core.Html;
using Nop.Core.Infrastructure;

namespace Nop.Services.Forums
{
    public static class ForumExtensions
    {
        /// <summary>
        /// Formats the forum post text
        /// </summary>
        /// <param name="forumPost">Forum post</param>
        /// <returns>Formatted text</returns>
        public static string FormatPostText(this ForumPost forumPost)
        {
            string text = forumPost.Text;

            if (String.IsNullOrEmpty(text))
                return string.Empty;

            switch (EngineContext.Current.Resolve<ForumSettings>().ForumEditor)
            {
                case EditorType.SimpleTextBox:
                    {
                        text = HtmlHelper.FormatText(text, false, true, false, false, false, false);
                    }
                    break;
                case EditorType.BBCodeEditor:
                    {
                        text = HtmlHelper.FormatText(text, false, true, false, true, false, false);
                    }
                    break;
                default:
                    break;
            }

            return text;
        }

        /// <summary>
        /// Strips the topic subject
        /// </summary>
        /// <param name="forumTopic">Forum topic</param>
        /// <returns>Formatted subject</returns>
        public static string StripTopicSubject(this ForumTopic forumTopic)
        {
            string subject = forumTopic.Subject;
            if (String.IsNullOrEmpty(subject))
            {
                return subject;
            }

            int strippedTopicMaxLength = EngineContext.Current.Resolve<ForumSettings>().StrippedTopicMaxLength;
            if (strippedTopicMaxLength > 0)
            {
                if (subject.Length > strippedTopicMaxLength)
                {
                    int index = subject.IndexOf(" ", strippedTopicMaxLength);
                    if (index > 0)
                    {
                        subject = subject.Substring(0, index);
                        subject += "...";
                    }
                }
            }

            return subject;
        }

        /// <summary>
        /// Formats the forum signature text
        /// </summary>
        /// <param name="text">Text</param>
        /// <returns>Formatted text</returns>
        public static string FormatForumSignatureText(this string text)
        {
            if (String.IsNullOrEmpty(text))
                return string.Empty;

            text = HtmlHelper.FormatText(text, false, true, false, false, false, false);
            return text;
        }

        /// <summary>
        /// Formats the private message text
        /// </summary>
        /// <param name="pm">Private message</param>
        /// <returns>Formatted text</returns>
        public static string FormatPrivateMessageText(this PrivateMessage pm)
        {
            string text = pm.Text;

            if (String.IsNullOrEmpty(text))
                return string.Empty;

            text = HtmlHelper.FormatText(text, false, true, false, true, false, false);

            return text;
        }
        
        /// <summary>
        /// Get forum last topic
        /// </summary>
        /// <param name="forum">Forum</param>
        /// <param name="forumService">Forum service</param>
        /// <returns>Forum topic</returns>
        public static ForumTopic GetLastTopic(this Forum forum, IForumService forumService)
        {
            if (forum == null)
                throw new ArgumentNullException("forum");

            return forumService.GetTopicById(forum.LastTopicId);
        }

        /// <summary>
        /// Get forum last post
        /// </summary>
        /// <param name="forum">Forum</param>
        /// <param name="forumService">Forum service</param>
        /// <returns>Forum topic</returns>
        public static ForumPost GetLastPost(this Forum forum, IForumService forumService)
        {
            if (forum == null)
                throw new ArgumentNullException("forum");

            return forumService.GetPostById(forum.LastPostId);
        }

        /// <summary>
        /// Get first post
        /// </summary>
        /// <param name="forumTopic">Forum topic</param>
        /// <param name="forumService">Forum service</param>
        /// <returns>Forum post</returns>
        public static ForumPost GetFirstPost(this ForumTopic forumTopic, IForumService forumService)
        {
            if (forumTopic == null)
                throw new ArgumentNullException("forumTopic");

            var forumPosts = forumService.GetAllPosts(forumTopic.Id, 0, string.Empty, 0, 1);
            if (forumPosts.Any())
                return forumPosts[0];

            return null;
        }

        /// <summary>
        /// Get last post
        /// </summary>
        /// <param name="forumTopic">Forum topic</param>
        /// <param name="forumService">Forum service</param>
        /// <returns>Forum post</returns>
        public static ForumPost GetLastPost(this ForumTopic forumTopic, IForumService forumService)
        {
            if (forumTopic == null)
                throw new ArgumentNullException("forumTopic");

            return forumService.GetPostById(forumTopic.LastPostId);
        }
    }
}
