using System;
using Nop.Web.Framework.Models;

namespace Nop.Plugin.Misc.AbcCore.Areas.Admin.Models
{
    public partial record AbcPromoProductSearchModel : BaseSearchModel
    {
        public int AbcPromoId { get; set; }
        public string AbcPromoName { get; set; }
    }
}