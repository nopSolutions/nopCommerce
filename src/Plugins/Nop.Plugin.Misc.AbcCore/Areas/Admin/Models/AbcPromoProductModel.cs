using System;
using Nop.Web.Framework.Models;

namespace Nop.Plugin.Misc.AbcCore.Areas.Admin.Models
{
    public partial record AbcPromoProductModel : BaseNopEntityModel
    {
        public string AbcItemNumber { get; set; }
        public string Name { get; set; }
        public bool Published { get; set; }
        public bool IsABC { get; set; }
        public bool IsHawthorne { get; set; }
        public string Brand { get; set; }
    }
}