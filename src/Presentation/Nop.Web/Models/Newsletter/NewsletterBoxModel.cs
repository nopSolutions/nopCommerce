using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FluentValidation.Attributes;
using Nop.Core.Domain.Catalog;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using Nop.Web.Validators.Newsletter;

namespace Nop.Web.Models.Newsletter
{
    [Validator(typeof(NewsletterBoxValidator))]
    public class NewsletterBoxModel : BaseNopModel
    {
        public string Email { get; set; }

        public bool NewsletterBoxEnabled { get; set; }
    }
}