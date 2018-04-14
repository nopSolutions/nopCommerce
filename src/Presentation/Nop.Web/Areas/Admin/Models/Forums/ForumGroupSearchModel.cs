using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Forums
{
    /// <summary>
    /// Represents a forum group search model
    /// </summary>
    public partial class ForumGroupSearchModel : BaseSearchModel
    {
        #region Ctor

        public ForumGroupSearchModel()
        {
            this.ForumSearch = new ForumSearchModel();
        }

        #endregion

        #region Properties

        public ForumSearchModel ForumSearch { get; set; }

        #endregion
    }
}