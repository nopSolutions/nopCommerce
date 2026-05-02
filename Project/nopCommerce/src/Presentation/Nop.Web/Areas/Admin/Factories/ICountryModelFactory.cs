using Nop.Core.Domain.Directory;
using Nop.Web.Areas.Admin.Models.Directory;

namespace Nop.Web.Areas.Admin.Factories;

/// <summary>
/// Represents the country model factory
/// </summary>
public partial interface ICountryModelFactory
{
    /// <summary>
    /// Prepare country search model
    /// </summary>
    /// <param name="searchModel">Country search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the country search model
    /// </returns>
    Task<CountrySearchModel> PrepareCountrySearchModelAsync(CountrySearchModel searchModel);

    /// <summary>
    /// Prepare paged country list model
    /// </summary>
    /// <param name="searchModel">Country search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the country list model
    /// </returns>
    Task<CountryListModel> PrepareCountryListModelAsync(CountrySearchModel searchModel);

    /// <summary>
    /// Prepare country model
    /// </summary>
    /// <param name="model">Country model</param>
    /// <param name="country">Country</param>
    /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the country model
    /// </returns>
    Task<CountryModel> PrepareCountryModelAsync(CountryModel model, Country country, bool excludeProperties = false);

    /// <summary>
    /// Prepare paged state and province list model
    /// </summary>
    /// <param name="searchModel">State and province search model</param>
    /// <param name="country">Country</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the state and province list model
    /// </returns>
    Task<StateProvinceListModel> PrepareStateProvinceListModelAsync(StateProvinceSearchModel searchModel, Country country);

    /// <summary>
    /// Prepare state and province model
    /// </summary>
    /// <param name="model">State and province model</param>
    /// <param name="country">Country</param>
    /// <param name="state">State or province</param>
    /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the state and province model
    /// </returns>
    Task<StateProvinceModel> PrepareStateProvinceModelAsync(StateProvinceModel model,
        Country country, StateProvince state, bool excludeProperties = false);
}