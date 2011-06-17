using System.Collections.Generic;
using System.Web.Mvc;
using FluentValidation.Attributes;
using Nop.Core.Domain.Forums;
using Nop.Web.Validators.Boards;

namespace Nop.Web.Models.Boards
{
    [Validator(typeof(ForumTopicValidator))]
    public class ForumTopicModel
    {
        public ForumTopicModel()
        {
            TopicPriorities = new List<SelectListItem>();
        }

        public int Id { get; set; }

        public int ForumId { get; set; }

        public int TopicTypeId { get; set; }

        [AllowHtml]
        public string Subject { get; set; }

        [AllowHtml]
        public string Text { get; set; }

        public Forum Forum { get; set; }

        public bool Subscribed { get; set; }

        public IEnumerable<SelectListItem> TopicPriorities { get; set; }

        public ForumBreadcrumbModel ForumBreadcrumbModel { get; set; }

        public string PostError { get; set; }

        public bool IsCustomerAllowedToSetTopicPriority { get; set; }

        public bool IsCustomerAllowedToSubscribe { get; set; }

        public EditorType ForumEditor { get; set; }
    }
}