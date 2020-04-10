using Nop.Web.Framework.Models;

namespace Nop.Plugin.Payments.PayPalSmartPaymentButtons.Models
{
    /// <summary>
    /// Represents a payment info model
    /// </summary>
    public class PaymentInfoModel : BaseNopModel
    {
        #region Properties

        public string OrderId { get; set; }

        public string Errors { get; set; }

        #endregion
    }
}