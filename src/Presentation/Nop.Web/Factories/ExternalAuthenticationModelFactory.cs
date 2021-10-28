using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        protected IAuthenticationPluginManager AuthenticationPluginManager { get; }
        protected IStoreContext StoreContext { get; }
        protected IWorkContext WorkContext { get; }

        #endregion

        #region Ctor

        public ExternalAuthenticationModelFactory(IAuthenticationPluginManager authenticationPluginManager,
            IStoreContext storeContext,
            IWorkContext workContext)
        {
            AuthenticationPluginManager = authenticationPluginManager;
            StoreContext = storeContext;
            WorkContext = workContext;
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
            var store = await StoreContext.GetCurrentStoreAsync();

            return (await AuthenticationPluginManager
                .LoadActivePluginsAsync(await WorkContext.GetCurrentCustomerAsync(), store.Id))
                .Select(authenticationMethod => new ExternalAuthenticationMethodModel
                {
                    ViewComponentName = authenticationMethod.GetPublicViewComponentName()
                })
                .ToList();
        }

        #endregion
    }
}