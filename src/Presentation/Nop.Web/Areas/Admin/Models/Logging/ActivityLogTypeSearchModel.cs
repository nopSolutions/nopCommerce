using System.Collections.Generic;
using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Logging
{
    /// <summary>
    /// Represents an activity log type search model
    /// </summary>
    public partial class ActivityLogTypeSearchModel : BaseSearchModel
    {
        #region Properties       

        public IList<ActivityLogTypeModel> ActivityLogTypeListModel { get; set; }

        #endregion
    }
}