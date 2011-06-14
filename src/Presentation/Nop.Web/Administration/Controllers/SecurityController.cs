using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Nop.Admin.Models.Customers;
using Nop.Admin.Models.Security;
using Nop.Services;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Security.Permissions;
using Nop.Web.Framework.Controllers;

namespace Nop.Admin.Controllers
{
	[AdminAuthorize]
    public class SecurityController : BaseNopController
	{
		#region Fields

        private readonly ILogger _logger;
        private readonly IAuthenticationService _authenticationService;
        private readonly IPermissionService _permissionService;
        private readonly ICustomerService _customerService;
        private readonly ILocalizationService _localizationService;

		#endregion

		#region Constructors

        public SecurityController(ILogger logger, IAuthenticationService authenticationService,
            IPermissionService permissionService,
            ICustomerService customerService, ILocalizationService localizationService)
		{
            this._logger = logger;
            this._authenticationService = authenticationService;
            this._permissionService = permissionService;
            this._customerService = customerService;
            this._localizationService = localizationService;
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

        public ActionResult Permissions()
        {
            var model = new PermissionMappingModel();

            var permissionRecords = _permissionService.GetAllPermissionRecords();
            var customerRoles = _customerService.GetAllCustomerRoles(true);
            foreach (var pr in permissionRecords)
            {
                model.AvailablePermissions.Add(new PermissionRecordModel()
                {
                    Name = pr.Name,
                    SystemName = pr.SystemName
                });
            }
            foreach (var cr in customerRoles)
            {
                model.AvailableCustomerRoles.Add(new CustomerRoleModel()
                {
                    Id = cr.Id,
                    Name = cr.Name
                });
            }
            foreach (var pr in permissionRecords)
                foreach (var cr in customerRoles)
                {
                    bool allowed = pr.CustomerRoles.Where(x => x.Id == cr.Id).ToList().Count() > 0;
                    if (!model.Allowed.ContainsKey(pr.SystemName))
                        model.Allowed[pr.SystemName] = new Dictionary<int, bool>();
                    model.Allowed[pr.SystemName][cr.Id] = allowed;
                }

            return View(model);
        }

        [HttpPost, ActionName("Permissions")]
        public ActionResult PermissionsSave(FormCollection form)
        {
            var permissionRecords = _permissionService.GetAllPermissionRecords();
            var customerRoles = _customerService.GetAllCustomerRoles(true);


            foreach (var cr in customerRoles)
            {
                string formKey = "allow_" + cr.Id;
                var permissionRecordSystemNamesToRestrict = form[formKey] != null ? form[formKey].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList() : new List<string>();

                foreach (var pr in permissionRecords)
                {

                    bool allow = permissionRecordSystemNamesToRestrict.Contains(pr.SystemName);
                    if (allow)
                    {
                        if (pr.CustomerRoles.Where(x => x.Id == cr.Id).FirstOrDefault() == null)
                        {
                            pr.CustomerRoles.Add(cr);
                            _permissionService.UpdatePermissionRecord(pr);
                        }
                    }
                    else
                    {
                        if (pr.CustomerRoles.Where(x => x.Id == cr.Id).FirstOrDefault() != null)
                        {
                            pr.CustomerRoles.Remove(cr);
                            _permissionService.UpdatePermissionRecord(pr);
                        }
                    }
                }
            }

            SuccessNotification(_localizationService.GetResource("Admin.Configuration.ACL.Updated"));
            return RedirectToAction("Permissions");
        }
        #endregion
    }
}
