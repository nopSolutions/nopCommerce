using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Customers;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Web.Areas.Admin.Controllers
{
    public partial class CustomerRoleController : BaseAdminController
    {
        #region Fields

        protected ICustomerActivityService CustomerActivityService { get; }
        protected ICustomerRoleModelFactory CustomerRoleModelFactory { get; }
        protected ICustomerService CustomerService { get; }
        protected ILocalizationService LocalizationService { get; }
        protected INotificationService NotificationService { get; }
        protected IPermissionService PermissionService { get; }
        protected IProductService ProductService { get; }
        protected IWorkContext WorkContext { get; }

        #endregion

        #region Ctor

        public CustomerRoleController(ICustomerActivityService customerActivityService,
            ICustomerRoleModelFactory customerRoleModelFactory,
            ICustomerService customerService,
            ILocalizationService localizationService,
            INotificationService notificationService,
            IPermissionService permissionService,
            IProductService productService,
            IWorkContext workContext)
        {
            CustomerActivityService = customerActivityService;
            CustomerRoleModelFactory = customerRoleModelFactory;
            CustomerService = customerService;
            LocalizationService = localizationService;
            NotificationService = notificationService;
            PermissionService = permissionService;
            ProductService = productService;
            WorkContext = workContext;
        }

        #endregion

        #region Methods

        public virtual IActionResult Index()
        {
            return RedirectToAction("List");
        }

        public virtual async Task<IActionResult> List()
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();

            //prepare model
            var model = await CustomerRoleModelFactory.PrepareCustomerRoleSearchModelAsync(new CustomerRoleSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> List(CustomerRoleSearchModel searchModel)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageCustomers))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await CustomerRoleModelFactory.PrepareCustomerRoleListModelAsync(searchModel);

            return Json(model);
        }

        public virtual async Task<IActionResult> Create()
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageCustomers) || !await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageAcl))
                return AccessDeniedView();

            //prepare model
            var model = await CustomerRoleModelFactory.PrepareCustomerRoleModelAsync(new CustomerRoleModel(), null);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> Create(CustomerRoleModel model, bool continueEditing)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageCustomers) || !await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageAcl))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var customerRole = model.ToEntity<CustomerRole>();
                await CustomerService.InsertCustomerRoleAsync(customerRole);

                //activity log
                await CustomerActivityService.InsertActivityAsync("AddNewCustomerRole",
                    string.Format(await LocalizationService.GetResourceAsync("ActivityLog.AddNewCustomerRole"), customerRole.Name), customerRole);

                NotificationService.SuccessNotification(await LocalizationService.GetResourceAsync("Admin.Customers.CustomerRoles.Added"));

                return continueEditing ? RedirectToAction("Edit", new { id = customerRole.Id }) : RedirectToAction("List");
            }

            //prepare model
            model = await CustomerRoleModelFactory.PrepareCustomerRoleModelAsync(model, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual async Task<IActionResult> Edit(int id)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageCustomers) || !await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageAcl))
                return AccessDeniedView();

            //try to get a customer role with the specified id
            var customerRole = await CustomerService.GetCustomerRoleByIdAsync(id);
            if (customerRole == null)
                return RedirectToAction("List");

            //prepare model
            var model = await CustomerRoleModelFactory.PrepareCustomerRoleModelAsync(null, customerRole);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> Edit(CustomerRoleModel model, bool continueEditing)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageCustomers) || !await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageAcl))
                return AccessDeniedView();

            //try to get a customer role with the specified id
            var customerRole = await CustomerService.GetCustomerRoleByIdAsync(model.Id);
            if (customerRole == null)
                return RedirectToAction("List");

            try
            {
                if (ModelState.IsValid)
                {
                    if (customerRole.IsSystemRole && !model.Active)
                        throw new NopException(await LocalizationService.GetResourceAsync("Admin.Customers.CustomerRoles.Fields.Active.CantEditSystem"));

                    if (customerRole.IsSystemRole && !customerRole.SystemName.Equals(model.SystemName, StringComparison.InvariantCultureIgnoreCase))
                        throw new NopException(await LocalizationService.GetResourceAsync("Admin.Customers.CustomerRoles.Fields.SystemName.CantEditSystem"));

                    if (NopCustomerDefaults.RegisteredRoleName.Equals(customerRole.SystemName, StringComparison.InvariantCultureIgnoreCase) &&
                        model.PurchasedWithProductId > 0)
                        throw new NopException(await LocalizationService.GetResourceAsync("Admin.Customers.CustomerRoles.Fields.PurchasedWithProduct.Registered"));

                    customerRole = model.ToEntity(customerRole);
                    await CustomerService.UpdateCustomerRoleAsync(customerRole);

                    //activity log
                    await CustomerActivityService.InsertActivityAsync("EditCustomerRole",
                        string.Format(await LocalizationService.GetResourceAsync("ActivityLog.EditCustomerRole"), customerRole.Name), customerRole);

                    NotificationService.SuccessNotification(await LocalizationService.GetResourceAsync("Admin.Customers.CustomerRoles.Updated"));

                    return continueEditing ? RedirectToAction("Edit", new { id = customerRole.Id }) : RedirectToAction("List");
                }

                //prepare model
                model = await CustomerRoleModelFactory.PrepareCustomerRoleModelAsync(model, customerRole, true);

                //if we got this far, something failed, redisplay form
                return View(model);
            }
            catch (Exception exc)
            {
                await NotificationService.ErrorNotificationAsync(exc);
                return RedirectToAction("Edit", new { id = customerRole.Id });
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageCustomers) || !await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageAcl))
                return AccessDeniedView();

            //try to get a customer role with the specified id
            var customerRole = await CustomerService.GetCustomerRoleByIdAsync(id);
            if (customerRole == null)
                return RedirectToAction("List");

            try
            {
                await CustomerService.DeleteCustomerRoleAsync(customerRole);

                //activity log
                await CustomerActivityService.InsertActivityAsync("DeleteCustomerRole",
                    string.Format(await LocalizationService.GetResourceAsync("ActivityLog.DeleteCustomerRole"), customerRole.Name), customerRole);

                NotificationService.SuccessNotification(await LocalizationService.GetResourceAsync("Admin.Customers.CustomerRoles.Deleted"));

                return RedirectToAction("List");
            }
            catch (Exception exc)
            {
                NotificationService.ErrorNotification(exc.Message);
                return RedirectToAction("Edit", new { id = customerRole.Id });
            }
        }

        public virtual async Task<IActionResult> AssociateProductToCustomerRolePopup()
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageCustomers) || !await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageAcl))
                return AccessDeniedView();

            //prepare model
            var model = await CustomerRoleModelFactory.PrepareCustomerRoleProductSearchModelAsync(new CustomerRoleProductSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> AssociateProductToCustomerRolePopupList(CustomerRoleProductSearchModel searchModel)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageCustomers) || !await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageAcl))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await CustomerRoleModelFactory.PrepareCustomerRoleProductListModelAsync(searchModel);

            return Json(model);
        }

        [HttpPost]
        [FormValueRequired("save")]
        public virtual async Task<IActionResult> AssociateProductToCustomerRolePopup([Bind(Prefix = nameof(AddProductToCustomerRoleModel))] AddProductToCustomerRoleModel model)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageCustomers) || !await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageAcl))
                return AccessDeniedView();

            //try to get a product with the specified id
            var associatedProduct = await ProductService.GetProductByIdAsync(model.AssociatedToProductId);
            if (associatedProduct == null)
                return Content("Cannot load a product");

            //a vendor should have access only to his products
            var currentVendor = await WorkContext.GetCurrentVendorAsync();
            if (currentVendor != null && associatedProduct.VendorId != currentVendor.Id)
                return Content("This is not your product");

            ViewBag.RefreshPage = true;
            ViewBag.productId = associatedProduct.Id;
            ViewBag.productName = associatedProduct.Name;

            return View(new CustomerRoleProductSearchModel());
        }

        #endregion
    }
}