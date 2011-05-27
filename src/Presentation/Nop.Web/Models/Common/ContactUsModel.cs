using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FluentValidation.Attributes;
using Nop.Core.Domain.Catalog;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using Nop.Web.Models.Media;
using Nop.Web.Validators.Common;

namespace Nop.Web.Models.Common
{
    [Validator(typeof(ContactUsValidator))]
    public class ContactUsModel : BaseNopModel
    {
        [AllowHtml]
        [NopResourceDisplayName("ContactUs.Email")]
        public string Email { get; set; }

        [AllowHtml]
        [NopResourceDisplayName("ContactUs.Enquiry")]
        public string Enquiry { get; set; }

        [AllowHtml]
        [NopResourceDisplayName("ContactUs.FullName")]
        public string FullName { get; set; }

        public bool SuccessfullySent { get; set; }
        public string Result { get; set; }
    }
}