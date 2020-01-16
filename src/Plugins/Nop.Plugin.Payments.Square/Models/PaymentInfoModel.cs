using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Payments.Square.Models
{
    /// <summary>
    /// Represents payment model
    /// </summary>
    public class PaymentInfoModel : BaseNopModel
    {
        #region Ctor

        public PaymentInfoModel()
        {
            StoredCards = new List<SelectListItem>();
        }

        #endregion

        #region Properties

        public bool IsGuest { get; set; }

        public string CardNonce { get; set; }

        public string Token { get; set; }

        public string Errors { get; set; }

        public decimal OrderTotal { get; set; }

        public string Currency { get; set; }

        public string BillingFirstName { get; set; }

        public string BillingLastName { get; set; }

        public string BillingEmail { get; set; }

        public string BillingPostalCode { get; set; }

        public string BillingCountry { get; set; }

        public string BillingState { get; set; }

        public string BillingCity { get; set; }

        [NopResourceDisplayName("Plugins.Payments.Square.Fields.PostalCode")]
        public string PostalCode { get; set; }

        [NopResourceDisplayName("Plugins.Payments.Square.Fields.SaveCard")]
        public bool SaveCard { get; set; }

        [NopResourceDisplayName("Plugins.Payments.Square.Fields.StoredCard")]
        public string StoredCardId { get; set; }
        public IList<SelectListItem> StoredCards { get; set; }

        #endregion
    }
}