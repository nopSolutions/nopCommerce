using System.Collections.Generic;
using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Weixin
{
    /// <summary>
    /// Represents a AddUserRelatedModel
    /// </summary>
    public partial class AddUserRelatedModel : BaseNopModel
    {
        #region Ctor

        public AddUserRelatedModel()
        {
            SelectedUserIds = new List<int>();
        }
        #endregion

        #region Properties

        public int RelatedId { get; set; }

        public IList<int> SelectedUserIds { get; set; }

        #endregion
    }
}