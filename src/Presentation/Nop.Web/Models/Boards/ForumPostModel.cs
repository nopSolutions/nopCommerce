using System;
using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Boards
{
    public partial record ForumPostModel : BaseNopModel
    {
        public int Id { get; set; }
        public int ForumTopicId { get; set; }
        public string ForumTopicSeName { get; set; }

        public string FormattedText { get; set; }

        public bool IsCurrentCustomerAllowedToEditPost { get; set; }
        public bool IsCurrentCustomerAllowedToDeletePost { get; set; }

        public int CustomerId { get; set; }
        public bool AllowViewingProfiles { get; set; }
        public string CustomerAvatarUrl { get; set; }
        public string CustomerName { get; set; }
        public bool IsCustomerForumModerator { get; set; }

        public string PostCreatedOnStr { get; set; }

        public bool ShowCustomersPostCount { get; set; }
        public int ForumPostCount { get; set; }

        public bool ShowCustomersJoinDate { get; set; }
        public DateTime CustomerJoinDate { get; set; }

        public bool ShowCustomersLocation { get; set; }
        public string CustomerLocation { get; set; }

        public bool AllowPrivateMessages { get; set; }

        public bool SignaturesEnabled { get; set; }
        public string FormattedSignature { get; set; }

        public int CurrentTopicPage { get; set; }

        public bool AllowPostVoting { get; set; }
        public int VoteCount { get; set; }
        public bool? VoteIsUp { get; set; }
    }
}