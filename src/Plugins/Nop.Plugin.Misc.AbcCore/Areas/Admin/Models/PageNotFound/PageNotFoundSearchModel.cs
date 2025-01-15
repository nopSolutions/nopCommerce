using Nop.Web.Framework.Models;

namespace Nop.Plugin.Misc.AbcCore.Areas.Admin.Models
{
    public partial record PageNotFoundSearchModel : BaseSearchModel
    {
        public string Slug { get; set; }
    }
}