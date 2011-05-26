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
    public class SubscriptionActivationModel : BaseNopModel
    {
        public string Result { get; set; }
    }
}