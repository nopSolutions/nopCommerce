using System;
using Nop.Web.Framework.Models;

namespace Nop.Plugin.Misc.AbcCore.Areas.Admin.Models
{
    public partial record MissingImageProductModel : BaseNopEntityModel
    {
        public string ItemNumber { get; set; }
        public string Sku { get; set; }
        public string Name { get; set; }
    }
}