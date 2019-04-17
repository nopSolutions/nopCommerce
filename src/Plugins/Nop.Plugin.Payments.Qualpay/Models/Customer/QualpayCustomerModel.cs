using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Payments.Qualpay.Models.Customer
{
    /// <summary>
    /// Represents Qualpay customer model
    /// </summary>
    public class QualpayCustomerModel : BaseNopEntityModel
    {
        #region Ctor

        public QualpayCustomerModel()
        {
            CustomerCardSearchModel = new QualpayCustomerCardSearchModel();
        }

        #endregion

        #region Properties

        [NopResourceDisplayName("Plugins.Payments.Qualpay.Customer")]
        public string QualpayCustomerId { get; set; }

        public bool CustomerExists { get; set; }

        public bool HideBlock { get; set; }

        public QualpayCustomerCardSearchModel CustomerCardSearchModel { get; set; }

        #endregion
    }
}