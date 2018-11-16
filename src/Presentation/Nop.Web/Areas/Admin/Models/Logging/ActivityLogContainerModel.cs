using Nop.Web.Framework.Models;
using System.Collections.Generic;

namespace Nop.Web.Areas.Admin.Models.Logging
{
    /// <summary>
    /// Represents an activity log container model
    /// </summary>
    public partial class ActivityLogContainerModel : BaseNopModel
    {
        #region Ctor

        public ActivityLogContainerModel()
        {
            ListLogs = new ActivityLogSearchModel();
            ListTypes = new ActivityLogTypeSearchModel();
        }

        #endregion

        #region Properties

        public ActivityLogSearchModel ListLogs { get; set; }

        public ActivityLogTypeSearchModel ListTypes { get; set; }

        #endregion
    }
}
