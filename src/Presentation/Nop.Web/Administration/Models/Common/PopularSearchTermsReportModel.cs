using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Models.Common
{
    public partial class PopularSearchTermsReportModel : BaseNopModel
    {
        public bool HidePanel { get; set; }

        public string HidePanelSettingName { get; set; }
    }
}