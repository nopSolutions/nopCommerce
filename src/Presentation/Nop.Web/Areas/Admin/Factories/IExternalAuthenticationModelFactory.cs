using Nop.Web.Areas.Admin.Models.ExternalAuthentication;

namespace Nop.Web.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the external authentication method model factory
    /// </summary>
    public partial interface IExternalAuthenticationMethodModelFactory
    {
        /// <summary>
        /// Prepare external authentication method search model
        /// </summary>
        /// <param name="model">External authentication method search model</param>
        /// <returns>External authentication method search model</returns>
        ExternalAuthenticationMethodSearchModel PrepareExternalAuthenticationMethodSearchModel(
            ExternalAuthenticationMethodSearchModel model);

        /// <summary>
        /// Prepare paged external authentication method list model
        /// </summary>
        /// <param name="searchModel">External authentication method search model</param>
        /// <returns>External authentication method list model</returns>
        ExternalAuthenticationMethodListModel PrepareExternalAuthenticationMethodListModel(
            ExternalAuthenticationMethodSearchModel searchModel);
    }
}