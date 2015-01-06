using System.Web.Mvc;
using FluentValidation.Attributes;
using Nop.Admin.Validators.Templates;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Models.Templates
{
    [Validator(typeof(TopicTemplateValidator))]
    public partial class TopicTemplateModel : BaseNopEntityModel
    {
        [NopResourceDisplayName("Admin.System.Templates.Topic.Name")]
        [AllowHtml]
        public string Name { get; set; }

        [NopResourceDisplayName("Admin.System.Templates.Topic.ViewPath")]
        [AllowHtml]
        public string ViewPath { get; set; }

        [NopResourceDisplayName("Admin.System.Templates.Topic.DisplayOrder")]
        public int DisplayOrder { get; set; }
    }
}