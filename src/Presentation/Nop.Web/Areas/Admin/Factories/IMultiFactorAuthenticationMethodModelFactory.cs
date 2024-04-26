using Nop.Web.Areas.Admin.Models.MultiFactorAuthentication;

namespace Nop.Web.Areas.Admin.Factories;

/// <summary>
/// Represents the multi-factor authentication method model factory
/// </summary>
public partial interface IMultiFactorAuthenticationMethodModelFactory
{
    /// <summary>
    /// Prepare multi-factor authentication method search model
    /// </summary>
    /// <param name="searchModel">Multi-factor authentication method search model</param>
    /// <returns>Multi-factor authentication method search model</returns>
    MultiFactorAuthenticationMethodSearchModel PrepareMultiFactorAuthenticationMethodSearchModel(
        MultiFactorAuthenticationMethodSearchModel searchModel);

    /// <summary>
    /// Prepare paged multi-factor authentication method list model
    /// </summary>
    /// <param name="searchModel">Multi-factor authentication method search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the multi-factor authentication method list model
    /// </returns>
    Task<MultiFactorAuthenticationMethodListModel> PrepareMultiFactorAuthenticationMethodListModelAsync(
        MultiFactorAuthenticationMethodSearchModel searchModel);
}