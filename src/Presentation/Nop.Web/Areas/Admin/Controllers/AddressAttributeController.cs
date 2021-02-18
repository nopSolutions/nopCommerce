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

        private readonly IAddressAttributeModelFactory _addressAttributeModelFactory;
        private readonly IAddressAttributeService _addressAttributeService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;
        private readonly IPermissionService _permissionService;

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
            _addressAttributeModelFactory = addressAttributeModelFactory;
            _addressAttributeService = addressAttributeService;
            _customerActivityService = customerActivityService;
            _localizedEntityService = localizedEntityService;
            _localizationService = localizationService;
            _notificationService = notificationService;
            _permissionService = permissionService;
        }

        #endregion

        #region Utilities

        protected virtual async Task UpdateAttributeLocalesAsync(AddressAttribute addressAttribute, AddressAttributeModel model)
        {
            foreach (var localized in model.Locales)
            {
                await _localizedEntityService.SaveLocalizedValueAsync(addressAttribute,
                    x => x.Name,
                    localized.Name,
                    localized.LanguageId);
            }
        }

        protected virtual async Task UpdateValueLocalesAsync(AddressAttributeValue addressAttributeValue, AddressAttributeValueModel model)
        {
            foreach (var localized in model.Locales)
            {
                await _localizedEntityService.SaveLocalizedValueAsync(addressAttributeValue,
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
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //select an appropriate card
            SaveSelectedCardName("customersettings-addressformfields");

            //we just redirect a user to the address settings page
            return RedirectToAction("CustomerUser", "Setting");
        }

        [HttpPost]
        public virtual async Task<IActionResult> List(AddressAttributeSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await _addressAttributeModelFactory.PrepareAddressAttributeListModelAsync(searchModel);

            return Json(model);
        }

        public virtual async Task<IActionResult> Create()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //prepare model
            var model = await _addressAttributeModelFactory.PrepareAddressAttributeModelAsync(new AddressAttributeModel(), null);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> Create(AddressAttributeModel model, bool continueEditing)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var addressAttribute = model.ToEntity<AddressAttribute>();
                await _addressAttributeService.InsertAddressAttributeAsync(addressAttribute);

                //activity log
                await _customerActivityService.InsertActivityAsync("AddNewAddressAttribute",
                    string.Format(await _localizationService.GetResourceAsync("ActivityLog.AddNewAddressAttribute"), addressAttribute.Id),
                    addressAttribute);

                //locales
                await UpdateAttributeLocalesAsync(addressAttribute, model);

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Address.AddressAttributes.Added"));

                if (!continueEditing)
                    return RedirectToAction("List");

                return RedirectToAction("Edit", new { id = addressAttribute.Id });
            }

            //prepare model
            model = await _addressAttributeModelFactory.PrepareAddressAttributeModelAsync(model, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual async Task<IActionResult> Edit(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //try to get an address attribute with the specified id
            var addressAttribute = await _addressAttributeService.GetAddressAttributeByIdAsync(id);
            if (addressAttribute == null)
                return RedirectToAction("List");

            //prepare model
            var model = await _addressAttributeModelFactory.PrepareAddressAttributeModelAsync(null, addressAttribute);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> Edit(AddressAttributeModel model, bool continueEditing)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //try to get an address attribute with the specified id
            var addressAttribute = await _addressAttributeService.GetAddressAttributeByIdAsync(model.Id);
            if (addressAttribute == null)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                addressAttribute = model.ToEntity(addressAttribute);
                await _addressAttributeService.UpdateAddressAttributeAsync(addressAttribute);

                //activity log
                await _customerActivityService.InsertActivityAsync("EditAddressAttribute",
                    string.Format(await _localizationService.GetResourceAsync("ActivityLog.EditAddressAttribute"), addressAttribute.Id),
                    addressAttribute);

                //locales
                await UpdateAttributeLocalesAsync(addressAttribute, model);

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Address.AddressAttributes.Updated"));

                if (!continueEditing)
                    return RedirectToAction("List");

                return RedirectToAction("Edit", new { id = addressAttribute.Id });
            }

            //prepare model
            model = await _addressAttributeModelFactory.PrepareAddressAttributeModelAsync(model, addressAttribute, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //try to get an address attribute with the specified id
            var addressAttribute = await _addressAttributeService.GetAddressAttributeByIdAsync(id);
            if (addressAttribute == null)
                return RedirectToAction("List");

            await _addressAttributeService.DeleteAddressAttributeAsync(addressAttribute);

            //activity log
            await _customerActivityService.InsertActivityAsync("DeleteAddressAttribute",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.DeleteAddressAttribute"), addressAttribute.Id),
                addressAttribute);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Address.AddressAttributes.Deleted"));

            return RedirectToAction("List");
        }

        #endregion

        #region Address attribute values

        [HttpPost]
        public virtual async Task<IActionResult> ValueList(AddressAttributeValueSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
                return await AccessDeniedDataTablesJson();

            //try to get an address attribute with the specified id
            var addressAttribute = await _addressAttributeService.GetAddressAttributeByIdAsync(searchModel.AddressAttributeId)
                ?? throw new ArgumentException("No address attribute found with the specified id");

            //prepare model
            var model = await _addressAttributeModelFactory.PrepareAddressAttributeValueListModelAsync(searchModel, addressAttribute);

            return Json(model);
        }

        public virtual async Task<IActionResult> ValueCreatePopup(int addressAttributeId)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //try to get an address attribute with the specified id
            var addressAttribute = await _addressAttributeService.GetAddressAttributeByIdAsync(addressAttributeId);
            if (addressAttribute == null)
                return RedirectToAction("List");

            //prepare model
            var model = await _addressAttributeModelFactory
                .PrepareAddressAttributeValueModelAsync(new AddressAttributeValueModel(), addressAttribute, null);

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> ValueCreatePopup(AddressAttributeValueModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //try to get an address attribute with the specified id
            var addressAttribute = await _addressAttributeService.GetAddressAttributeByIdAsync(model.AddressAttributeId);
            if (addressAttribute == null)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                var addressAttributeValue = model.ToEntity<AddressAttributeValue>();
                await _addressAttributeService.InsertAddressAttributeValueAsync(addressAttributeValue);

                //activity log
                await _customerActivityService.InsertActivityAsync("AddNewAddressAttributeValue",
                    string.Format(await _localizationService.GetResourceAsync("ActivityLog.AddNewAddressAttributeValue"), addressAttributeValue.Id),
                    addressAttributeValue);

                await UpdateValueLocalesAsync(addressAttributeValue, model);

                ViewBag.RefreshPage = true;

                return View(model);
            }

            //prepare model
            model = await _addressAttributeModelFactory.PrepareAddressAttributeValueModelAsync(model, addressAttribute, null, true);
            
            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual async Task<IActionResult> ValueEditPopup(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //try to get an address attribute value with the specified id
            var addressAttributeValue = await _addressAttributeService.GetAddressAttributeValueByIdAsync(id);
            if (addressAttributeValue == null)
                return RedirectToAction("List");

            //try to get an address attribute with the specified id
            var addressAttribute = await _addressAttributeService.GetAddressAttributeByIdAsync(addressAttributeValue.AddressAttributeId);
            if (addressAttribute == null)
                return RedirectToAction("List");

            //prepare model
            var model = await _addressAttributeModelFactory.PrepareAddressAttributeValueModelAsync(null, addressAttribute, addressAttributeValue);

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> ValueEditPopup(AddressAttributeValueModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //try to get an address attribute value with the specified id
            var addressAttributeValue = await _addressAttributeService.GetAddressAttributeValueByIdAsync(model.Id);
            if (addressAttributeValue == null)
                return RedirectToAction("List");

            //try to get an address attribute with the specified id
            var addressAttribute = await _addressAttributeService.GetAddressAttributeByIdAsync(addressAttributeValue.AddressAttributeId);
            if (addressAttribute == null)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                addressAttributeValue = model.ToEntity(addressAttributeValue);
                await _addressAttributeService.UpdateAddressAttributeValueAsync(addressAttributeValue);

                await UpdateValueLocalesAsync(addressAttributeValue, model);

                //activity log
                await _customerActivityService.InsertActivityAsync("EditAddressAttributeValue",
                    string.Format(await _localizationService.GetResourceAsync("ActivityLog.EditAddressAttributeValue"), addressAttributeValue.Id),
                    addressAttributeValue);

                ViewBag.RefreshPage = true;

                return View(model);
            }

            //prepare model
            model = await _addressAttributeModelFactory.PrepareAddressAttributeValueModelAsync(model, addressAttribute, addressAttributeValue, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> ValueDelete(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //try to get an address attribute value with the specified id
            var addressAttributeValue = await _addressAttributeService.GetAddressAttributeValueByIdAsync(id)
                ?? throw new ArgumentException("No address attribute value found with the specified id", nameof(id));

            await _addressAttributeService.DeleteAddressAttributeValueAsync(addressAttributeValue);

            //activity log
            await _customerActivityService.InsertActivityAsync("DeleteAddressAttributeValue",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.DeleteAddressAttributeValue"), addressAttributeValue.Id),
                addressAttributeValue);

            return new NullJsonResult();
        }

        #endregion
    }
}