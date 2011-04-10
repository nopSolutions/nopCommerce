using System;
using System.Collections.Generic;
using FluentValidation.Attributes;
using Nop.Admin.Validators;
using Nop.Core.Domain.Forums;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Models
{
    [Validator(typeof(ForumValidator))]
    public class ForumModel : BaseNopEntityModel
    {
        public ForumModel()
        {
            ForumGroups = new List<ForumGroup>();
        }

        [NopResourceDisplayName("Admin.ContentManagement.Forums.Forum.Fields.ForumGroupId")]
        public int ForumGroupId { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.Forums.Forum.Fields.Name")]
        public string Name { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.Forums.Forum.Fields.Description")]
        public string Description { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.Forums.Forum.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.Forums.Forum.Fields.CreatedOn")]
        public DateTime CreatedOnUtc { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.Forums.Forum.Fields.UpdatedOn")]
        public DateTime UpdatedOnUtc { get; set; }

        public List<ForumGroup> ForumGroups { get; set; }
    }
}