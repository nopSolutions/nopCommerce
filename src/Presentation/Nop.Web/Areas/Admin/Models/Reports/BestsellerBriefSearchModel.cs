using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Reports
{
    /// <summary>
    /// Represents a bestseller brief search model
    /// </summary>
    public partial class BestsellerBriefSearchModel : BaseSearchModel
    {
        #region Properties

        //keep it synchronized to OrderReportService class, BestSellersReport() method, orderBy parameter
        //TODO: move from int to enum
        public int OrderBy { get; set; }

        #endregion
    }
}