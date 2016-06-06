using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Models.Orders
{
    public partial class BestsellersBriefReportByAmountModel : BaseNopModel
    {
        public bool HidePanel { get; set; }

        public string HidePanelSettingName { get; set; }
    }
}