using System.Web.Mvc;
using FluentValidation.Attributes;
using Nop.Core.Domain.Forums;
using Nop.Web.Validators.Boards;

namespace Nop.Web.Models.Boards
{
    [Validator(typeof(ForumPostValidator))]
    public class ForumPostModel
    {
        public int Id { get; set; }

        public int ForumTopicId { get; set; }

        [AllowHtml]
        public string Text { get; set; }

        public string ForumName { get; set; }

        public bool Subscribed { get; set; }

        public ForumBreadcrumbModel ForumBreadcrumbModel { get; set; }

        public string PostError { get; set; }

        public bool IsCustomerAllowedToSubscribe { get; set; }

        public EditorType ForumEditor { get; set; }

        public ForumPost ForumPost { get; set; }

        public ForumTopic ForumTopic { get; set; }

        public bool IsCustomerAllowedToEditPost { get; set; }

        public bool IsCustomerAllowedToDeletePost { get; set; }

        public CommonTopicSettingsModel CommonTopicSettingsModel { get; set; }

        public string AvatarUrl { get; set; }

        public string Signature { get; set; }

        public string Location { get; set; }
    }

    public class CommonTopicSettingsModel
    {
        public bool ShowCustomersPostCount { get; set; }

        public bool AllowPrivateMessages { get; set; }

        public bool RelativeDateTimeFormattingEnabled { get; set; }

        public bool SignaturesEnabled { get; set; }

        public bool AllowViewingProfiles { get; set; }

        public bool ShowCustomersJoinDate { get; set; }

        public bool ShowCustomersLocation { get; set; }

        public EditorType ForumEditor { get; set; }
                
        public bool AllowCustomersToUploadAvatars { get; set; }

        public bool DefaultAvatarEnabled { get; set; }

        public string DefaultAvatarUrl { get; set; }        
    }

}