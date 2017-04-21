using System.Collections.Generic;
using System.Web.Routing;
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

        private readonly IOpenAuthenticationService _openAuthenticationService;
        private readonly IStoreContext _storeContext;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public ExternalAuthenticationModelFactory(IOpenAuthenticationService openAuthenticationService,
            IStoreContext storeContext,
            IWorkContext workContext)
        {
            this._openAuthenticationService = openAuthenticationService;
            this._storeContext = storeContext;
            this._workContext = workContext;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare the external authentication method model
        /// </summary>
        /// <returns>List of the external authentication method model</returns>
        public virtual List<ExternalAuthenticationMethodModel> PrepareExternalMethodsModel()
        {
            var model = new List<ExternalAuthenticationMethodModel>();

            foreach (var eam in _openAuthenticationService
                .LoadActiveExternalAuthenticationMethods(_workContext.CurrentCustomer, _storeContext.CurrentStore.Id))
            {
                var eamModel = new ExternalAuthenticationMethodModel();

                string actionName;
                string controllerName;
                RouteValueDictionary routeValues;
                eam.GetPublicInfoRoute(out actionName, out controllerName, out routeValues);
                eamModel.ActionName = actionName;
                eamModel.ControllerName = controllerName;
                eamModel.RouteValues = routeValues;

                model.Add(eamModel);
            }

            return model;
        }

        #endregion
    }
}
