using FluentValidation.Attributes;
using Nop.Web.Framework.Mvc;
using Nop.Web.Validators.Newsletter;

namespace Nop.Web.Models.Newsletter
{
    [Validator(typeof(NewsletterBoxValidator))]
    public class NewsletterBoxModel : BaseNopModel
    {
        public string Email { get; set; }
    }
}