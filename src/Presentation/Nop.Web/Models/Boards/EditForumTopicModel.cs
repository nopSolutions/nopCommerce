using System.Collections.Generic;
#if NET451
using System.Web.Mvc;
#endif
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
#if NET451
            TopicPriorities = new List<SelectListItem>();
#endif
        }

        public bool IsEdit { get; set; }

        public int Id { get; set; }

        public int ForumId { get; set; }
        public string ForumName { get; set; }
        public string ForumSeName { get; set; }

        public int TopicTypeId { get; set; }
        public EditorType ForumEditor { get; set; }
        	
#if NET451
		[AllowHtml]
#endif
        public string Subject { get; set; }
        	
#if NET451
		[AllowHtml]
#endif
        public string Text { get; set; }
        
        public bool IsCustomerAllowedToSetTopicPriority { get; set; }
#if NET451
        public IEnumerable<SelectListItem> TopicPriorities { get; set; }
#endif

        public bool IsCustomerAllowedToSubscribe { get; set; }
        public bool Subscribed { get; set; }

    }
}