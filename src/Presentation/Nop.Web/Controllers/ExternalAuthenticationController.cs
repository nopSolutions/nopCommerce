using Microsoft.AspNetCore.Mvc;
using Nop.Services.Authentication.External;

namespace Nop.Web.Controllers
{
    public partial class ExternalAuthenticationController : BasePublicController
    {
        #region Methods

        public virtual IActionResult RemoveParameterAssociation(string returnUrl)
        {
            //prevent open redirection attack
            if (!Url.IsLocalUrl(returnUrl))
                returnUrl = Url.RouteUrl("HomePage");

            ExternalAuthorizerHelper.RemoveParameters();

            return Redirect(returnUrl);
        }

        #endregion
    }
}