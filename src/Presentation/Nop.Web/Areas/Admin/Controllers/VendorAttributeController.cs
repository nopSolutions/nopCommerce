using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Vendors;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
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
        private readonly INotificationService _notificationService;
        private readonly IPermissionService _permissionService;
        private readonly IVendorAttributeModelFactory _vendorAttributeModelFactory;
        private readonly IVendorAttributeService _vendorAttributeService;

        #endregion

        #region Ctor

        public VendorAttributeController(ICustomerActivityService customerActivityService,
            ILocalizationService localizationService,
            ILocalizedEntityService localizedEntityService,
            INotificationService notificationService,
            IPermissionService permissionService,
            IVendorAttributeModelFactory vendorAttributeModelFactory,
            IVendorAttributeService vendorAttributeService)
        {
            _customerActivityService = customerActivityService;
            _localizationService = localizationService;
            _localizedEntityService = localizedEntityService;
            _notificationService = notificationService;
            _permissionService = permissionService;
            _vendorAttributeModelFactory = vendorAttributeModelFactory;
            _vendorAttributeService = vendorAttributeService;
        }

        #endregion

        #region Utilities

        protected virtual async Task UpdateAttributeLocalesAsync(VendorAttribute vendorAttribute, VendorAttributeModel model)
        {
            foreach (var localized in model.Locales)
            {
                await _localizedEntityService.SaveLocalizedValueAsync(vendorAttribute,
                    x => x.Name,
                    localized.Name,
                    localized.LanguageId);
            }
        }

        protected virtual async Task UpdateValueLocalesAsync(VendorAttributeValue vendorAttributeValue, VendorAttributeValueModel model)
        {
            foreach (var localized in model.Locales)
            {
                await _localizedEntityService.SaveLocalizedValueAsync(vendorAttributeValue,
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

        public virtual async Task<IActionResult> List()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //we just redirect a user to the vendor settings page
            return RedirectToAction("Vendor", "Setting");
        }

        [HttpPost]
        public virtual async Task<IActionResult> List(VendorAttributeSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await _vendorAttributeModelFactory.PrepareVendorAttributeListModelAsync(searchModel);

            return Json(model);
        }

        public virtual async Task<IActionResult> Create()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //prepare model
            var model = await _vendorAttributeModelFactory.PrepareVendorAttributeModelAsync(new VendorAttributeModel(), null);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> Create(VendorAttributeModel model, bool continueEditing)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var vendorAttribute = model.ToEntity<VendorAttribute>();
                await _vendorAttributeService.InsertVendorAttributeAsync(vendorAttribute);

                //activity log
                await _customerActivityService.InsertActivityAsync("AddNewVendorAttribute",
                    string.Format(await _localizationService.GetResourceAsync("ActivityLog.AddNewVendorAttribute"), vendorAttribute.Id), vendorAttribute);

                //locales
                await UpdateAttributeLocalesAsync(vendorAttribute, model);

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Vendors.VendorAttributes.Added"));

                if (!continueEditing)
                    return RedirectToAction("List");
                
                return RedirectToAction("Edit", new { id = vendorAttribute.Id });
            }

            //prepare model
            model = await _vendorAttributeModelFactory.PrepareVendorAttributeModelAsync(model, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual async Task<IActionResult> Edit(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //try to get a vendor attribute with the specified id
            var vendorAttribute = await _vendorAttributeService.GetVendorAttributeByIdAsync(id);
            if (vendorAttribute == null)
                return RedirectToAction("List");

            //prepare model
            var model = await _vendorAttributeModelFactory.PrepareVendorAttributeModelAsync(null, vendorAttribute);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> Edit(VendorAttributeModel model, bool continueEditing)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //try to get a vendor attribute with the specified id
            var vendorAttribute = await _vendorAttributeService.GetVendorAttributeByIdAsync(model.Id);
            if (vendorAttribute == null)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                vendorAttribute = model.ToEntity(vendorAttribute);
                await _vendorAttributeService.UpdateVendorAttributeAsync(vendorAttribute);

                //activity log
                await _customerActivityService.InsertActivityAsync("EditVendorAttribute",
                    string.Format(await _localizationService.GetResourceAsync("ActivityLog.EditVendorAttribute"), vendorAttribute.Id), vendorAttribute);

                //locales
                await UpdateAttributeLocalesAsync(vendorAttribute, model);

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Vendors.VendorAttributes.Updated"));
                if (!continueEditing)
                    return RedirectToAction("List");
                
                return RedirectToAction("Edit", new { id = vendorAttribute.Id });
            }

            //prepare model
            model = await _vendorAttributeModelFactory.PrepareVendorAttributeModelAsync(model, vendorAttribute, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //try to get a vendor attribute with the specified id
            var vendorAttribute = await _vendorAttributeService.GetVendorAttributeByIdAsync(id);
            if (vendorAttribute == null)
                return RedirectToAction("List");

            await _vendorAttributeService.DeleteVendorAttributeAsync(vendorAttribute);

            //activity log
            await _customerActivityService.InsertActivityAsync("DeleteVendorAttribute",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.DeleteVendorAttribute"), vendorAttribute.Id), vendorAttribute);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Vendors.VendorAttributes.Deleted"));

            return RedirectToAction("List");
        }

        #endregion

        #region Vendor attribute values

        [HttpPost]
        public virtual async Task<IActionResult> ValueList(VendorAttributeValueSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
                return await AccessDeniedDataTablesJson();

            //try to get a vendor attribute with the specified id
            var vendorAttribute = await _vendorAttributeService.GetVendorAttributeByIdAsync(searchModel.VendorAttributeId)
                ?? throw new ArgumentException("No vendor attribute found with the specified id");

            //prepare model
            var model = await _vendorAttributeModelFactory.PrepareVendorAttributeValueListModelAsync(searchModel, vendorAttribute);

            return Json(model);
        }

        public virtual async Task<IActionResult> ValueCreatePopup(int vendorAttributeId)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //try to get a vendor attribute with the specified id
            var vendorAttribute = await _vendorAttributeService.GetVendorAttributeByIdAsync(vendorAttributeId);
            if (vendorAttribute == null)
                return RedirectToAction("List");

            //prepare model
            var model = await _vendorAttributeModelFactory.PrepareVendorAttributeValueModelAsync(new VendorAttributeValueModel(), vendorAttribute, null);

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> ValueCreatePopup(VendorAttributeValueModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //try to get a vendor attribute with the specified id
            var vendorAttribute = await _vendorAttributeService.GetVendorAttributeByIdAsync(model.VendorAttributeId);
            if (vendorAttribute == null)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                var value = model.ToEntity<VendorAttributeValue>();

                await _vendorAttributeService.InsertVendorAttributeValueAsync(value);

                //activity log
                await _customerActivityService.InsertActivityAsync("AddNewVendorAttributeValue",
                    string.Format(await _localizationService.GetResourceAsync("ActivityLog.AddNewVendorAttributeValue"), value.Id), value);

                await UpdateValueLocalesAsync(value, model);

                ViewBag.RefreshPage = true;

                return View(model);
            }

            //prepare model
            model = await _vendorAttributeModelFactory.PrepareVendorAttributeValueModelAsync(model, vendorAttribute, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        //edit
        public virtual async Task<IActionResult> ValueEditPopup(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //try to get a vendor attribute value with the specified id
            var vendorAttributeValue = await _vendorAttributeService.GetVendorAttributeValueByIdAsync(id);
            if (vendorAttributeValue == null)
                return RedirectToAction("List");

            //try to get a vendor attribute with the specified id
            var vendorAttribute = await _vendorAttributeService.GetVendorAttributeByIdAsync(vendorAttributeValue.VendorAttributeId);
            if (vendorAttribute == null)
                return RedirectToAction("List");

            //prepare model
            var model = await _vendorAttributeModelFactory.PrepareVendorAttributeValueModelAsync(null, vendorAttribute, vendorAttributeValue);

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> ValueEditPopup(VendorAttributeValueModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //try to get a vendor attribute value with the specified id
            var vendorAttributeValue = await _vendorAttributeService.GetVendorAttributeValueByIdAsync(model.Id);
            if (vendorAttributeValue == null)
                return RedirectToAction("List");

            //try to get a vendor attribute with the specified id
            var vendorAttribute = await _vendorAttributeService.GetVendorAttributeByIdAsync(vendorAttributeValue.VendorAttributeId);
            if (vendorAttribute == null)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                vendorAttributeValue = model.ToEntity(vendorAttributeValue);
                await _vendorAttributeService.UpdateVendorAttributeValueAsync(vendorAttributeValue);

                //activity log
                await _customerActivityService.InsertActivityAsync("EditVendorAttributeValue",
                    string.Format(await _localizationService.GetResourceAsync("ActivityLog.EditVendorAttributeValue"), vendorAttributeValue.Id),
                    vendorAttributeValue);

                await UpdateValueLocalesAsync(vendorAttributeValue, model);

                ViewBag.RefreshPage = true;

                return View(model);
            }

            //prepare model
            model = await _vendorAttributeModelFactory.PrepareVendorAttributeValueModelAsync(model, vendorAttribute, vendorAttributeValue, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> ValueDelete(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //try to get a vendor attribute value with the specified id
            var value = await _vendorAttributeService.GetVendorAttributeValueByIdAsync(id)
                ?? throw new ArgumentException("No vendor attribute value found with the specified id", nameof(id));

            await _vendorAttributeService.DeleteVendorAttributeValueAsync(value);

            //activity log
            await _customerActivityService.InsertActivityAsync("DeleteVendorAttributeValue",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.DeleteVendorAttributeValue"), value.Id), value);

            return new NullJsonResult();
        }

        #endregion
    }
}