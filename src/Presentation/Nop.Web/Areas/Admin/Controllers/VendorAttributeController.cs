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

        protected ICustomerActivityService CustomerActivityService { get; }
        protected ILocalizationService LocalizationService { get; }
        protected ILocalizedEntityService LocalizedEntityService { get; }
        protected INotificationService NotificationService { get; }
        protected IPermissionService PermissionService { get; }
        protected IVendorAttributeModelFactory VendorAttributeModelFactory { get; }
        protected IVendorAttributeService VendorAttributeService { get; }

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
            CustomerActivityService = customerActivityService;
            LocalizationService = localizationService;
            LocalizedEntityService = localizedEntityService;
            NotificationService = notificationService;
            PermissionService = permissionService;
            VendorAttributeModelFactory = vendorAttributeModelFactory;
            VendorAttributeService = vendorAttributeService;
        }

        #endregion

        #region Utilities

        protected virtual async Task UpdateAttributeLocalesAsync(VendorAttribute vendorAttribute, VendorAttributeModel model)
        {
            foreach (var localized in model.Locales)
            {
                await LocalizedEntityService.SaveLocalizedValueAsync(vendorAttribute,
                    x => x.Name,
                    localized.Name,
                    localized.LanguageId);
            }
        }

        protected virtual async Task UpdateValueLocalesAsync(VendorAttributeValue vendorAttributeValue, VendorAttributeValueModel model)
        {
            foreach (var localized in model.Locales)
            {
                await LocalizedEntityService.SaveLocalizedValueAsync(vendorAttributeValue,
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
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //we just redirect a user to the vendor settings page
            return RedirectToAction("Vendor", "Setting");
        }

        [HttpPost]
        public virtual async Task<IActionResult> List(VendorAttributeSearchModel searchModel)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await VendorAttributeModelFactory.PrepareVendorAttributeListModelAsync(searchModel);

            return Json(model);
        }

        public virtual async Task<IActionResult> Create()
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //prepare model
            var model = await VendorAttributeModelFactory.PrepareVendorAttributeModelAsync(new VendorAttributeModel(), null);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> Create(VendorAttributeModel model, bool continueEditing)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var vendorAttribute = model.ToEntity<VendorAttribute>();
                await VendorAttributeService.InsertVendorAttributeAsync(vendorAttribute);

                //activity log
                await CustomerActivityService.InsertActivityAsync("AddNewVendorAttribute",
                    string.Format(await LocalizationService.GetResourceAsync("ActivityLog.AddNewVendorAttribute"), vendorAttribute.Id), vendorAttribute);

                //locales
                await UpdateAttributeLocalesAsync(vendorAttribute, model);

                NotificationService.SuccessNotification(await LocalizationService.GetResourceAsync("Admin.Vendors.VendorAttributes.Added"));

                if (!continueEditing)
                    return RedirectToAction("List");
                
                return RedirectToAction("Edit", new { id = vendorAttribute.Id });
            }

            //prepare model
            model = await VendorAttributeModelFactory.PrepareVendorAttributeModelAsync(model, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual async Task<IActionResult> Edit(int id)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //try to get a vendor attribute with the specified id
            var vendorAttribute = await VendorAttributeService.GetVendorAttributeByIdAsync(id);
            if (vendorAttribute == null)
                return RedirectToAction("List");

            //prepare model
            var model = await VendorAttributeModelFactory.PrepareVendorAttributeModelAsync(null, vendorAttribute);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> Edit(VendorAttributeModel model, bool continueEditing)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //try to get a vendor attribute with the specified id
            var vendorAttribute = await VendorAttributeService.GetVendorAttributeByIdAsync(model.Id);
            if (vendorAttribute == null)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                vendorAttribute = model.ToEntity(vendorAttribute);
                await VendorAttributeService.UpdateVendorAttributeAsync(vendorAttribute);

                //activity log
                await CustomerActivityService.InsertActivityAsync("EditVendorAttribute",
                    string.Format(await LocalizationService.GetResourceAsync("ActivityLog.EditVendorAttribute"), vendorAttribute.Id), vendorAttribute);

                //locales
                await UpdateAttributeLocalesAsync(vendorAttribute, model);

                NotificationService.SuccessNotification(await LocalizationService.GetResourceAsync("Admin.Vendors.VendorAttributes.Updated"));
                if (!continueEditing)
                    return RedirectToAction("List");
                
                return RedirectToAction("Edit", new { id = vendorAttribute.Id });
            }

            //prepare model
            model = await VendorAttributeModelFactory.PrepareVendorAttributeModelAsync(model, vendorAttribute, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //try to get a vendor attribute with the specified id
            var vendorAttribute = await VendorAttributeService.GetVendorAttributeByIdAsync(id);
            if (vendorAttribute == null)
                return RedirectToAction("List");

            await VendorAttributeService.DeleteVendorAttributeAsync(vendorAttribute);

            //activity log
            await CustomerActivityService.InsertActivityAsync("DeleteVendorAttribute",
                string.Format(await LocalizationService.GetResourceAsync("ActivityLog.DeleteVendorAttribute"), vendorAttribute.Id), vendorAttribute);

            NotificationService.SuccessNotification(await LocalizationService.GetResourceAsync("Admin.Vendors.VendorAttributes.Deleted"));

            return RedirectToAction("List");
        }

        #endregion

        #region Vendor attribute values

        [HttpPost]
        public virtual async Task<IActionResult> ValueList(VendorAttributeValueSearchModel searchModel)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
                return await AccessDeniedDataTablesJson();

            //try to get a vendor attribute with the specified id
            var vendorAttribute = await VendorAttributeService.GetVendorAttributeByIdAsync(searchModel.VendorAttributeId)
                ?? throw new ArgumentException("No vendor attribute found with the specified id");

            //prepare model
            var model = await VendorAttributeModelFactory.PrepareVendorAttributeValueListModelAsync(searchModel, vendorAttribute);

            return Json(model);
        }

        public virtual async Task<IActionResult> ValueCreatePopup(int vendorAttributeId)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //try to get a vendor attribute with the specified id
            var vendorAttribute = await VendorAttributeService.GetVendorAttributeByIdAsync(vendorAttributeId);
            if (vendorAttribute == null)
                return RedirectToAction("List");

            //prepare model
            var model = await VendorAttributeModelFactory.PrepareVendorAttributeValueModelAsync(new VendorAttributeValueModel(), vendorAttribute, null);

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> ValueCreatePopup(VendorAttributeValueModel model)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //try to get a vendor attribute with the specified id
            var vendorAttribute = await VendorAttributeService.GetVendorAttributeByIdAsync(model.VendorAttributeId);
            if (vendorAttribute == null)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                var value = model.ToEntity<VendorAttributeValue>();

                await VendorAttributeService.InsertVendorAttributeValueAsync(value);

                //activity log
                await CustomerActivityService.InsertActivityAsync("AddNewVendorAttributeValue",
                    string.Format(await LocalizationService.GetResourceAsync("ActivityLog.AddNewVendorAttributeValue"), value.Id), value);

                await UpdateValueLocalesAsync(value, model);

                ViewBag.RefreshPage = true;

                return View(model);
            }

            //prepare model
            model = await VendorAttributeModelFactory.PrepareVendorAttributeValueModelAsync(model, vendorAttribute, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        //edit
        public virtual async Task<IActionResult> ValueEditPopup(int id)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //try to get a vendor attribute value with the specified id
            var vendorAttributeValue = await VendorAttributeService.GetVendorAttributeValueByIdAsync(id);
            if (vendorAttributeValue == null)
                return RedirectToAction("List");

            //try to get a vendor attribute with the specified id
            var vendorAttribute = await VendorAttributeService.GetVendorAttributeByIdAsync(vendorAttributeValue.VendorAttributeId);
            if (vendorAttribute == null)
                return RedirectToAction("List");

            //prepare model
            var model = await VendorAttributeModelFactory.PrepareVendorAttributeValueModelAsync(null, vendorAttribute, vendorAttributeValue);

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> ValueEditPopup(VendorAttributeValueModel model)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //try to get a vendor attribute value with the specified id
            var vendorAttributeValue = await VendorAttributeService.GetVendorAttributeValueByIdAsync(model.Id);
            if (vendorAttributeValue == null)
                return RedirectToAction("List");

            //try to get a vendor attribute with the specified id
            var vendorAttribute = await VendorAttributeService.GetVendorAttributeByIdAsync(vendorAttributeValue.VendorAttributeId);
            if (vendorAttribute == null)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                vendorAttributeValue = model.ToEntity(vendorAttributeValue);
                await VendorAttributeService.UpdateVendorAttributeValueAsync(vendorAttributeValue);

                //activity log
                await CustomerActivityService.InsertActivityAsync("EditVendorAttributeValue",
                    string.Format(await LocalizationService.GetResourceAsync("ActivityLog.EditVendorAttributeValue"), vendorAttributeValue.Id),
                    vendorAttributeValue);

                await UpdateValueLocalesAsync(vendorAttributeValue, model);

                ViewBag.RefreshPage = true;

                return View(model);
            }

            //prepare model
            model = await VendorAttributeModelFactory.PrepareVendorAttributeValueModelAsync(model, vendorAttribute, vendorAttributeValue, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> ValueDelete(int id)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //try to get a vendor attribute value with the specified id
            var value = await VendorAttributeService.GetVendorAttributeValueByIdAsync(id)
                ?? throw new ArgumentException("No vendor attribute value found with the specified id", nameof(id));

            await VendorAttributeService.DeleteVendorAttributeValueAsync(value);

            //activity log
            await CustomerActivityService.InsertActivityAsync("DeleteVendorAttributeValue",
                string.Format(await LocalizationService.GetResourceAsync("ActivityLog.DeleteVendorAttributeValue"), value.Id), value);

            return new NullJsonResult();
        }

        #endregion
    }
}