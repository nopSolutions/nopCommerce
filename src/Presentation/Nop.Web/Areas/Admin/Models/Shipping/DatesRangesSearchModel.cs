using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Shipping
{
    /// <summary>
    /// Represents a dates and ranges search model
    /// </summary>
    public partial class DatesRangesSearchModel : BaseSearchModel
    {
        #region Ctor

        public DatesRangesSearchModel()
        {
            this.DeliveryDateSearchModel = new DeliveryDateSearchModel();
            this.ProductAvailabilityRangeSearchModel = new ProductAvailabilityRangeSearchModel();
        }

        #endregion

        #region Properties

        public DeliveryDateSearchModel DeliveryDateSearchModel { get; set; }

        public ProductAvailabilityRangeSearchModel ProductAvailabilityRangeSearchModel { get; set; }

        #endregion
    }
}