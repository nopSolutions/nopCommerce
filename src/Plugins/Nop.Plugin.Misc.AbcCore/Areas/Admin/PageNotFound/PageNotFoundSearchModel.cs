using Nop.Web.Framework.Models;

namespace Nop.Plugin.Misc.AbcCore.Areas.Admin.PageNotFound
{
    public partial record PageNotFoundSearchModel : BaseSearchModel
    {
        public string Slug { get; set; }
        public string CustomerEmail { get; set; }
    }
}