using System;
using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Vendors;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Security;
using Nop.Services.Vendors;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Vendors;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Web.Areas.Admin.Controllers
{
    public partial class VendorAttributeController : BaseAdminController
    {
        #region Fields

        private readonly ICustomerActivityService _customerActivityService;
        private readonly ILocalizationService _localizationService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly IPermissionService _permissionService;
        private readonly IVendorAttributeModelFactory _vendorAttributeModelFactory;
        private readonly IVendorAttributeService _vendorAttributeService;

        #endregion

        #region Ctor

        public VendorAttributeController(ICustomerActivityService customerActivityService,
            ILocalizationService localizationService,
            ILocalizedEntityService localizedEntityService,
            IPermissionService permissionService,
            IVendorAttributeModelFactory vendorAttributeModelFactory,
            IVendorAttributeService vendorAttributeService)
        {
            this._customerActivityService = customerActivityService;
            this._localizationService = localizationService;
            this._localizedEntityService = localizedEntityService;
            this._permissionService = permissionService;
            this._vendorAttributeModelFactory = vendorAttributeModelFactory;
            this._vendorAttributeService = vendorAttributeService;
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

            //we just redirect a user to the vendor settings page
            return RedirectToAction("Vendor", "Setting");
        }

        [HttpPost]
        public virtual IActionResult List(VendorAttributeSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedKendoGridJson();

            //prepare model
            var model = _vendorAttributeModelFactory.PrepareVendorAttributeListModel(searchModel);

            return Json(model);
        }

        public virtual IActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //prepare model
            var model = _vendorAttributeModelFactory.PrepareVendorAttributeModel(new VendorAttributeModel(), null);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult Create(VendorAttributeModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var vendorAttribute = model.ToEntity<VendorAttribute>();
                _vendorAttributeService.InsertVendorAttribute(vendorAttribute);

                //activity log
                _customerActivityService.InsertActivity("AddNewVendorAttribute",
                    string.Format(_localizationService.GetResource("ActivityLog.AddNewVendorAttribute"), vendorAttribute.Id), vendorAttribute);

                //locales
                UpdateAttributeLocales(vendorAttribute, model);

                SuccessNotification(_localizationService.GetResource("Admin.Vendors.VendorAttributes.Added"));

                if (!continueEditing)
                    return RedirectToAction("List");

                //selected tab
                SaveSelectedTabName();

                return RedirectToAction("Edit", new { id = vendorAttribute.Id });
            }

            //prepare model
            model = _vendorAttributeModelFactory.PrepareVendorAttributeModel(model, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual IActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //try to get a vendor attribute with the specified id
            var vendorAttribute = _vendorAttributeService.GetVendorAttributeById(id);
            if (vendorAttribute == null)
                return RedirectToAction("List");

            //prepare model
            var model = _vendorAttributeModelFactory.PrepareVendorAttributeModel(null, vendorAttribute);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult Edit(VendorAttributeModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //try to get a vendor attribute with the specified id
            var vendorAttribute = _vendorAttributeService.GetVendorAttributeById(model.Id);
            if (vendorAttribute == null)
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
                if (!continueEditing)
                    return RedirectToAction("List");

                //selected tab
                SaveSelectedTabName();

                return RedirectToAction("Edit", new { id = vendorAttribute.Id });
            }

            //prepare model
            model = _vendorAttributeModelFactory.PrepareVendorAttributeModel(model, vendorAttribute, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual IActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //try to get a vendor attribute with the specified id
            var vendorAttribute = _vendorAttributeService.GetVendorAttributeById(id);
            if (vendorAttribute == null)
                return RedirectToAction("List");

            _vendorAttributeService.DeleteVendorAttribute(vendorAttribute);

            //activity log
            _customerActivityService.InsertActivity("DeleteVendorAttribute",
                string.Format(_localizationService.GetResource("ActivityLog.DeleteVendorAttribute"), vendorAttribute.Id), vendorAttribute);

            SuccessNotification(_localizationService.GetResource("Admin.Vendors.VendorAttributes.Deleted"));

            return RedirectToAction("List");
        }

        #endregion

        #region Vendor attribute values

        [HttpPost]
        public virtual IActionResult ValueList(VendorAttributeValueSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedKendoGridJson();

            //try to get a vendor attribute with the specified id
            var vendorAttribute = _vendorAttributeService.GetVendorAttributeById(searchModel.VendorAttributeId)
                ?? throw new ArgumentException("No vendor attribute found with the specified id");

            //prepare model
            var model = _vendorAttributeModelFactory.PrepareVendorAttributeValueListModel(searchModel, vendorAttribute);

            return Json(model);
        }

        public virtual IActionResult ValueCreatePopup(int vendorAttributeId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //try to get a vendor attribute with the specified id
            var vendorAttribute = _vendorAttributeService.GetVendorAttributeById(vendorAttributeId);
            if (vendorAttribute == null)
                return RedirectToAction("List");

            //prepare model
            var model = _vendorAttributeModelFactory.PrepareVendorAttributeValueModel(new VendorAttributeValueModel(), vendorAttribute, null);

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult ValueCreatePopup(VendorAttributeValueModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //try to get a vendor attribute with the specified id
            var vendorAttribute = _vendorAttributeService.GetVendorAttributeById(model.VendorAttributeId);
            if (vendorAttribute == null)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                var value = model.ToEntity<VendorAttributeValue>();

                _vendorAttributeService.InsertVendorAttributeValue(value);

                //activity log
                _customerActivityService.InsertActivity("AddNewVendorAttributeValue",
                    string.Format(_localizationService.GetResource("ActivityLog.AddNewVendorAttributeValue"), value.Id), value);

                UpdateValueLocales(value, model);

                ViewBag.RefreshPage = true;

                return View(model);
            }

            //prepare model
            model = _vendorAttributeModelFactory.PrepareVendorAttributeValueModel(model, vendorAttribute, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        //edit
        public virtual IActionResult ValueEditPopup(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //try to get a vendor attribute value with the specified id
            var vendorAttributeValue = _vendorAttributeService.GetVendorAttributeValueById(id);
            if (vendorAttributeValue == null)
                return RedirectToAction("List");

            //try to get a vendor attribute with the specified id
            var vendorAttribute = _vendorAttributeService.GetVendorAttributeById(vendorAttributeValue.VendorAttributeId);
            if (vendorAttribute == null)
                return RedirectToAction("List");

            //prepare model
            var model = _vendorAttributeModelFactory.PrepareVendorAttributeValueModel(null, vendorAttribute, vendorAttributeValue);

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult ValueEditPopup(VendorAttributeValueModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //try to get a vendor attribute value with the specified id
            var vendorAttributeValue = _vendorAttributeService.GetVendorAttributeValueById(model.Id);
            if (vendorAttributeValue == null)
                return RedirectToAction("List");

            //try to get a vendor attribute with the specified id
            var vendorAttribute = _vendorAttributeService.GetVendorAttributeById(vendorAttributeValue.VendorAttributeId);
            if (vendorAttribute == null)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                vendorAttributeValue = model.ToEntity(vendorAttributeValue);
                _vendorAttributeService.UpdateVendorAttributeValue(vendorAttributeValue);

                //activity log
                _customerActivityService.InsertActivity("EditVendorAttributeValue",
                    string.Format(_localizationService.GetResource("ActivityLog.EditVendorAttributeValue"), vendorAttributeValue.Id),
                    vendorAttributeValue);

                UpdateValueLocales(vendorAttributeValue, model);

                ViewBag.RefreshPage = true;

                return View(model);
            }

            //prepare model
            model = _vendorAttributeModelFactory.PrepareVendorAttributeValueModel(model, vendorAttribute, vendorAttributeValue, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual IActionResult ValueDelete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //try to get a vendor attribute value with the specified id
            var value = _vendorAttributeService.GetVendorAttributeValueById(id)
                ?? throw new ArgumentException("No vendor attribute value found with the specified id", nameof(id));

            _vendorAttributeService.DeleteVendorAttributeValue(value);

            //activity log
            _customerActivityService.InsertActivity("DeleteVendorAttributeValue",
                string.Format(_localizationService.GetResource("ActivityLog.DeleteVendorAttributeValue"), value.Id), value);

            return new NullJsonResult();
        }

        #endregion
    }
}