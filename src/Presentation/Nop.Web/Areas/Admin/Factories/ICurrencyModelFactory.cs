using Nop.Core.Domain.Directory;
using Nop.Web.Areas.Admin.Models.Directory;

namespace Nop.Web.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the currency model factory
    /// </summary>
    public partial interface ICurrencyModelFactory
    {
        /// <summary>
        /// Prepare currency search model
        /// </summary>
        /// <param name="searchModel">Currency search model</param>
        /// <param name="prepareExchangeRates">Whether to prepare exchange rate models</param>
        /// <returns>Currency search model</returns>
        CurrencySearchModel PrepareCurrencySearchModel(CurrencySearchModel searchModel, bool prepareExchangeRates = false);

        /// <summary>
        /// Prepare paged currency list model
        /// </summary>
        /// <param name="searchModel">Currency search model</param>
        /// <returns>Currency list model</returns>
        CurrencyListModel PrepareCurrencyListModel(CurrencySearchModel searchModel);

        /// <summary>
        /// Prepare currency model
        /// </summary>
        /// <param name="model">Currency model</param>
        /// <param name="currency">Currency</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>Currency model</returns>
        CurrencyModel PrepareCurrencyModel(CurrencyModel model, Currency currency, bool excludeProperties = false);
    }
}