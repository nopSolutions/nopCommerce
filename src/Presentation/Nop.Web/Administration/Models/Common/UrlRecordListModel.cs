using System.Web.Mvc;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Models.Common
{
    public partial class UrlRecordListModel : BaseNopModel
    {
        [NopResourceDisplayName("Admin.System.SeNames.Name")]
        [AllowHtml]
        public string SeName { get; set; }
    }
}