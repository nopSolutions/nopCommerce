using System.Collections.Generic;
using System.Web.Mvc;
using FluentValidation.Attributes;
using Nop.Core.Domain.Forums;
using Nop.Web.Validators.Boards;

namespace Nop.Web.Models.Boards
{
    [Validator(typeof(EditForumTopicValidator))]
    public partial class EditForumTopicModel
    {
        public EditForumTopicModel()
        {
            TopicPriorities = new List<SelectListItem>();
        }

        public bool IsEdit { get; set; }

        public int Id { get; set; }

        public int ForumId { get; set; }
        public string ForumName { get; set; }
        public string ForumSeName { get; set; }

        public int TopicTypeId { get; set; }
        public EditorType ForumEditor { get; set; }
        [AllowHtml]
        public string Subject { get; set; }
        [AllowHtml]
        public string Text { get; set; }
        
        public bool IsCustomerAllowedToSetTopicPriority { get; set; }
        public IEnumerable<SelectListItem> TopicPriorities { get; set; }

        public bool IsCustomerAllowedToSubscribe { get; set; }
        public bool Subscribed { get; set; }

    }
}