using Nop.Web.Areas.Admin.Models.Directory;
using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Payments
{
    /// <summary>
    /// Represents a payment method restriction model
    /// </summary>
    public partial record PaymentMethodRestrictionModel : BaseNopModel
    {
        #region Ctor

        public PaymentMethodRestrictionModel()
        {
            AvailablePaymentMethods = new List<PaymentMethodModel>();
            AvailableCountries = new List<CountryModel>();
            Restricted = new Dictionary<string, IDictionary<int, bool>>();
        }

        #endregion

        #region Properties

        public IList<PaymentMethodModel> AvailablePaymentMethods { get; set; }

        public IList<CountryModel> AvailableCountries { get; set; }

        //[payment method system name] / [customer role id] / [restricted]
        public IDictionary<string, IDictionary<int, bool>> Restricted { get; set; }

        #endregion
    }
}