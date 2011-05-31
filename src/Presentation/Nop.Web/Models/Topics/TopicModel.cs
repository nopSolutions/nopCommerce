using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using FluentValidation.Attributes;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Discounts;
using Nop.Web.Framework;
using Nop.Web.Framework.Localization;
using Nop.Web.Framework.Mvc;


namespace Nop.Web.Models.Topics
{
    public class TopicModel : BaseNopEntityModel
    {
        public string SystemName { get; set; }

        public bool IncludeInSitemap { get; set; }

        public string Title { get; set; }

        public string Body { get; set; }

        public string MetaKeywords { get; set; }

        public string MetaDescription { get; set; }

        public string MetaTitle { get; set; }
    }
}