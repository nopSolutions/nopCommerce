using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Orders
{
    /// <summary>
    /// Represents a checkout attribute value search model
    /// </summary>
    public partial record CheckoutAttributeValueSearchModel : BaseSearchModel
    {
        #region Properties

        public int CheckoutAttributeId { get; set; }

        #endregion
    }
}