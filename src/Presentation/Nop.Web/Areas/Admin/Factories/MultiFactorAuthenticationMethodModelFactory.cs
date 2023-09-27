using Nop.Services.Authentication.MultiFactor;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.MultiFactorAuthentication;
using Nop.Web.Framework.Models.Extensions;

namespace Nop.Web.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the multi-factor authentication method model factory implementation
    /// </summary>
    public partial class MultiFactorAuthenticationMethodModelFactory : IMultiFactorAuthenticationMethodModelFactory
    {
        #region Fields

        protected readonly IMultiFactorAuthenticationPluginManager _multiFactorAuthenticationPluginManager;

        #endregion

        #region Ctor

        public MultiFactorAuthenticationMethodModelFactory(IMultiFactorAuthenticationPluginManager multiFactorAuthenticationPluginManager)
        {
            _multiFactorAuthenticationPluginManager = multiFactorAuthenticationPluginManager;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare multi-factor authentication method search model
        /// </summary>
        /// <param name="searchModel">Multi-factor authentication method search model</param>
        /// <returns>Multi-factor authentication method search model</returns>
        public virtual MultiFactorAuthenticationMethodSearchModel PrepareMultiFactorAuthenticationMethodSearchModel(
            MultiFactorAuthenticationMethodSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare paged multi-factor authentication method list model
        /// </summary>
        /// <param name="searchModel">Multi-factor authentication method search model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the multi-factor authentication method list model
        /// </returns>
        public virtual async Task<MultiFactorAuthenticationMethodListModel>
            PrepareMultiFactorAuthenticationMethodListModelAsync(MultiFactorAuthenticationMethodSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get multi-factor authentication methods
            var multiFactorAuthenticationMethods = (await _multiFactorAuthenticationPluginManager.LoadAllPluginsAsync()).ToPagedList(searchModel);

            //prepare grid model
            var model = new MultiFactorAuthenticationMethodListModel().PrepareToGrid(searchModel, multiFactorAuthenticationMethods, () =>
            {
                //fill in model values from the entity
                return multiFactorAuthenticationMethods.Select(method =>
                {
                    //fill in model values from the entity
                    var multiFactorAuthenticationMethodModel = method.ToPluginModel<MultiFactorAuthenticationMethodModel>();

                    //fill in additional values (not existing in the entity)
                    multiFactorAuthenticationMethodModel.IsActive = _multiFactorAuthenticationPluginManager.IsPluginActive(method);
                    multiFactorAuthenticationMethodModel.ConfigurationUrl = method.GetConfigurationPageUrl();

                    return multiFactorAuthenticationMethodModel;
                });
            });

            return model;
        }

        #endregion
    }
}
