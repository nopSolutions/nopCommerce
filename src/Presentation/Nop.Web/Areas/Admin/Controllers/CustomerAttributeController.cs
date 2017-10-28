using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Nop.Web.Areas.Admin.Extensions;
using Nop.Web.Areas.Admin.Models.Customers;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Security;
using Nop.Web.Framework.Kendoui;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Web.Areas.Admin.Controllers
{
    public partial class CustomerAttributeController : BaseAdminController
    {
        #region Fields

        private readonly ICustomerAttributeService _customerAttributeService;
        private readonly ILanguageService _languageService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;
        private readonly IPermissionService _permissionService;
        private readonly ICustomerActivityService _customerActivityService;

        #endregion

        #region Ctor

        public CustomerAttributeController(ICustomerAttributeService customerAttributeService,
            ILanguageService languageService, 
            ILocalizedEntityService localizedEntityService,
            ILocalizationService localizationService,
            IWorkContext workContext,
            IPermissionService permissionService,
            ICustomerActivityService customerActivityService)
        {
            this._customerAttributeService = customerAttributeService;
            this._languageService = languageService;
            this._localizedEntityService = localizedEntityService;
            this._localizationService = localizationService;
            this._workContext = workContext;
            this._permissionService = permissionService;
            this._customerActivityService = customerActivityService;
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

            //we just redirect a user to the customer settings page

            //select "customer form fields" tab
            SaveSelectedTabName("tab-customerformfields");
            return RedirectToAction("CustomerUser", "Setting");
        }

        [HttpPost]
        public virtual IActionResult List(DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedKendoGridJson();

            var customerAttributes = _customerAttributeService.GetAllCustomerAttributes();
            var gridModel = new DataSourceResult
            {
                Data = customerAttributes.Select(x =>
                {
                    var attributeModel = x.ToModel();
                    attributeModel.AttributeControlTypeName = x.AttributeControlType.GetLocalizedEnum(_localizationService, _workContext);
                    return attributeModel;
                }),
                Total = customerAttributes.Count()
            };
            return Json(gridModel);
        }
        
        //create
        public virtual IActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            var model = new CustomerAttributeModel();
            //locales
            AddLocales(_languageService, model.Locales);
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult Create(CustomerAttributeModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var customerAttribute = model.ToEntity();
                _customerAttributeService.InsertCustomerAttribute(customerAttribute);

                //activity log
                _customerActivityService.InsertActivity("AddNewCustomerAttribute", _localizationService.GetResource("ActivityLog.AddNewCustomerAttribute"), customerAttribute.Id);

                //locales
                UpdateAttributeLocales(customerAttribute, model);

                SuccessNotification(_localizationService.GetResource("Admin.Customers.CustomerAttributes.Added"));

                if (continueEditing)
                {
                    //selected tab
                    SaveSelectedTabName();

                    return RedirectToAction("Edit", new { id = customerAttribute.Id });
                }
                return RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }

        //edit
        public virtual IActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            var customerAttribute = _customerAttributeService.GetCustomerAttributeById(id);
            if (customerAttribute == null)
                //No customer attribute found with the specified id
                return RedirectToAction("List");

            var model = customerAttribute.ToModel();
            //locales
            AddLocales(_languageService, model.Locales, (locale, languageId) =>
            {
                locale.Name = customerAttribute.GetLocalized(x => x.Name, languageId, false, false);
            });
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult Edit(CustomerAttributeModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            var customerAttribute = _customerAttributeService.GetCustomerAttributeById(model.Id);
            if (customerAttribute == null)
                //No customer attribute found with the specified id
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                customerAttribute = model.ToEntity(customerAttribute);
                _customerAttributeService.UpdateCustomerAttribute(customerAttribute);

                //activity log
                _customerActivityService.InsertActivity("EditCustomerAttribute", _localizationService.GetResource("ActivityLog.EditCustomerAttribute"), customerAttribute.Id);

                //locales
                UpdateAttributeLocales(customerAttribute, model);

                SuccessNotification(_localizationService.GetResource("Admin.Customers.CustomerAttributes.Updated"));
                if (continueEditing)
                {
                    //selected tab
                    SaveSelectedTabName();

                    return RedirectToAction("Edit", new {id = customerAttribute.Id});
                }
                return RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }

        //delete
        [HttpPost]
        public virtual IActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            var customerAttribute = _customerAttributeService.GetCustomerAttributeById(id);
            _customerAttributeService.DeleteCustomerAttribute(customerAttribute);

            //activity log
            _customerActivityService.InsertActivity("DeleteCustomerAttribute", _localizationService.GetResource("ActivityLog.DeleteCustomerAttribute"), customerAttribute.Id);

            SuccessNotification(_localizationService.GetResource("Admin.Customers.CustomerAttributes.Deleted"));
            return RedirectToAction("List");
        }

        #endregion

        #region Customer attribute values

        //list
        [HttpPost]
        public virtual IActionResult ValueList(int customerAttributeId, DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedKendoGridJson();

            var values = _customerAttributeService.GetCustomerAttributeValues(customerAttributeId);
            var gridModel = new DataSourceResult
            {
                Data = values.Select(x => new CustomerAttributeValueModel
                {
                    Id = x.Id,
                    CustomerAttributeId = x.CustomerAttributeId,
                    Name = x.Name,
                    IsPreSelected = x.IsPreSelected,
                    DisplayOrder = x.DisplayOrder,
                }),
                Total = values.Count()
            };
            return Json(gridModel);
        }

        //create
        public virtual IActionResult ValueCreatePopup(int customerAttributeId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            var customerAttribute = _customerAttributeService.GetCustomerAttributeById(customerAttributeId);
            if (customerAttribute == null)
                //No customer attribute found with the specified id
                return RedirectToAction("List");

            var model = new CustomerAttributeValueModel
            {
                CustomerAttributeId = customerAttributeId
            };
            //locales
            AddLocales(_languageService, model.Locales);
            return View(model);
        }

        [HttpPost]
        public virtual IActionResult ValueCreatePopup(CustomerAttributeValueModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            var customerAttribute = _customerAttributeService.GetCustomerAttributeById(model.CustomerAttributeId);
            if (customerAttribute == null)
                //No customer attribute found with the specified id
                return RedirectToAction("List");
            
            if (ModelState.IsValid)
            {
                var cav = new CustomerAttributeValue
                {
                    CustomerAttributeId = model.CustomerAttributeId,
                    Name = model.Name,
                    IsPreSelected = model.IsPreSelected,
                    DisplayOrder = model.DisplayOrder
                };

                _customerAttributeService.InsertCustomerAttributeValue(cav);

                //activity log
                _customerActivityService.InsertActivity("AddNewCustomerAttributeValue", _localizationService.GetResource("ActivityLog.AddNewCustomerAttributeValue"), cav.Id);

                UpdateValueLocales(cav, model);

                ViewBag.RefreshPage = true;
                return View(model);
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }

        //edit
        public virtual IActionResult ValueEditPopup(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            var cav = _customerAttributeService.GetCustomerAttributeValueById(id);
            if (cav == null)
                //No customer attribute value found with the specified id
                return RedirectToAction("List");

            var model = new CustomerAttributeValueModel
            {
                CustomerAttributeId = cav.CustomerAttributeId,
                Name = cav.Name,
                IsPreSelected = cav.IsPreSelected,
                DisplayOrder = cav.DisplayOrder
            };

            //locales
            AddLocales(_languageService, model.Locales, (locale, languageId) =>
            {
                locale.Name = cav.GetLocalized(x => x.Name, languageId, false, false);
            });

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult ValueEditPopup(CustomerAttributeValueModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            var cav = _customerAttributeService.GetCustomerAttributeValueById(model.Id);
            if (cav == null)
                //No customer attribute value found with the specified id
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                cav.Name = model.Name;
                cav.IsPreSelected = model.IsPreSelected;
                cav.DisplayOrder = model.DisplayOrder;
                _customerAttributeService.UpdateCustomerAttributeValue(cav);

                //activity log
                _customerActivityService.InsertActivity("EditCustomerAttributeValue", _localizationService.GetResource("ActivityLog.EditCustomerAttributeValue"), cav.Id);

                UpdateValueLocales(cav, model);

                ViewBag.RefreshPage = true;
                return View(model);
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }

        //delete
        [HttpPost]
        public virtual IActionResult ValueDelete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            var cav = _customerAttributeService.GetCustomerAttributeValueById(id);
            if (cav == null)
                throw new ArgumentException("No customer attribute value found with the specified id");
            _customerAttributeService.DeleteCustomerAttributeValue(cav);

            //activity log
            _customerActivityService.InsertActivity("DeleteCustomerAttributeValue", _localizationService.GetResource("ActivityLog.DeleteCustomerAttributeValue"), cav.Id);

            return new NullJsonResult();
        }
        
        #endregion
    }
}