using System.Web.Mvc;
using FluentValidation.Attributes;
using Nop.Core.Domain.Forums;
using Nop.Web.Validators.Boards;

namespace Nop.Web.Models.Boards
{
    [Validator(typeof(EditForumPostValidator))]
    public partial class EditForumPostModel
    {
        public int Id { get; set; }
        public int ForumTopicId { get; set; }

        public bool IsEdit { get; set; }

        [AllowHtml]
        public string Text { get; set; }
        public EditorType ForumEditor { get; set; }

        public string ForumName { get; set; }
        public string ForumTopicSubject { get; set; }
        public string ForumTopicSeName { get; set; }

        public bool IsCustomerAllowedToSubscribe { get; set; }
        public bool Subscribed { get; set; }
    }
}