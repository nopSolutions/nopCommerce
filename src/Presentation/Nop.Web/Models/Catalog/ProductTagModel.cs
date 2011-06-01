using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nop.Web.Framework.Mvc;

namespace Nop.Web.Models.Catalog
{
    public class ProductTagModel : BaseNopEntityModel
    {
        public string Name { get; set; }

        public int ProductCount { get; set; }
    }
}