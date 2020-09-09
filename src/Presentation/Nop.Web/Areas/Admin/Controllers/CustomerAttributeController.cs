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

        private readonly ICustomerActivityService _customerActivityService;
        private readonly ICustomerAttributeModelFactory _customerAttributeModelFactory;
        private readonly ICustomerAttributeService _customerAttributeService;
        private readonly ILocalizationService _localizationService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly INotificationService _notificationService;
        private readonly IPermissionService _permissionService;

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
            _customerActivityService = customerActivityService;
            _customerAttributeModelFactory = customerAttributeModelFactory;
            _customerAttributeService = customerAttributeService;
            _localizationService = localizationService;
            _localizedEntityService = localizedEntityService;
            _notificationService = notificationService;
            _permissionService = permissionService;
        }

        #endregion

        #region Utilities

        protected virtual async Task UpdateAttributeLocales(CustomerAttribute customerAttribute, CustomerAttributeModel model)
        {
            foreach (var localized in model.Locales)
            {
                await _localizedEntityService.SaveLocalizedValue(customerAttribute,
                    x => x.Name,
                    localized.Name,
                    localized.LanguageId);
            }
        }

        protected virtual async Task UpdateValueLocales(CustomerAttributeValue customerAttributeValue, CustomerAttributeValueModel model)
        {
            foreach (var localized in model.Locales)
            {
                await _localizedEntityService.SaveLocalizedValue(customerAttributeValue,
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
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //select an appropriate panel
            SaveSelectedPanelName("customersettings-customerformfields");

            //we just redirect a user to the customer settings page
            return RedirectToAction("CustomerUser", "Setting");
        }

        [HttpPost]
        public virtual async Task<IActionResult> List(CustomerAttributeSearchModel searchModel)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedDataTablesJson();

            //prepare model
            var model = _customerAttributeModelFactory.PrepareCustomerAttributeListModel(searchModel);

            return Json(model);
        }

        public virtual async Task<IActionResult> Create()
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //prepare model
            var model = await _customerAttributeModelFactory.PrepareCustomerAttributeModel(new CustomerAttributeModel(), null);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> Create(CustomerAttributeModel model, bool continueEditing)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var customerAttribute = model.ToEntity<CustomerAttribute>();
                await _customerAttributeService.InsertCustomerAttribute(customerAttribute);

                //activity log
                await _customerActivityService.InsertActivity("AddNewCustomerAttribute",
                    string.Format(await _localizationService.GetResource("ActivityLog.AddNewCustomerAttribute"), customerAttribute.Id),
                    customerAttribute);

                //locales
                await UpdateAttributeLocales(customerAttribute, model);

                _notificationService.SuccessNotification(await _localizationService.GetResource("Admin.Customers.CustomerAttributes.Added"));

                if (!continueEditing)
                    return RedirectToAction("List");
                
                return RedirectToAction("Edit", new { id = customerAttribute.Id });
            }
            
            //prepare model
            model = await _customerAttributeModelFactory.PrepareCustomerAttributeModel(model, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual async Task<IActionResult> Edit(int id)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //try to get a customer attribute with the specified id
            var customerAttribute = await _customerAttributeService.GetCustomerAttributeById(id);
            if (customerAttribute == null)
                return RedirectToAction("List");

            //prepare model
            var model = await _customerAttributeModelFactory.PrepareCustomerAttributeModel(null, customerAttribute);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> Edit(CustomerAttributeModel model, bool continueEditing)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            var customerAttribute = await _customerAttributeService.GetCustomerAttributeById(model.Id);
            if (customerAttribute == null)
                //no customer attribute found with the specified id
                return RedirectToAction("List");

            if (!ModelState.IsValid)
                //if we got this far, something failed, redisplay form
                return View(model);

            customerAttribute = model.ToEntity(customerAttribute);
            await _customerAttributeService.UpdateCustomerAttribute(customerAttribute);

            //activity log
            await _customerActivityService.InsertActivity("EditCustomerAttribute",
                string.Format(await _localizationService.GetResource("ActivityLog.EditCustomerAttribute"), customerAttribute.Id),
                customerAttribute);

            //locales
            await UpdateAttributeLocales(customerAttribute, model);

            _notificationService.SuccessNotification(await _localizationService.GetResource("Admin.Customers.CustomerAttributes.Updated"));

            if (!continueEditing)
                return RedirectToAction("List");
            
            return RedirectToAction("Edit", new { id = customerAttribute.Id });
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            var customerAttribute = await _customerAttributeService.GetCustomerAttributeById(id);
            await _customerAttributeService.DeleteCustomerAttribute(customerAttribute);

            //activity log
            await _customerActivityService.InsertActivity("DeleteCustomerAttribute",
                string.Format(await _localizationService.GetResource("ActivityLog.DeleteCustomerAttribute"), customerAttribute.Id),
                customerAttribute);

            _notificationService.SuccessNotification(await _localizationService.GetResource("Admin.Customers.CustomerAttributes.Deleted"));
            return RedirectToAction("List");
        }

        #endregion

        #region Customer attribute values

        [HttpPost]
        public virtual async Task<IActionResult> ValueList(CustomerAttributeValueSearchModel searchModel)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedDataTablesJson();

            //try to get a customer attribute with the specified id
            var customerAttribute = await _customerAttributeService.GetCustomerAttributeById(searchModel.CustomerAttributeId)
                ?? throw new ArgumentException("No customer attribute found with the specified id");

            //prepare model
            var model = await _customerAttributeModelFactory.PrepareCustomerAttributeValueListModel(searchModel, customerAttribute);

            return Json(model);
        }

        public virtual async Task<IActionResult> ValueCreatePopup(int customerAttributeId)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //try to get a customer attribute with the specified id
            var customerAttribute = await _customerAttributeService.GetCustomerAttributeById(customerAttributeId);
            if (customerAttribute == null)
                return RedirectToAction("List");

            //prepare model
            var model = await _customerAttributeModelFactory
                .PrepareCustomerAttributeValueModel(new CustomerAttributeValueModel(), customerAttribute, null);

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> ValueCreatePopup(CustomerAttributeValueModel model)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //try to get a customer attribute with the specified id
            var customerAttribute = await _customerAttributeService.GetCustomerAttributeById(model.CustomerAttributeId);
            if (customerAttribute == null)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                var cav = model.ToEntity<CustomerAttributeValue>();
                await _customerAttributeService.InsertCustomerAttributeValue(cav);

                //activity log
                await _customerActivityService.InsertActivity("AddNewCustomerAttributeValue",
                    string.Format(await _localizationService.GetResource("ActivityLog.AddNewCustomerAttributeValue"), cav.Id), cav);

                await UpdateValueLocales(cav, model);

                ViewBag.RefreshPage = true;

                return View(model);
            }

            //prepare model
            model = await _customerAttributeModelFactory.PrepareCustomerAttributeValueModel(model, customerAttribute, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual async Task<IActionResult> ValueEditPopup(int id)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //try to get a customer attribute value with the specified id
            var customerAttributeValue = await _customerAttributeService.GetCustomerAttributeValueById(id);
            if (customerAttributeValue == null)
                return RedirectToAction("List");

            //try to get a customer attribute with the specified id
            var customerAttribute = await _customerAttributeService.GetCustomerAttributeById(customerAttributeValue.CustomerAttributeId);
            if (customerAttribute == null)
                return RedirectToAction("List");

            //prepare model
            var model = await _customerAttributeModelFactory.PrepareCustomerAttributeValueModel(null, customerAttribute, customerAttributeValue);

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> ValueEditPopup(CustomerAttributeValueModel model)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //try to get a customer attribute value with the specified id
            var customerAttributeValue = await _customerAttributeService.GetCustomerAttributeValueById(model.Id);
            if (customerAttributeValue == null)
                return RedirectToAction("List");

            //try to get a customer attribute with the specified id
            var customerAttribute = await _customerAttributeService.GetCustomerAttributeById(customerAttributeValue.CustomerAttributeId);
            if (customerAttribute == null)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                customerAttributeValue = model.ToEntity(customerAttributeValue);
                await _customerAttributeService.UpdateCustomerAttributeValue(customerAttributeValue);

                //activity log
                await _customerActivityService.InsertActivity("EditCustomerAttributeValue",
                    string.Format(await _localizationService.GetResource("ActivityLog.EditCustomerAttributeValue"), customerAttributeValue.Id),
                    customerAttributeValue);

                await UpdateValueLocales(customerAttributeValue, model);

                ViewBag.RefreshPage = true;

                return View(model);
            }

            //prepare model
            model = await _customerAttributeModelFactory.PrepareCustomerAttributeValueModel(model, customerAttribute, customerAttributeValue, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> ValueDelete(int id)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //try to get a customer attribute value with the specified id
            var customerAttributeValue = await _customerAttributeService.GetCustomerAttributeValueById(id)
                ?? throw new ArgumentException("No customer attribute value found with the specified id", nameof(id));

            await _customerAttributeService.DeleteCustomerAttributeValue(customerAttributeValue);

            //activity log
            await _customerActivityService.InsertActivity("DeleteCustomerAttributeValue",
                string.Format(await _localizationService.GetResource("ActivityLog.DeleteCustomerAttributeValue"), customerAttributeValue.Id),
                customerAttributeValue);

            return new NullJsonResult();
        }

        #endregion
    }
}