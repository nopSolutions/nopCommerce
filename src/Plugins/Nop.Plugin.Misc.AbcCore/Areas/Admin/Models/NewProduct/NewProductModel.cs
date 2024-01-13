using System;
using Nop.Web.Framework.Models;

namespace Nop.Plugin.Misc.AbcCore.Areas.Admin.Models
{
    public partial record NewProductModel : BaseNopEntityModel
    {
        public string ItemNo { get; set; }
        public string Sku { get; set; }
        public string Name { get; set; }
        public string WebEnabledDate { get; set; }
    }
}