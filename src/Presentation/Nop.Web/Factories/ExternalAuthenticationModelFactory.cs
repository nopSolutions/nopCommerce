using System.Collections.Generic;
using System.Linq;
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

        private readonly IAuthenticationPluginManager _authenticationPluginManager;
        private readonly IStoreContext _storeContext;
        private readonly IWorkContext _workContext;

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
        /// <returns>List of the external authentication method model</returns>
        public virtual List<ExternalAuthenticationMethodModel> PrepareExternalMethodsModel()
        {
            return _authenticationPluginManager
                .LoadActivePlugins(_workContext.CurrentCustomer, _storeContext.CurrentStore.Id)
                .Select(authenticationMethod => new ExternalAuthenticationMethodModel
                {
                    ViewComponentName = authenticationMethod.GetPublicViewComponentName()
                })
                .ToList();
        }

        #endregion
    }
}