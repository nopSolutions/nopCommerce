using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Home
{
    /// <summary>
    /// Represents a dashboard model
    /// </summary>
    public partial record DashboardModel : BaseNopModel
    {
        #region Properties

        public bool IsLoggedInAsVendor { get; set; }

        #endregion
    }
}