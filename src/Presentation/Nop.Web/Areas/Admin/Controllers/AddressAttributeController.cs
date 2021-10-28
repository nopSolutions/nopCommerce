using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Common;
using Nop.Services.Common;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Common;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Web.Areas.Admin.Controllers
{
    public partial class AddressAttributeController : BaseAdminController
    {
        #region Fields

        protected IAddressAttributeModelFactory AddressAttributeModelFactory { get; }
        protected IAddressAttributeService AddressAttributeService { get; }
        protected ICustomerActivityService CustomerActivityService { get; }
        protected ILocalizedEntityService LocalizedEntityService { get; }
        protected ILocalizationService LocalizationService { get; }
        protected INotificationService NotificationService { get; }
        protected IPermissionService PermissionService { get; }

        #endregion

        #region Ctor

        public AddressAttributeController(IAddressAttributeModelFactory addressAttributeModelFactory,
            IAddressAttributeService addressAttributeService,
            ICustomerActivityService customerActivityService,
            ILocalizedEntityService localizedEntityService,
            ILocalizationService localizationService,
            INotificationService notificationService,
            IPermissionService permissionService)
        {
            AddressAttributeModelFactory = addressAttributeModelFactory;
            AddressAttributeService = addressAttributeService;
            CustomerActivityService = customerActivityService;
            LocalizedEntityService = localizedEntityService;
            LocalizationService = localizationService;
            NotificationService = notificationService;
            PermissionService = permissionService;
        }

        #endregion

        #region Utilities

        protected virtual async Task UpdateAttributeLocalesAsync(AddressAttribute addressAttribute, AddressAttributeModel model)
        {
            foreach (var localized in model.Locales)
            {
                await LocalizedEntityService.SaveLocalizedValueAsync(addressAttribute,
                    x => x.Name,
                    localized.Name,
                    localized.LanguageId);
            }
        }

        protected virtual async Task UpdateValueLocalesAsync(AddressAttributeValue addressAttributeValue, AddressAttributeValueModel model)
        {
            foreach (var localized in model.Locales)
            {
                await LocalizedEntityService.SaveLocalizedValueAsync(addressAttributeValue,
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

        public virtual async Task<IActionResult> List()
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //select an appropriate card
            SaveSelectedCardName("customersettings-addressformfields");

            //we just redirect a user to the address settings page
            return RedirectToAction("CustomerUser", "Setting");
        }

        [HttpPost]
        public virtual async Task<IActionResult> List(AddressAttributeSearchModel searchModel)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await AddressAttributeModelFactory.PrepareAddressAttributeListModelAsync(searchModel);

            return Json(model);
        }

        public virtual async Task<IActionResult> Create()
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //prepare model
            var model = await AddressAttributeModelFactory.PrepareAddressAttributeModelAsync(new AddressAttributeModel(), null);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> Create(AddressAttributeModel model, bool continueEditing)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var addressAttribute = model.ToEntity<AddressAttribute>();
                await AddressAttributeService.InsertAddressAttributeAsync(addressAttribute);

                //activity log
                await CustomerActivityService.InsertActivityAsync("AddNewAddressAttribute",
                    string.Format(await LocalizationService.GetResourceAsync("ActivityLog.AddNewAddressAttribute"), addressAttribute.Id),
                    addressAttribute);

                //locales
                await UpdateAttributeLocalesAsync(addressAttribute, model);

                NotificationService.SuccessNotification(await LocalizationService.GetResourceAsync("Admin.Address.AddressAttributes.Added"));

                if (!continueEditing)
                    return RedirectToAction("List");

                return RedirectToAction("Edit", new { id = addressAttribute.Id });
            }

            //prepare model
            model = await AddressAttributeModelFactory.PrepareAddressAttributeModelAsync(model, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual async Task<IActionResult> Edit(int id)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //try to get an address attribute with the specified id
            var addressAttribute = await AddressAttributeService.GetAddressAttributeByIdAsync(id);
            if (addressAttribute == null)
                return RedirectToAction("List");

            //prepare model
            var model = await AddressAttributeModelFactory.PrepareAddressAttributeModelAsync(null, addressAttribute);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> Edit(AddressAttributeModel model, bool continueEditing)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //try to get an address attribute with the specified id
            var addressAttribute = await AddressAttributeService.GetAddressAttributeByIdAsync(model.Id);
            if (addressAttribute == null)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                addressAttribute = model.ToEntity(addressAttribute);
                await AddressAttributeService.UpdateAddressAttributeAsync(addressAttribute);

                //activity log
                await CustomerActivityService.InsertActivityAsync("EditAddressAttribute",
                    string.Format(await LocalizationService.GetResourceAsync("ActivityLog.EditAddressAttribute"), addressAttribute.Id),
                    addressAttribute);

                //locales
                await UpdateAttributeLocalesAsync(addressAttribute, model);

                NotificationService.SuccessNotification(await LocalizationService.GetResourceAsync("Admin.Address.AddressAttributes.Updated"));

                if (!continueEditing)
                    return RedirectToAction("List");

                return RedirectToAction("Edit", new { id = addressAttribute.Id });
            }

            //prepare model
            model = await AddressAttributeModelFactory.PrepareAddressAttributeModelAsync(model, addressAttribute, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //try to get an address attribute with the specified id
            var addressAttribute = await AddressAttributeService.GetAddressAttributeByIdAsync(id);
            if (addressAttribute == null)
                return RedirectToAction("List");

            await AddressAttributeService.DeleteAddressAttributeAsync(addressAttribute);

            //activity log
            await CustomerActivityService.InsertActivityAsync("DeleteAddressAttribute",
                string.Format(await LocalizationService.GetResourceAsync("ActivityLog.DeleteAddressAttribute"), addressAttribute.Id),
                addressAttribute);

            NotificationService.SuccessNotification(await LocalizationService.GetResourceAsync("Admin.Address.AddressAttributes.Deleted"));

            return RedirectToAction("List");
        }

        #endregion

        #region Address attribute values

        [HttpPost]
        public virtual async Task<IActionResult> ValueList(AddressAttributeValueSearchModel searchModel)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
                return await AccessDeniedDataTablesJson();

            //try to get an address attribute with the specified id
            var addressAttribute = await AddressAttributeService.GetAddressAttributeByIdAsync(searchModel.AddressAttributeId)
                ?? throw new ArgumentException("No address attribute found with the specified id");

            //prepare model
            var model = await AddressAttributeModelFactory.PrepareAddressAttributeValueListModelAsync(searchModel, addressAttribute);

            return Json(model);
        }

        public virtual async Task<IActionResult> ValueCreatePopup(int addressAttributeId)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //try to get an address attribute with the specified id
            var addressAttribute = await AddressAttributeService.GetAddressAttributeByIdAsync(addressAttributeId);
            if (addressAttribute == null)
                return RedirectToAction("List");

            //prepare model
            var model = await AddressAttributeModelFactory
                .PrepareAddressAttributeValueModelAsync(new AddressAttributeValueModel(), addressAttribute, null);

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> ValueCreatePopup(AddressAttributeValueModel model)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //try to get an address attribute with the specified id
            var addressAttribute = await AddressAttributeService.GetAddressAttributeByIdAsync(model.AddressAttributeId);
            if (addressAttribute == null)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                var addressAttributeValue = model.ToEntity<AddressAttributeValue>();
                await AddressAttributeService.InsertAddressAttributeValueAsync(addressAttributeValue);

                //activity log
                await CustomerActivityService.InsertActivityAsync("AddNewAddressAttributeValue",
                    string.Format(await LocalizationService.GetResourceAsync("ActivityLog.AddNewAddressAttributeValue"), addressAttributeValue.Id),
                    addressAttributeValue);

                await UpdateValueLocalesAsync(addressAttributeValue, model);

                ViewBag.RefreshPage = true;

                return View(model);
            }

            //prepare model
            model = await AddressAttributeModelFactory.PrepareAddressAttributeValueModelAsync(model, addressAttribute, null, true);
            
            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual async Task<IActionResult> ValueEditPopup(int id)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //try to get an address attribute value with the specified id
            var addressAttributeValue = await AddressAttributeService.GetAddressAttributeValueByIdAsync(id);
            if (addressAttributeValue == null)
                return RedirectToAction("List");

            //try to get an address attribute with the specified id
            var addressAttribute = await AddressAttributeService.GetAddressAttributeByIdAsync(addressAttributeValue.AddressAttributeId);
            if (addressAttribute == null)
                return RedirectToAction("List");

            //prepare model
            var model = await AddressAttributeModelFactory.PrepareAddressAttributeValueModelAsync(null, addressAttribute, addressAttributeValue);

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> ValueEditPopup(AddressAttributeValueModel model)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //try to get an address attribute value with the specified id
            var addressAttributeValue = await AddressAttributeService.GetAddressAttributeValueByIdAsync(model.Id);
            if (addressAttributeValue == null)
                return RedirectToAction("List");

            //try to get an address attribute with the specified id
            var addressAttribute = await AddressAttributeService.GetAddressAttributeByIdAsync(addressAttributeValue.AddressAttributeId);
            if (addressAttribute == null)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                addressAttributeValue = model.ToEntity(addressAttributeValue);
                await AddressAttributeService.UpdateAddressAttributeValueAsync(addressAttributeValue);

                await UpdateValueLocalesAsync(addressAttributeValue, model);

                //activity log
                await CustomerActivityService.InsertActivityAsync("EditAddressAttributeValue",
                    string.Format(await LocalizationService.GetResourceAsync("ActivityLog.EditAddressAttributeValue"), addressAttributeValue.Id),
                    addressAttributeValue);

                ViewBag.RefreshPage = true;

                return View(model);
            }

            //prepare model
            model = await AddressAttributeModelFactory.PrepareAddressAttributeValueModelAsync(model, addressAttribute, addressAttributeValue, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> ValueDelete(int id)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //try to get an address attribute value with the specified id
            var addressAttributeValue = await AddressAttributeService.GetAddressAttributeValueByIdAsync(id)
                ?? throw new ArgumentException("No address attribute value found with the specified id", nameof(id));

            await AddressAttributeService.DeleteAddressAttributeValueAsync(addressAttributeValue);

            //activity log
            await CustomerActivityService.InsertActivityAsync("DeleteAddressAttributeValue",
                string.Format(await LocalizationService.GetResourceAsync("ActivityLog.DeleteAddressAttributeValue"), addressAttributeValue.Id),
                addressAttributeValue);

            return new NullJsonResult();
        }

        #endregion
    }
}