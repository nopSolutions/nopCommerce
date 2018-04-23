using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Payments
{
    /// <summary>
    /// Represents a payment methods model
    /// </summary>
    public partial class PaymentMethodsModel : BaseNopModel
    {
        #region Ctor

        public PaymentMethodsModel()
        {
            PaymentsMethod = new PaymentMethodSearchModel();
            PaymentMethodRestriction = new PaymentMethodRestrictionModel();
        }

        #endregion

        #region Properties

        public PaymentMethodSearchModel PaymentsMethod { get; set; }

        public PaymentMethodRestrictionModel PaymentMethodRestriction { get; set; }

        #endregion
    }
}