using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using Nop.Admin.Models;
using Nop.Core;
using Nop.Core.Domain.Shipping;
using Nop.Services;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Directory;
using Nop.Services.Logging;
using Nop.Services.Shipping;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Telerik.Web.Mvc;

namespace Nop.Admin.Controllers
{
	[AdminAuthorize]
    public class SecurityController : BaseNopController
	{
		#region Fields

        private readonly ILogger _logger;
        private readonly IAuthenticationService _authenticationService;

		#endregion

		#region Constructors

        public SecurityController(ILogger logger, IAuthenticationService authenticationService)
		{
            this._logger = logger;
            this._authenticationService = authenticationService;
		}

		#endregion Constructors 

        #region Methods

        public ActionResult AccessDenied(string pageUrl)
        {
            var currentUser = _authenticationService.GetAuthenticatedUser();

            if (currentUser == null)
            {
                _logger.Information(string.Format("Access denied to anonymous request on {0}", pageUrl));
                return View();
            }

            _logger.Information(string.Format("Access denied to user #{0} '{1}' on {2}", currentUser.Email, currentUser.Email, pageUrl));


            return View();
        }
        
        #endregion
    }
}
