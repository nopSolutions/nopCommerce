using Nop.Core;
using Nop.Services.Authentication.External;
using Nop.Web.Models.Customer;

namespace Nop.Web.Factories
{
    /// <summary>
    /// Represents the external authentication model factory
    /// </summary>
    public partial class ExternalAuthenticationModelFactory : IExternalAuthenticationModelFactory
    {
        #region Fields

        protected readonly IAuthenticationPluginManager _authenticationPluginManager;
        protected readonly IStoreContext _storeContext;
        protected readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public ExternalAuthenticationModelFactory(IAuthenticationPluginManager authenticationPluginManager,
            IStoreContext storeContext,
            IWorkContext workContext)
        {
            _authenticationPluginManager = authenticationPluginManager;
            _storeContext = storeContext;
            _workContext = workContext;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare the external authentication method model
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the list of the external authentication method model
        /// </returns>
        public virtual async Task<List<ExternalAuthenticationMethodModel>> PrepareExternalMethodsModelAsync()
        {
            var store = await _storeContext.GetCurrentStoreAsync();

            return (await _authenticationPluginManager
                .LoadActivePluginsAsync(await _workContext.GetCurrentCustomerAsync(), store.Id))
                .Select(authenticationMethod => new ExternalAuthenticationMethodModel
                {
                    ViewComponent = authenticationMethod.GetPublicViewComponent()
                })
                .ToList();
        }

        #endregion
    }
}