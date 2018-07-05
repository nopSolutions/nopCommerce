using System;
using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Customers;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Logging;
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
        private readonly IPermissionService _permissionService;

        #endregion

        #region Ctor

        public CustomerAttributeController(ICustomerActivityService customerActivityService,
            ICustomerAttributeModelFactory customerAttributeModelFactory,
            ICustomerAttributeService customerAttributeService,
            ILocalizationService localizationService,
            ILocalizedEntityService localizedEntityService,
            IPermissionService permissionService)
        {
            this._customerActivityService = customerActivityService;
            this._customerAttributeModelFactory = customerAttributeModelFactory;
            this._customerAttributeService = customerAttributeService;
            this._localizationService = localizationService;
            this._localizedEntityService = localizedEntityService;
            this._permissionService = permissionService;
        }

        #endregion

        #region Utilities

        protected virtual void UpdateAttributeLocales(CustomerAttribute customerAttribute, CustomerAttributeModel model)
        {
            foreach (var localized in model.Locales)
            {
                _localizedEntityService.SaveLocalizedValue(customerAttribute,
                    x => x.Name,
                    localized.Name,
                    localized.LanguageId);
            }
        }

        protected virtual void UpdateValueLocales(CustomerAttributeValue customerAttributeValue, CustomerAttributeValueModel model)
        {
            foreach (var localized in model.Locales)
            {
                _localizedEntityService.SaveLocalizedValue(customerAttributeValue,
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

        public virtual IActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //select "customer form fields" tab
            SaveSelectedTabName("tab-customerformfields");

            //we just redirect a user to the customer settings page
            return RedirectToAction("CustomerUser", "Setting");
        }

        [HttpPost]
        public virtual IActionResult List(CustomerAttributeSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedKendoGridJson();

            //prepare model
            var model = _customerAttributeModelFactory.PrepareCustomerAttributeListModel(searchModel);

            return Json(model);
        }

        public virtual IActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //prepare model
            var model = _customerAttributeModelFactory.PrepareCustomerAttributeModel(new CustomerAttributeModel(), null);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult Create(CustomerAttributeModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var customerAttribute = model.ToEntity<CustomerAttribute>();
                _customerAttributeService.InsertCustomerAttribute(customerAttribute);

                //activity log
                _customerActivityService.InsertActivity("AddNewCustomerAttribute",
                    string.Format(_localizationService.GetResource("ActivityLog.AddNewCustomerAttribute"), customerAttribute.Id),
                    customerAttribute);

                //locales
                UpdateAttributeLocales(customerAttribute, model);

                SuccessNotification(_localizationService.GetResource("Admin.Customers.CustomerAttributes.Added"));

                if (!continueEditing)
                    return RedirectToAction("List");

                //selected tab
                SaveSelectedTabName();

                return RedirectToAction("Edit", new { id = customerAttribute.Id });
            }
            
            //prepare model
            model = _customerAttributeModelFactory.PrepareCustomerAttributeModel(model, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual IActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //try to get a customer attribute with the specified id
            var customerAttribute = _customerAttributeService.GetCustomerAttributeById(id);
            if (customerAttribute == null)
                return RedirectToAction("List");

            //prepare model
            var model = _customerAttributeModelFactory.PrepareCustomerAttributeModel(null, customerAttribute);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult Edit(CustomerAttributeModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            var customerAttribute = _customerAttributeService.GetCustomerAttributeById(model.Id);
            if (customerAttribute == null)
                //no customer attribute found with the specified id
                return RedirectToAction("List");

            if (!ModelState.IsValid)
                //if we got this far, something failed, redisplay form
                return View(model);

            customerAttribute = model.ToEntity(customerAttribute);
            _customerAttributeService.UpdateCustomerAttribute(customerAttribute);

            //activity log
            _customerActivityService.InsertActivity("EditCustomerAttribute",
                string.Format(_localizationService.GetResource("ActivityLog.EditCustomerAttribute"), customerAttribute.Id),
                customerAttribute);

            //locales
            UpdateAttributeLocales(customerAttribute, model);

            SuccessNotification(_localizationService.GetResource("Admin.Customers.CustomerAttributes.Updated"));

            if (!continueEditing)
                return RedirectToAction("List");

            //selected tab
            SaveSelectedTabName();

            return RedirectToAction("Edit", new { id = customerAttribute.Id });
        }

        [HttpPost]
        public virtual IActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            var customerAttribute = _customerAttributeService.GetCustomerAttributeById(id);
            _customerAttributeService.DeleteCustomerAttribute(customerAttribute);

            //activity log
            _customerActivityService.InsertActivity("DeleteCustomerAttribute",
                string.Format(_localizationService.GetResource("ActivityLog.DeleteCustomerAttribute"), customerAttribute.Id),
                customerAttribute);

            SuccessNotification(_localizationService.GetResource("Admin.Customers.CustomerAttributes.Deleted"));
            return RedirectToAction("List");
        }

        #endregion

        #region Customer attribute values

        [HttpPost]
        public virtual IActionResult ValueList(CustomerAttributeValueSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedKendoGridJson();

            //try to get a customer attribute with the specified id
            var customerAttribute = _customerAttributeService.GetCustomerAttributeById(searchModel.CustomerAttributeId)
                ?? throw new ArgumentException("No customer attribute found with the specified id");

            //prepare model
            var model = _customerAttributeModelFactory.PrepareCustomerAttributeValueListModel(searchModel, customerAttribute);

            return Json(model);
        }

        public virtual IActionResult ValueCreatePopup(int customerAttributeId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //try to get a customer attribute with the specified id
            var customerAttribute = _customerAttributeService.GetCustomerAttributeById(customerAttributeId);
            if (customerAttribute == null)
                return RedirectToAction("List");

            //prepare model
            var model = _customerAttributeModelFactory
                .PrepareCustomerAttributeValueModel(new CustomerAttributeValueModel(), customerAttribute, null);

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult ValueCreatePopup(CustomerAttributeValueModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //try to get a customer attribute with the specified id
            var customerAttribute = _customerAttributeService.GetCustomerAttributeById(model.CustomerAttributeId);
            if (customerAttribute == null)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                var cav = model.ToEntity<CustomerAttributeValue>();
                _customerAttributeService.InsertCustomerAttributeValue(cav);

                //activity log
                _customerActivityService.InsertActivity("AddNewCustomerAttributeValue",
                    string.Format(_localizationService.GetResource("ActivityLog.AddNewCustomerAttributeValue"), cav.Id), cav);

                UpdateValueLocales(cav, model);

                ViewBag.RefreshPage = true;

                return View(model);
            }

            //prepare model
            model = _customerAttributeModelFactory.PrepareCustomerAttributeValueModel(model, customerAttribute, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual IActionResult ValueEditPopup(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //try to get a customer attribute value with the specified id
            var customerAttributeValue = _customerAttributeService.GetCustomerAttributeValueById(id);
            if (customerAttributeValue == null)
                return RedirectToAction("List");

            //try to get a customer attribute with the specified id
            var customerAttribute = _customerAttributeService.GetCustomerAttributeById(customerAttributeValue.CustomerAttributeId);
            if (customerAttribute == null)
                return RedirectToAction("List");

            //prepare model
            var model = _customerAttributeModelFactory.PrepareCustomerAttributeValueModel(null, customerAttribute, customerAttributeValue);

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult ValueEditPopup(CustomerAttributeValueModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //try to get a customer attribute value with the specified id
            var customerAttributeValue = _customerAttributeService.GetCustomerAttributeValueById(model.Id);
            if (customerAttributeValue == null)
                return RedirectToAction("List");

            //try to get a customer attribute with the specified id
            var customerAttribute = _customerAttributeService.GetCustomerAttributeById(customerAttributeValue.CustomerAttributeId);
            if (customerAttribute == null)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                customerAttributeValue = model.ToEntity(customerAttributeValue);
                _customerAttributeService.UpdateCustomerAttributeValue(customerAttributeValue);

                //activity log
                _customerActivityService.InsertActivity("EditCustomerAttributeValue",
                    string.Format(_localizationService.GetResource("ActivityLog.EditCustomerAttributeValue"), customerAttributeValue.Id),
                    customerAttributeValue);

                UpdateValueLocales(customerAttributeValue, model);

                ViewBag.RefreshPage = true;

                return View(model);
            }

            //prepare model
            model = _customerAttributeModelFactory.PrepareCustomerAttributeValueModel(model, customerAttribute, customerAttributeValue, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual IActionResult ValueDelete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //try to get a customer attribute value with the specified id
            var customerAttributeValue = _customerAttributeService.GetCustomerAttributeValueById(id)
                ?? throw new ArgumentException("No customer attribute value found with the specified id", nameof(id));

            _customerAttributeService.DeleteCustomerAttributeValue(customerAttributeValue);

            //activity log
            _customerActivityService.InsertActivity("DeleteCustomerAttributeValue",
                string.Format(_localizationService.GetResource("ActivityLog.DeleteCustomerAttributeValue"), customerAttributeValue.Id),
                customerAttributeValue);

            return new NullJsonResult();
        }

        #endregion
    }
}