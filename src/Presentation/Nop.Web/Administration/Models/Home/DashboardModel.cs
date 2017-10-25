using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Models.Home
{
    /// <summary>
    /// 仪表盘视图模型
    /// </summary>
    public partial class DashboardModel : BaseNopModel
    {
        /// <summary>
        /// 是否是供应商角色
        /// </summary>
        public bool IsLoggedInAsVendor { get; set; }
    }
}