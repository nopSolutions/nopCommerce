using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Mvc.Models;

namespace Nop.Plugin.Payments.Worldpay.Models
{
    /// <summary>
    /// Represents the Worldpay payment model
    /// </summary>
    public class PaymentInfoModel : BaseNopModel
    {
        #region Ctor

        public PaymentInfoModel()
        {
            ExpireMonths = new List<SelectListItem>();
            ExpireYears = new List<SelectListItem>();
            CardTypes = new List<SelectListItem>();
            StoredCards = new List<SelectListItem>();
        }

        #endregion

        #region Properties

        public bool IsGuest { get; set; }

        public string CardNumber { get; set; }

        public string CardCode { get; set; }

        public string ExpireMonth { get; set; }
        public IList<SelectListItem> ExpireMonths { get; set; }

        public string ExpireYear { get; set; }
        public IList<SelectListItem> ExpireYears { get; set; }

        public string CardType { get; set; }
        public IList<SelectListItem> CardTypes { get; set; }

        public string Errors { get; set; }

        public string Token { get; set; }

        public bool SaveCard { get; set; }

        public string StoredCardId { get; set; }
        public IList<SelectListItem> StoredCards { get; set; }

        #endregion
    }
}