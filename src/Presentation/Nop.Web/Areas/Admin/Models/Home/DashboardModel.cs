using Nop.Web.Areas.Admin.Models.Common;
using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Home
{
    public partial class DashboardModel : BaseNopModel
    {
        public DashboardModel()
        {
            this.PopularSearchTermSearchModel = new PopularSearchTermSearchModel();
        }

        public bool IsLoggedInAsVendor { get; set; }

        public PopularSearchTermSearchModel PopularSearchTermSearchModel { get; set; }
    }
}