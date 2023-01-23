using System.Collections.Generic;
using Nop.Web.Framework.Models;

namespace Nop.Plugin.Payments.CyberSource.Models
{
    /// <summary>
    /// Represents a CyberSource customer token list model
    /// </summary>
    public record CustomerTokenListModel : BaseNopModel
    {
        #region Ctor

        public CustomerTokenListModel()
        {
            Tokens = new List<CustomerTokenDetailsModel>();
        }

        #endregion

        #region Properties

        public IList<CustomerTokenDetailsModel> Tokens { get; set; }

        #endregion

        #region Nested classes

        public record CustomerTokenDetailsModel : BaseNopEntityModel
        {
            public string CardNumber { get; set; }

            public string ThreeDigitCardType { get; set; }

            public string CardExpirationYear { get; set; }

            public string CardExpirationMonth { get; set; }
        }

        #endregion
    }
}