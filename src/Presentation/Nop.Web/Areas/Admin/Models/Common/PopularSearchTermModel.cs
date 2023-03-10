using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Common
{
    /// <summary>
    /// Represents a popular search term model
    /// </summary>
    public partial record PopularSearchTermModel : BaseNopModel
    {
        #region Properties

        [NopResourceDisplayName("Admin.SearchTermReport.Keyword")]
        public string Keyword { get; set; }

        [NopResourceDisplayName("Admin.SearchTermReport.Count")]
        public int Count { get; set; }

        #endregion
    }
}
