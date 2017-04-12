#if NET451
using System.Web.Mvc;
using Nop.Services.Authentication.External;
using Nop.Web.Factories;

namespace Nop.Web.Controllers
{
    public partial class ExternalAuthenticationController : BasePublicController
    {
		#region Fields

        private readonly IExternalAuthenticationModelFactory _externalAuthenticationModelFactory;

        #endregion

        #region Ctor

        public ExternalAuthenticationController(IExternalAuthenticationModelFactory externalAuthenticationModelFactory)
        {
            this._externalAuthenticationModelFactory = externalAuthenticationModelFactory;
        }

        #endregion

        #region Methods

        public virtual RedirectResult RemoveParameterAssociation(string returnUrl)
        {
            //prevent open redirection attack
            if (!Url.IsLocalUrl(returnUrl))
                returnUrl = Url.RouteUrl("HomePage");

            ExternalAuthorizerHelper.RemoveParameters();
            return Redirect(returnUrl);
        }

        [ChildActionOnly]
        public virtual ActionResult ExternalMethods()
        {
            var model = _externalAuthenticationModelFactory.PrepareExternalMethodsModel();
            return PartialView(model);
        }

        #endregion
    }
}
#endif