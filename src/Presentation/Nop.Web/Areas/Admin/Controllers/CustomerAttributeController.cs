using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Customers;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Customers;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Web.Areas.Admin.Controllers
{
    public partial class CustomerAttributeController : BaseAdminController
    {
        #region Fields

        protected ICustomerActivityService CustomerActivityService { get; }
        protected ICustomerAttributeModelFactory CustomerAttributeModelFactory { get; }
        protected ICustomerAttributeService CustomerAttributeService { get; }
        protected ILocalizationService LocalizationService { get; }
        protected ILocalizedEntityService LocalizedEntityService { get; }
        protected INotificationService NotificationService { get; }
        protected IPermissionService PermissionService { get; }

        #endregion

        #region Ctor

        public CustomerAttributeController(ICustomerActivityService customerActivityService,
            ICustomerAttributeModelFactory customerAttributeModelFactory,
            ICustomerAttributeService customerAttributeService,
            ILocalizationService localizationService,
            ILocalizedEntityService localizedEntityService,
            INotificationService notificationService,
            IPermissionService permissionService)
        {
            CustomerActivityService = customerActivityService;
            CustomerAttributeModelFactory = customerAttributeModelFactory;
            CustomerAttributeService = customerAttributeService;
            LocalizationService = localizationService;
            LocalizedEntityService = localizedEntityService;
            NotificationService = notificationService;
            PermissionService = permissionService;
        }

        #endregion

        #region Utilities

        protected virtual async Task UpdateAttributeLocalesAsync(CustomerAttribute customerAttribute, CustomerAttributeModel model)
        {
            foreach (var localized in model.Locales)
            {
                await LocalizedEntityService.SaveLocalizedValueAsync(customerAttribute,
                    x => x.Name,
                    localized.Name,
                    localized.LanguageId);
            }
        }

        protected virtual async Task UpdateValueLocalesAsync(CustomerAttributeValue customerAttributeValue, CustomerAttributeValueModel model)
        {
            foreach (var localized in model.Locales)
            {
                await LocalizedEntityService.SaveLocalizedValueAsync(customerAttributeValue,
                    x => x.Name,
                    localized.Name,
                    localized.LanguageId);
            }
        }

        #endregion

        #region Customer attributes

        public virtual IActionResult Index()
        {
            return RedirectToAction("List");
        }

        public virtual async Task<IActionResult> List()
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //select an appropriate card
            SaveSelectedCardName("customersettings-customerformfields");

            //we just redirect a user to the customer settings page
            return RedirectToAction("CustomerUser", "Setting");
        }

        [HttpPost]
        public virtual async Task<IActionResult> List(CustomerAttributeSearchModel searchModel)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await CustomerAttributeModelFactory.PrepareCustomerAttributeListModelAsync(searchModel);

            return Json(model);
        }

        public virtual async Task<IActionResult> Create()
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //prepare model
            var model = await CustomerAttributeModelFactory.PrepareCustomerAttributeModelAsync(new CustomerAttributeModel(), null);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> Create(CustomerAttributeModel model, bool continueEditing)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var customerAttribute = model.ToEntity<CustomerAttribute>();
                await CustomerAttributeService.InsertCustomerAttributeAsync(customerAttribute);

                //activity log
                await CustomerActivityService.InsertActivityAsync("AddNewCustomerAttribute",
                    string.Format(await LocalizationService.GetResourceAsync("ActivityLog.AddNewCustomerAttribute"), customerAttribute.Id),
                    customerAttribute);

                //locales
                await UpdateAttributeLocalesAsync(customerAttribute, model);

                NotificationService.SuccessNotification(await LocalizationService.GetResourceAsync("Admin.Customers.CustomerAttributes.Added"));

                if (!continueEditing)
                    return RedirectToAction("List");
                
                return RedirectToAction("Edit", new { id = customerAttribute.Id });
            }
            
            //prepare model
            model = await CustomerAttributeModelFactory.PrepareCustomerAttributeModelAsync(model, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual async Task<IActionResult> Edit(int id)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //try to get a customer attribute with the specified id
            var customerAttribute = await CustomerAttributeService.GetCustomerAttributeByIdAsync(id);
            if (customerAttribute == null)
                return RedirectToAction("List");

            //prepare model
            var model = await CustomerAttributeModelFactory.PrepareCustomerAttributeModelAsync(null, customerAttribute);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> Edit(CustomerAttributeModel model, bool continueEditing)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            var customerAttribute = await CustomerAttributeService.GetCustomerAttributeByIdAsync(model.Id);
            if (customerAttribute == null)
                //no customer attribute found with the specified id
                return RedirectToAction("List");

            if (!ModelState.IsValid)
                //if we got this far, something failed, redisplay form
                return View(model);

            customerAttribute = model.ToEntity(customerAttribute);
            await CustomerAttributeService.UpdateCustomerAttributeAsync(customerAttribute);

            //activity log
            await CustomerActivityService.InsertActivityAsync("EditCustomerAttribute",
                string.Format(await LocalizationService.GetResourceAsync("ActivityLog.EditCustomerAttribute"), customerAttribute.Id),
                customerAttribute);

            //locales
            await UpdateAttributeLocalesAsync(customerAttribute, model);

            NotificationService.SuccessNotification(await LocalizationService.GetResourceAsync("Admin.Customers.CustomerAttributes.Updated"));

            if (!continueEditing)
                return RedirectToAction("List");
            
            return RedirectToAction("Edit", new { id = customerAttribute.Id });
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            var customerAttribute = await CustomerAttributeService.GetCustomerAttributeByIdAsync(id);
            await CustomerAttributeService.DeleteCustomerAttributeAsync(customerAttribute);

            //activity log
            await CustomerActivityService.InsertActivityAsync("DeleteCustomerAttribute",
                string.Format(await LocalizationService.GetResourceAsync("ActivityLog.DeleteCustomerAttribute"), customerAttribute.Id),
                customerAttribute);

            NotificationService.SuccessNotification(await LocalizationService.GetResourceAsync("Admin.Customers.CustomerAttributes.Deleted"));
            return RedirectToAction("List");
        }

        #endregion

        #region Customer attribute values

        [HttpPost]
        public virtual async Task<IActionResult> ValueList(CustomerAttributeValueSearchModel searchModel)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
                return await AccessDeniedDataTablesJson();

            //try to get a customer attribute with the specified id
            var customerAttribute = await CustomerAttributeService.GetCustomerAttributeByIdAsync(searchModel.CustomerAttributeId)
                ?? throw new ArgumentException("No customer attribute found with the specified id");

            //prepare model
            var model = await CustomerAttributeModelFactory.PrepareCustomerAttributeValueListModelAsync(searchModel, customerAttribute);

            return Json(model);
        }

        public virtual async Task<IActionResult> ValueCreatePopup(int customerAttributeId)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //try to get a customer attribute with the specified id
            var customerAttribute = await CustomerAttributeService.GetCustomerAttributeByIdAsync(customerAttributeId);
            if (customerAttribute == null)
                return RedirectToAction("List");

            //prepare model
            var model = await CustomerAttributeModelFactory
                .PrepareCustomerAttributeValueModelAsync(new CustomerAttributeValueModel(), customerAttribute, null);

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> ValueCreatePopup(CustomerAttributeValueModel model)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //try to get a customer attribute with the specified id
            var customerAttribute = await CustomerAttributeService.GetCustomerAttributeByIdAsync(model.CustomerAttributeId);
            if (customerAttribute == null)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                var cav = model.ToEntity<CustomerAttributeValue>();
                await CustomerAttributeService.InsertCustomerAttributeValueAsync(cav);

                //activity log
                await CustomerActivityService.InsertActivityAsync("AddNewCustomerAttributeValue",
                    string.Format(await LocalizationService.GetResourceAsync("ActivityLog.AddNewCustomerAttributeValue"), cav.Id), cav);

                await UpdateValueLocalesAsync(cav, model);

                ViewBag.RefreshPage = true;

                return View(model);
            }

            //prepare model
            model = await CustomerAttributeModelFactory.PrepareCustomerAttributeValueModelAsync(model, customerAttribute, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual async Task<IActionResult> ValueEditPopup(int id)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //try to get a customer attribute value with the specified id
            var customerAttributeValue = await CustomerAttributeService.GetCustomerAttributeValueByIdAsync(id);
            if (customerAttributeValue == null)
                return RedirectToAction("List");

            //try to get a customer attribute with the specified id
            var customerAttribute = await CustomerAttributeService.GetCustomerAttributeByIdAsync(customerAttributeValue.CustomerAttributeId);
            if (customerAttribute == null)
                return RedirectToAction("List");

            //prepare model
            var model = await CustomerAttributeModelFactory.PrepareCustomerAttributeValueModelAsync(null, customerAttribute, customerAttributeValue);

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> ValueEditPopup(CustomerAttributeValueModel model)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //try to get a customer attribute value with the specified id
            var customerAttributeValue = await CustomerAttributeService.GetCustomerAttributeValueByIdAsync(model.Id);
            if (customerAttributeValue == null)
                return RedirectToAction("List");

            //try to get a customer attribute with the specified id
            var customerAttribute = await CustomerAttributeService.GetCustomerAttributeByIdAsync(customerAttributeValue.CustomerAttributeId);
            if (customerAttribute == null)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                customerAttributeValue = model.ToEntity(customerAttributeValue);
                await CustomerAttributeService.UpdateCustomerAttributeValueAsync(customerAttributeValue);

                //activity log
                await CustomerActivityService.InsertActivityAsync("EditCustomerAttributeValue",
                    string.Format(await LocalizationService.GetResourceAsync("ActivityLog.EditCustomerAttributeValue"), customerAttributeValue.Id),
                    customerAttributeValue);

                await UpdateValueLocalesAsync(customerAttributeValue, model);

                ViewBag.RefreshPage = true;

                return View(model);
            }

            //prepare model
            model = await CustomerAttributeModelFactory.PrepareCustomerAttributeValueModelAsync(model, customerAttribute, customerAttributeValue, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> ValueDelete(int id)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //try to get a customer attribute value with the specified id
            var customerAttributeValue = await CustomerAttributeService.GetCustomerAttributeValueByIdAsync(id)
                ?? throw new ArgumentException("No customer attribute value found with the specified id", nameof(id));

            await CustomerAttributeService.DeleteCustomerAttributeValueAsync(customerAttributeValue);

            //activity log
            await CustomerActivityService.InsertActivityAsync("DeleteCustomerAttributeValue",
                string.Format(await LocalizationService.GetResourceAsync("ActivityLog.DeleteCustomerAttributeValue"), customerAttributeValue.Id),
                customerAttributeValue);

            return new NullJsonResult();
        }

        #endregion
    }
}