using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Nop.Core;
using Nop.Core.Domain.Security;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Models.Security;

namespace Nop.Web.Areas.Admin.Controllers
{
    public partial class SecurityController : BaseAdminController
    {
        #region Fields

        private readonly ICustomerService _customerService;
        private readonly ILocalizationService _localizationService;
        private readonly ILogger _logger;
        private readonly INotificationService _notificationService;
        private readonly IPermissionService _permissionService;
        private readonly ISecurityModelFactory _securityModelFactory;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public SecurityController(ICustomerService customerService,
            ILocalizationService localizationService,
            ILogger logger,
            INotificationService notificationService,
            IPermissionService permissionService,
            ISecurityModelFactory securityModelFactory,
            IWorkContext workContext)
        {
            _customerService = customerService;
            _localizationService = localizationService;
            _logger = logger;
            _notificationService = notificationService;
            _permissionService = permissionService;
            _securityModelFactory = securityModelFactory;
            _workContext = workContext;
        }

        #endregion

        #region Methods

        public virtual async Task<IActionResult> AccessDenied(string pageUrl)
        {
            var currentCustomer = await _workContext.GetCurrentCustomerAsync();
            if (currentCustomer == null || await _customerService.IsGuestAsync(currentCustomer))
            {
                await _logger.InformationAsync($"Access denied to anonymous request on {pageUrl}");
                return View();
            }

            await _logger.InformationAsync($"Access denied to user #{currentCustomer.Email} '{currentCustomer.Email}' on {pageUrl}");

            return View();
        }

        public virtual async Task<IActionResult> Permissions()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageAcl))
                return AccessDeniedView();

            //prepare model
            var model = await _securityModelFactory.PreparePermissionMappingModelAsync(new PermissionMappingModel());

            return View(model);
        }

        [HttpPost, ActionName("Permissions")]
        public virtual async Task<IActionResult> PermissionsSave(PermissionMappingModel model, IFormCollection form)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageAcl))
                return AccessDeniedView();

            var permissionRecords = await _permissionService.GetAllPermissionRecordsAsync();
            var customerRoles = await _customerService.GetAllCustomerRolesAsync(true);

            foreach (var cr in customerRoles)
            {
                var formKey = "allow_" + cr.Id;
                var permissionRecordSystemNamesToRestrict = !StringValues.IsNullOrEmpty(form[formKey])
                    ? form[formKey].ToString().Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList()
                    : new List<string>();

                foreach (var pr in permissionRecords)
                {
                    var allow = permissionRecordSystemNamesToRestrict.Contains(pr.SystemName);

                    if (allow == await _permissionService.AuthorizeAsync(pr.SystemName, cr.Id))
                        continue;

                    if (allow)
                    {
                        await _permissionService.InsertPermissionRecordCustomerRoleMappingAsync(new PermissionRecordCustomerRoleMapping { PermissionRecordId = pr.Id, CustomerRoleId = cr.Id });
                    }
                    else
                    {
                        await _permissionService.DeletePermissionRecordCustomerRoleMappingAsync(pr.Id, cr.Id);                        
                    }

                    await _permissionService.UpdatePermissionRecordAsync(pr);
                }
            }

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.ACL.Updated"));

            return RedirectToAction("Permissions");
        }

        #endregion
    }
}