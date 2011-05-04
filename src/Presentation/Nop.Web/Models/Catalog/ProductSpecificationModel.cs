using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nop.Web.Framework.Mvc;
using Nop.Web.Models.Media;

namespace Nop.Web.Models.Catalog
{
    public class ProductSpecificationModel : BaseNopModel
    {
        public string SpecificationAttributeName { get; set; }

        public string SpecificationAttributeOption { get; set; }
    }
}