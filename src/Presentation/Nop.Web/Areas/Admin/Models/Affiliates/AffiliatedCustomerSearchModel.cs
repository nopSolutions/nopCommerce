using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Affiliates
{
    /// <summary>
    /// Represents an affiliated customer search model
    /// </summary>
    public partial class AffiliatedCustomerSearchModel : BaseSearchModel
    {
        #region Properties

        public int AffliateId { get; set; }

        #endregion
    }
}