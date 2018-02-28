using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Vendors;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Security;
using Nop.Services.Vendors;
using Nop.Web.Areas.Admin.Extensions;
using Nop.Web.Areas.Admin.Models.Vendors;
using Nop.Web.Framework.Kendoui;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Web.Areas.Admin.Controllers
{
    public partial class VendorAttributeController : BaseAdminController
    {
        #region Fields

        private readonly ICustomerActivityService _customerActivityService;
        private readonly ILanguageService _languageService;
        private readonly ILocalizationService _localizationService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly IPermissionService _permissionService;
        private readonly IVendorAttributeService _vendorAttributeService;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public VendorAttributeController(ICustomerActivityService customerActivityService,
            ILanguageService languageService, 
            ILocalizationService localizationService,
            ILocalizedEntityService localizedEntityService,
            IPermissionService permissionService,
            IVendorAttributeService vendorAttributeService,
            IWorkContext workContext)
        {
            this._customerActivityService = customerActivityService;
            this._languageService = languageService;
            this._localizationService = localizationService;
            this._localizedEntityService = localizedEntityService;
            this._permissionService = permissionService;
            this._vendorAttributeService = vendorAttributeService;
            this._workContext = workContext;
        }

        #endregion
        
        #region Utilities

        protected virtual void UpdateAttributeLocales(VendorAttribute vendorAttribute, VendorAttributeModel model)
        {
            foreach (var localized in model.Locales)
            {
                _localizedEntityService.SaveLocalizedValue(vendorAttribute,
                    x => x.Name,
                    localized.Name,
                    localized.LanguageId);
            }
        }

        protected virtual void UpdateValueLocales(VendorAttributeValue vendorAttributeValue, VendorAttributeValueModel model)
        {
            foreach (var localized in model.Locales)
            {
                _localizedEntityService.SaveLocalizedValue(vendorAttributeValue,
                    x => x.Name,
                    localized.Name,
                    localized.LanguageId);
            }
        }

        #endregion
        
        #region Vendor attributes

        public virtual IActionResult Index()
        {
            return RedirectToAction("List");
        }

        public virtual IActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();
            
            return RedirectToAction("Vendor", "Setting");
        }

        [HttpPost]
        public virtual IActionResult List(DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedKendoGridJson();

            var vendorAttributes = _vendorAttributeService.GetAllVendorAttributes();
            var gridModel = new DataSourceResult
            {
                Data = vendorAttributes.Select(x =>
                {
                    var attributeModel = x.ToModel();
                    attributeModel.AttributeControlTypeName = x.AttributeControlType.GetLocalizedEnum(_localizationService, _workContext);
                    return attributeModel;
                }),
                Total = vendorAttributes.Count()
            };
            return Json(gridModel);
        }
        
        //create
        public virtual IActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            var model = new VendorAttributeModel();
            //locales
            AddLocales(_languageService, model.Locales);
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult Create(VendorAttributeModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var vendorAttribute = model.ToEntity();
                _vendorAttributeService.InsertVendorAttribute(vendorAttribute);

                //activity log
                _customerActivityService.InsertActivity("AddNewVendorAttribute",
                    string.Format(_localizationService.GetResource("ActivityLog.AddNewVendorAttribute"), vendorAttribute.Id), vendorAttribute);

                //locales
                UpdateAttributeLocales(vendorAttribute, model);

                SuccessNotification(_localizationService.GetResource("Admin.Vendors.VendorAttributes.Added"));

                if (continueEditing)
                {
                    //selected tab
                    SaveSelectedTabName();

                    return RedirectToAction("Edit", new { id = vendorAttribute.Id });
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

            var vendorAttribute = _vendorAttributeService.GetVendorAttributeById(id);
            if (vendorAttribute == null)
                //No vendor attribute found with the specified id
                return RedirectToAction("List");

            var model = vendorAttribute.ToModel();
            //locales
            AddLocales(_languageService, model.Locales, (locale, languageId) =>
            {
                locale.Name = vendorAttribute.GetLocalized(x => x.Name, languageId, false, false);
            });
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult Edit(VendorAttributeModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            var vendorAttribute = _vendorAttributeService.GetVendorAttributeById(model.Id);
            if (vendorAttribute == null)
                //No vendor attribute found with the specified id
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                vendorAttribute = model.ToEntity(vendorAttribute);
                _vendorAttributeService.UpdateVendorAttribute(vendorAttribute);

                //activity log
                _customerActivityService.InsertActivity("EditVendorAttribute",
                    string.Format(_localizationService.GetResource("ActivityLog.EditVendorAttribute"), vendorAttribute.Id), vendorAttribute);

                //locales
                UpdateAttributeLocales(vendorAttribute, model);

                SuccessNotification(_localizationService.GetResource("Admin.Vendors.VendorAttributes.Updated"));
                if (continueEditing)
                {
                    //selected tab
                    SaveSelectedTabName();

                    return RedirectToAction("Edit", new {id = vendorAttribute.Id});
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

            var vendorAttribute = _vendorAttributeService.GetVendorAttributeById(id);
            _vendorAttributeService.DeleteVendorAttribute(vendorAttribute);

            //activity log
            _customerActivityService.InsertActivity("DeleteVendorAttribute",
                string.Format(_localizationService.GetResource("ActivityLog.DeleteVendorAttribute"), vendorAttribute.Id), vendorAttribute);

            SuccessNotification(_localizationService.GetResource("Admin.Vendors.VendorAttributes.Deleted"));
            return RedirectToAction("List");
        }

        #endregion

        #region Vendor attribute values

        //list
        [HttpPost]
        public virtual IActionResult ValueList(int vendorAttributeId, DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedKendoGridJson();

            var values = _vendorAttributeService.GetVendorAttributeValues(vendorAttributeId);
            var gridModel = new DataSourceResult
            {
                Data = values.Select(x => new VendorAttributeValueModel
                {
                    Id = x.Id,
                    VendorAttributeId = x.VendorAttributeId,
                    Name = x.Name,
                    IsPreSelected = x.IsPreSelected,
                    DisplayOrder = x.DisplayOrder,
                }),
                Total = values.Count()
            };
            return Json(gridModel);
        }

        //create
        public virtual IActionResult ValueCreatePopup(int vendorAttributeId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            var vendorAttribute = _vendorAttributeService.GetVendorAttributeById(vendorAttributeId);
            if (vendorAttribute == null)
                //No vendor attribute found with the specified id
                return RedirectToAction("List");

            var model = new VendorAttributeValueModel
            {
                VendorAttributeId = vendorAttributeId
            };
            //locales
            AddLocales(_languageService, model.Locales);
            return View(model);
        }

        [HttpPost]
        public virtual IActionResult ValueCreatePopup(VendorAttributeValueModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            var vendorAttribute = _vendorAttributeService.GetVendorAttributeById(model.VendorAttributeId);
            if (vendorAttribute == null)
                //No vendor attribute found with the specified id
                return RedirectToAction("List");
            
            if (ModelState.IsValid)
            {
                var value = new VendorAttributeValue
                {
                    VendorAttributeId = model.VendorAttributeId,
                    Name = model.Name,
                    IsPreSelected = model.IsPreSelected,
                    DisplayOrder = model.DisplayOrder
                };

                _vendorAttributeService.InsertVendorAttributeValue(value);

                //activity log
                _customerActivityService.InsertActivity("AddNewVendorAttributeValue",
                    string.Format(_localizationService.GetResource("ActivityLog.AddNewVendorAttributeValue"), value.Id), value);

                UpdateValueLocales(value, model);

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

            var cav = _vendorAttributeService.GetVendorAttributeValueById(id);
            if (cav == null)
                //No vendor attribute value found with the specified id
                return RedirectToAction("List");

            var model = new VendorAttributeValueModel
            {
                VendorAttributeId = cav.VendorAttributeId,
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
        public virtual IActionResult ValueEditPopup(VendorAttributeValueModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            var value = _vendorAttributeService.GetVendorAttributeValueById(model.Id);
            if (value == null)
                //No vendor attribute value found with the specified id
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                value.Name = model.Name;
                value.IsPreSelected = model.IsPreSelected;
                value.DisplayOrder = model.DisplayOrder;
                _vendorAttributeService.UpdateVendorAttributeValue(value);

                //activity log
                _customerActivityService.InsertActivity("EditVendorAttributeValue",
                    string.Format(_localizationService.GetResource("ActivityLog.EditVendorAttributeValue"), value.Id), value);

                UpdateValueLocales(value, model);

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

            var value = _vendorAttributeService.GetVendorAttributeValueById(id);
            if (value == null)
                throw new ArgumentException("No vendor attribute value found with the specified id");
            _vendorAttributeService.DeleteVendorAttributeValue(value);

            //activity log
            _customerActivityService.InsertActivity("DeleteVendorAttributeValue",
                string.Format(_localizationService.GetResource("ActivityLog.DeleteVendorAttributeValue"), value.Id), value);

            return new NullJsonResult();
        }
        
        #endregion
    }
}