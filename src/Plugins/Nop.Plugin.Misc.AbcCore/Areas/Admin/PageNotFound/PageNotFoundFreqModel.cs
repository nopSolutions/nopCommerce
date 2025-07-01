using System;
using Nop.Web.Framework.Models;

namespace Nop.Plugin.Misc.AbcCore.Areas.Admin.PageNotFound
{
    public partial record PageNotFoundFreqModel : BaseNopEntityModel
    {
        public string Slug { get; set; }
        public int Count { get; set; }
    }
}