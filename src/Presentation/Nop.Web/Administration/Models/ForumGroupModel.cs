using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using FluentValidation.Attributes;
using Nop.Admin.Validators;
using Nop.Core.Domain.Forums;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Models
{
    [Validator(typeof(ForumGroupValidator))]
    public class ForumGroupModel : BaseNopEntityModel
    {
        public ForumGroupModel()
        {
            Forums = new List<Forum>();
        }

        [NopResourceDisplayName("Admin.ContentManagement.Forums.ForumGroup.Fields.Name")]
        public string Name { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.Forums.ForumGroup.Fields.Description")]
        public string Description { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.Forums.ForumGroup.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.Forums.ForumGroup.Fields.CreatedOn")]
        public DateTime CreatedOn { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.Forums.ForumGroup.Fields.UpdatedOn")]
        public DateTime UpdatedOn { get; set; }

        public List<Forum> Forums { get; set; }
    }
}