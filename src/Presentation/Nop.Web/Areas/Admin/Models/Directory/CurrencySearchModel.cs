using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Directory
{
    /// <summary>
    /// Represents a currency search model
    /// </summary>
    public partial record CurrencySearchModel : BaseSearchModel
    {
        #region Ctor

        public CurrencySearchModel()
        {
            ExchangeRateProviderModel = new CurrencyExchangeRateProviderModel();
        }

        #endregion

        #region Properties
        
        public CurrencyExchangeRateProviderModel ExchangeRateProviderModel { get; set; }

        #endregion
    }
}