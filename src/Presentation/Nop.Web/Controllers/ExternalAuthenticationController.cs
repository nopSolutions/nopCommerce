using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;
using Nop.Core;
using Nop.Services.Authentication.External;
using Nop.Web.Models.Customer;

namespace Nop.Web.Controllers
{
    public partial class ExternalAuthenticationController : BasePublicController
    {
		#region Fields

        private readonly IOpenAuthenticationService _openAuthenticationService;
        private readonly IStoreContext _storeContext;

        #endregion

		#region Constructors

        public ExternalAuthenticationController(IOpenAuthenticationService openAuthenticationService,
            IStoreContext storeContext)
        {
            this._openAuthenticationService = openAuthenticationService;
            this._storeContext = storeContext;
        }

        #endregion

        #region Methods

        public RedirectResult RemoveParameterAssociation(string returnUrl)
        {
            //prevent open redirection attack
            if (!Url.IsLocalUrl(returnUrl))
                returnUrl = Url.RouteUrl("HomePage");

            ExternalAuthorizerHelper.RemoveParameters();
            return Redirect(returnUrl);
        }

        [ChildActionOnly]
        public ActionResult ExternalMethods()
        {
            //model
            var model = new List<ExternalAuthenticationMethodModel>();

            foreach (var eam in _openAuthenticationService
                .LoadActiveExternalAuthenticationMethods(_storeContext.CurrentStore.Id))
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

            return PartialView(model);
        }

        #endregion
    }
}
