using Nop.Web.Framework.Models;

namespace Nop.Plugin.Payments.PayPalCommerce.Models
{
    /// <summary>
    /// Represents a payment info model
    /// </summary>
    public record PaymentInfoModel : BaseNopModel
    {
        #region Properties

        public string OrderId { get; set; }

        public string OrderTotal { get; set; }

        public string Errors { get; set; }

        #endregion
    }
}