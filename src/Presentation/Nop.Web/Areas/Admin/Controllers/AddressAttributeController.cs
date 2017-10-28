using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Nop.Web.Areas.Admin.Extensions;
using Nop.Web.Areas.Admin.Models.Common;
using Nop.Core;
using Nop.Core.Domain.Common;
using Nop.Services.Common;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Security;
using Nop.Web.Framework.Kendoui;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Web.Areas.Admin.Controllers
{
    public partial class AddressAttributeController : BaseAdminController
    {
        #region Fields

        private readonly IAddressAttributeService _addressAttributeService;
        private readonly ILanguageService _languageService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;
        private readonly IPermissionService _permissionService;
        private readonly ICustomerActivityService _customerActivityService;

        #endregion

        #region Ctor

        public AddressAttributeController(IAddressAttributeService addressAttributeService,
            ILanguageService languageService, 
            ILocalizedEntityService localizedEntityService,
            ILocalizationService localizationService,
            IWorkContext workContext,
            IPermissionService permissionService,
            ICustomerActivityService customerActivityService)
        {
            this._addressAttributeService = addressAttributeService;
            this._languageService = languageService;
            this._localizedEntityService = localizedEntityService;
            this._localizationService = localizationService;
            this._workContext = workContext;
            this._permissionService = permissionService;
            this._customerActivityService = customerActivityService;
        }

        #endregion
        
        #region Utilities

        protected virtual void UpdateAttributeLocales(AddressAttribute addressAttribute, AddressAttributeModel model)
        {
            foreach (var localized in model.Locales)
            {
                _localizedEntityService.SaveLocalizedValue(addressAttribute,
                    x => x.Name,
                    localized.Name,
                    localized.LanguageId);
            }
        }

        protected virtual void UpdateValueLocales(AddressAttributeValue addressAttributeValue, AddressAttributeValueModel model)
        {
            foreach (var localized in model.Locales)
            {
                _localizedEntityService.SaveLocalizedValue(addressAttributeValue,
                    x => x.Name,
                    localized.Name,
                    localized.LanguageId);
            }
        }

        #endregion

        #region Address attributes

        public virtual IActionResult Index()
        {
            return RedirectToAction("List");
        }

        public virtual IActionResult ListBlock()
        {
            return PartialView("ListBlock");
        }

        public virtual IActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //we just redirect a user to the address settings page
             
            //select "address form fields" tab
            SaveSelectedTabName("tab-addressformfields");
            return RedirectToAction("CustomerUser", "Setting");
        }

        [HttpPost]
        public virtual IActionResult List(DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedKendoGridJson();

            var addressAttributes = _addressAttributeService.GetAllAddressAttributes();
            var gridModel = new DataSourceResult
            {
                Data = addressAttributes.Select(x =>
                {
                    var attributeModel = x.ToModel();
                    attributeModel.AttributeControlTypeName = x.AttributeControlType.GetLocalizedEnum(_localizationService, _workContext);
                    return attributeModel;
                }),
                Total = addressAttributes.Count()
            };
            return Json(gridModel);
        }

        //create
        public virtual IActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            var model = new AddressAttributeModel();
            //locales
            AddLocales(_languageService, model.Locales);
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult Create(AddressAttributeModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var addressAttribute = model.ToEntity();
                _addressAttributeService.InsertAddressAttribute(addressAttribute);

                //activity log
                _customerActivityService.InsertActivity("AddNewAddressAttribute", _localizationService.GetResource("ActivityLog.AddNewAddressAttribute"), addressAttribute.Id);

                //locales
                UpdateAttributeLocales(addressAttribute, model);

                SuccessNotification(_localizationService.GetResource("Admin.Address.AddressAttributes.Added"));

                if (continueEditing)
                {
                    //selected tab
                    SaveSelectedTabName();

                    return RedirectToAction("Edit", new { id = addressAttribute.Id });
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

            var addressAttribute = _addressAttributeService.GetAddressAttributeById(id);
            if (addressAttribute == null)
                //No address attribute found with the specified id
                return RedirectToAction("List");

            var model = addressAttribute.ToModel();
            //locales
            AddLocales(_languageService, model.Locales, (locale, languageId) =>
            {
                locale.Name = addressAttribute.GetLocalized(x => x.Name, languageId, false, false);
            });
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult Edit(AddressAttributeModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            var addressAttribute = _addressAttributeService.GetAddressAttributeById(model.Id);
            if (addressAttribute == null)
                //No address attribute found with the specified id
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                addressAttribute = model.ToEntity(addressAttribute);
                _addressAttributeService.UpdateAddressAttribute(addressAttribute);

                //activity log
                _customerActivityService.InsertActivity("EditAddressAttribute", _localizationService.GetResource("ActivityLog.EditAddressAttribute"), addressAttribute.Id);

                //locales
                UpdateAttributeLocales(addressAttribute, model);

                SuccessNotification(_localizationService.GetResource("Admin.Address.AddressAttributes.Updated"));
                if (continueEditing)
                {
                    //selected tab
                    SaveSelectedTabName();

                    return RedirectToAction("Edit", new {id = addressAttribute.Id});
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

            var addressAttribute = _addressAttributeService.GetAddressAttributeById(id);
            _addressAttributeService.DeleteAddressAttribute(addressAttribute);

            //activity log
            _customerActivityService.InsertActivity("DeleteAddressAttribute", _localizationService.GetResource("ActivityLog.DeleteAddressAttribute"), addressAttribute.Id);

            SuccessNotification(_localizationService.GetResource("Admin.Address.AddressAttributes.Deleted"));
            return RedirectToAction("List");
        }

        #endregion

        #region Address attribute values

        //list
        [HttpPost]
        public virtual IActionResult ValueList(int addressAttributeId, DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedKendoGridJson();

            var values = _addressAttributeService.GetAddressAttributeValues(addressAttributeId);
            var gridModel = new DataSourceResult
            {
                Data = values.Select(x => new AddressAttributeValueModel
                {
                    Id = x.Id,
                    AddressAttributeId = x.AddressAttributeId,
                    Name = x.Name,
                    IsPreSelected = x.IsPreSelected,
                    DisplayOrder = x.DisplayOrder,
                }),
                Total = values.Count()
            };
            return Json(gridModel);
        }

        //create
        public virtual IActionResult ValueCreatePopup(int addressAttributeId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            var addressAttribute = _addressAttributeService.GetAddressAttributeById(addressAttributeId);
            if (addressAttribute == null)
                //No address attribute found with the specified id
                return RedirectToAction("List");

            var model = new AddressAttributeValueModel
            {
                AddressAttributeId = addressAttributeId
            };
            //locales
            AddLocales(_languageService, model.Locales);
            return View(model);
        }

        [HttpPost]
        public virtual IActionResult ValueCreatePopup(AddressAttributeValueModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            var addressAttribute = _addressAttributeService.GetAddressAttributeById(model.AddressAttributeId);
            if (addressAttribute == null)
                //No address attribute found with the specified id
                return RedirectToAction("List");
            
            if (ModelState.IsValid)
            {
                var cav = new AddressAttributeValue
                {
                    AddressAttributeId = model.AddressAttributeId,
                    Name = model.Name,
                    IsPreSelected = model.IsPreSelected,
                    DisplayOrder = model.DisplayOrder
                };

                _addressAttributeService.InsertAddressAttributeValue(cav);

                //activity log
                _customerActivityService.InsertActivity("AddNewAddressAttributeValue", _localizationService.GetResource("ActivityLog.AddNewAddressAttributeValue"), cav.Id);
                
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

            var cav = _addressAttributeService.GetAddressAttributeValueById(id);
            if (cav == null)
                //No address attribute value found with the specified id
                return RedirectToAction("List");

            var model = new AddressAttributeValueModel
            {
                AddressAttributeId = cav.AddressAttributeId,
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
        public virtual IActionResult ValueEditPopup( AddressAttributeValueModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            var cav = _addressAttributeService.GetAddressAttributeValueById(model.Id);
            if (cav == null)
                //No address attribute value found with the specified id
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                cav.Name = model.Name;
                cav.IsPreSelected = model.IsPreSelected;
                cav.DisplayOrder = model.DisplayOrder;
                _addressAttributeService.UpdateAddressAttributeValue(cav);

                UpdateValueLocales(cav, model);

                //activity log
                _customerActivityService.InsertActivity("EditAddressAttributeValue", _localizationService.GetResource("ActivityLog.EditAddressAttributeValue"), cav.Id);
                
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

            var cav = _addressAttributeService.GetAddressAttributeValueById(id);
            if (cav == null)
                throw new ArgumentException("No address attribute value found with the specified id");
            _addressAttributeService.DeleteAddressAttributeValue(cav);
            
            //activity log
            _customerActivityService.InsertActivity("DeleteAddressAttributeValue", _localizationService.GetResource("ActivityLog.DeleteAddressAttributeValue"), cav.Id);
            
            return new NullJsonResult();
        }

        #endregion
    }
}