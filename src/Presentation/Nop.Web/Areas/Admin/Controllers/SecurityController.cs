using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        protected ICustomerService CustomerService { get; }
        protected ILocalizationService LocalizationService { get; }
        protected ILogger Logger { get; }
        protected INotificationService NotificationService { get; }
        protected IPermissionService PermissionService { get; }
        protected ISecurityModelFactory SecurityModelFactory { get; }
        protected IWorkContext WorkContext { get; }

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
            CustomerService = customerService;
            LocalizationService = localizationService;
            Logger = logger;
            NotificationService = notificationService;
            PermissionService = permissionService;
            SecurityModelFactory = securityModelFactory;
            WorkContext = workContext;
        }

        #endregion

        #region Methods

        public virtual async Task<IActionResult> AccessDenied(string pageUrl)
        {
            var currentCustomer = await WorkContext.GetCurrentCustomerAsync();
            if (currentCustomer == null || await CustomerService.IsGuestAsync(currentCustomer))
            {
                await Logger.InformationAsync($"Access denied to anonymous request on {pageUrl}");
                return View();
            }

            await Logger.InformationAsync($"Access denied to user #{currentCustomer.Email} '{currentCustomer.Email}' on {pageUrl}");

            return View();
        }

        public virtual async Task<IActionResult> Permissions()
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageAcl))
                return AccessDeniedView();

            //prepare model
            var model = await SecurityModelFactory.PreparePermissionMappingModelAsync(new PermissionMappingModel());

            return View(model);
        }

        [HttpPost, ActionName("Permissions")]
        public virtual async Task<IActionResult> PermissionsSave(PermissionMappingModel model)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageAcl))
                return AccessDeniedView();

            var permissionRecords = await PermissionService.GetAllPermissionRecordsAsync();
            var customerRoles = await CustomerService.GetAllCustomerRolesAsync(true);

            var form = model.Form;

            foreach (var cr in customerRoles)
            {
                var formKey = "allow_" + cr.Id;
                var permissionRecordSystemNamesToRestrict = !StringValues.IsNullOrEmpty(form[formKey])
                    ? form[formKey].ToString().Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList()
                    : new List<string>();

                foreach (var pr in permissionRecords)
                {
                    var allow = permissionRecordSystemNamesToRestrict.Contains(pr.SystemName);

                    if (allow == await PermissionService.AuthorizeAsync(pr.SystemName, cr.Id))
                        continue;

                    if (allow)
                    {
                        await PermissionService.InsertPermissionRecordCustomerRoleMappingAsync(new PermissionRecordCustomerRoleMapping { PermissionRecordId = pr.Id, CustomerRoleId = cr.Id });
                    }
                    else
                    {
                        await PermissionService.DeletePermissionRecordCustomerRoleMappingAsync(pr.Id, cr.Id);                        
                    }

                    await PermissionService.UpdatePermissionRecordAsync(pr);
                }
            }

            NotificationService.SuccessNotification(await LocalizationService.GetResourceAsync("Admin.Configuration.ACL.Updated"));

            return RedirectToAction("Permissions");
        }

        #endregion
    }
}