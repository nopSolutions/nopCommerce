using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Home
{
    /// <summary>
    /// Represents a dashboard model
    /// </summary>
    public partial class DashboardModel : BaseNopModel
    {
        #region Ctor

        public DashboardModel()
        {
            this.PopularSearchTermSearchModel = new PopularSearchTermSearchModel();
        }

        #endregion

        #region Properties

        public bool IsLoggedInAsVendor { get; set; }

        public PopularSearchTermSearchModel PopularSearchTermSearchModel { get; set; }

        #endregion
    }
}