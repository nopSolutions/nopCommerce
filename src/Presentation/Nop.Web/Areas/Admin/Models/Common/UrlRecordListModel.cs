using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Mvc.Models;

namespace Nop.Web.Areas.Admin.Models.Common
{
    public partial class UrlRecordListModel : BaseNopModel
    {
        [NopResourceDisplayName("Admin.System.SeNames.Name")]
        public string SeName { get; set; }
    }
}