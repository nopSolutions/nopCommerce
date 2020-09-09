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

        protected virtual async Task UpdateAttributeLocales(AddressAttribute addressAttribute, AddressAttributeModel model)
        {
            foreach (var localized in model.Locales)
            {
                await _localizedEntityService.SaveLocalizedValue(addressAttribute,
                    x => x.Name,
                    localized.Name,
                    localized.LanguageId);
            }
        }

        protected virtual async Task UpdateValueLocales(AddressAttributeValue addressAttributeValue, AddressAttributeValueModel model)
        {
            foreach (var localized in model.Locales)
            {
                await _localizedEntityService.SaveLocalizedValue(addressAttributeValue,
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
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //select an appropriate panel
            SaveSelectedPanelName("customersettings-addressformfields");

            //we just redirect a user to the address settings page
            return RedirectToAction("CustomerUser", "Setting");
        }

        [HttpPost]
        public virtual async Task<IActionResult> List(AddressAttributeSearchModel searchModel)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedDataTablesJson();

            //prepare model
            var model = await _addressAttributeModelFactory.PrepareAddressAttributeListModel(searchModel);

            return Json(model);
        }

        public virtual async Task<IActionResult> Create()
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //prepare model
            var model = await _addressAttributeModelFactory.PrepareAddressAttributeModel(new AddressAttributeModel(), null);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> Create(AddressAttributeModel model, bool continueEditing)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var addressAttribute = model.ToEntity<AddressAttribute>();
                await _addressAttributeService.InsertAddressAttribute(addressAttribute);

                //activity log
                await _customerActivityService.InsertActivity("AddNewAddressAttribute",
                    string.Format(await _localizationService.GetResource("ActivityLog.AddNewAddressAttribute"), addressAttribute.Id),
                    addressAttribute);

                //locales
                await UpdateAttributeLocales(addressAttribute, model);

                _notificationService.SuccessNotification(await _localizationService.GetResource("Admin.Address.AddressAttributes.Added"));

                if (!continueEditing)
                    return RedirectToAction("List");

                return RedirectToAction("Edit", new { id = addressAttribute.Id });
            }

            //prepare model
            model = await _addressAttributeModelFactory.PrepareAddressAttributeModel(model, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual async Task<IActionResult> Edit(int id)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //try to get an address attribute with the specified id
            var addressAttribute = await _addressAttributeService.GetAddressAttributeById(id);
            if (addressAttribute == null)
                return RedirectToAction("List");

            //prepare model
            var model = await _addressAttributeModelFactory.PrepareAddressAttributeModel(null, addressAttribute);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> Edit(AddressAttributeModel model, bool continueEditing)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //try to get an address attribute with the specified id
            var addressAttribute = await _addressAttributeService.GetAddressAttributeById(model.Id);
            if (addressAttribute == null)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                addressAttribute = model.ToEntity(addressAttribute);
                await _addressAttributeService.UpdateAddressAttribute(addressAttribute);

                //activity log
                await _customerActivityService.InsertActivity("EditAddressAttribute",
                    string.Format(await _localizationService.GetResource("ActivityLog.EditAddressAttribute"), addressAttribute.Id),
                    addressAttribute);

                //locales
                await UpdateAttributeLocales(addressAttribute, model);

                _notificationService.SuccessNotification(await _localizationService.GetResource("Admin.Address.AddressAttributes.Updated"));

                if (!continueEditing)
                    return RedirectToAction("List");

                return RedirectToAction("Edit", new { id = addressAttribute.Id });
            }

            //prepare model
            model = await _addressAttributeModelFactory.PrepareAddressAttributeModel(model, addressAttribute, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //try to get an address attribute with the specified id
            var addressAttribute = await _addressAttributeService.GetAddressAttributeById(id);
            if (addressAttribute == null)
                return RedirectToAction("List");

            await _addressAttributeService.DeleteAddressAttribute(addressAttribute);

            //activity log
            await _customerActivityService.InsertActivity("DeleteAddressAttribute",
                string.Format(await _localizationService.GetResource("ActivityLog.DeleteAddressAttribute"), addressAttribute.Id),
                addressAttribute);

            _notificationService.SuccessNotification(await _localizationService.GetResource("Admin.Address.AddressAttributes.Deleted"));

            return RedirectToAction("List");
        }

        #endregion

        #region Address attribute values

        [HttpPost]
        public virtual async Task<IActionResult> ValueList(AddressAttributeValueSearchModel searchModel)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedDataTablesJson();

            //try to get an address attribute with the specified id
            var addressAttribute = await _addressAttributeService.GetAddressAttributeById(searchModel.AddressAttributeId)
                ?? throw new ArgumentException("No address attribute found with the specified id");

            //prepare model
            var model = await _addressAttributeModelFactory.PrepareAddressAttributeValueListModel(searchModel, addressAttribute);

            return Json(model);
        }

        public virtual async Task<IActionResult> ValueCreatePopup(int addressAttributeId)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //try to get an address attribute with the specified id
            var addressAttribute = await _addressAttributeService.GetAddressAttributeById(addressAttributeId);
            if (addressAttribute == null)
                return RedirectToAction("List");

            //prepare model
            var model = await _addressAttributeModelFactory
                .PrepareAddressAttributeValueModel(new AddressAttributeValueModel(), addressAttribute, null);

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> ValueCreatePopup(AddressAttributeValueModel model)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //try to get an address attribute with the specified id
            var addressAttribute = await _addressAttributeService.GetAddressAttributeById(model.AddressAttributeId);
            if (addressAttribute == null)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                var addressAttributeValue = model.ToEntity<AddressAttributeValue>();
                await _addressAttributeService.InsertAddressAttributeValue(addressAttributeValue);

                //activity log
                await _customerActivityService.InsertActivity("AddNewAddressAttributeValue",
                    string.Format(await _localizationService.GetResource("ActivityLog.AddNewAddressAttributeValue"), addressAttributeValue.Id),
                    addressAttributeValue);

                await UpdateValueLocales(addressAttributeValue, model);

                ViewBag.RefreshPage = true;

                return View(model);
            }

            //prepare model
            model = await _addressAttributeModelFactory.PrepareAddressAttributeValueModel(model, addressAttribute, null, true);
            
            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual async Task<IActionResult> ValueEditPopup(int id)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //try to get an address attribute value with the specified id
            var addressAttributeValue = await _addressAttributeService.GetAddressAttributeValueById(id);
            if (addressAttributeValue == null)
                return RedirectToAction("List");

            //try to get an address attribute with the specified id
            var addressAttribute = await _addressAttributeService.GetAddressAttributeById(addressAttributeValue.AddressAttributeId);
            if (addressAttribute == null)
                return RedirectToAction("List");

            //prepare model
            var model = await _addressAttributeModelFactory.PrepareAddressAttributeValueModel(null, addressAttribute, addressAttributeValue);

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> ValueEditPopup(AddressAttributeValueModel model)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //try to get an address attribute value with the specified id
            var addressAttributeValue = await _addressAttributeService.GetAddressAttributeValueById(model.Id);
            if (addressAttributeValue == null)
                return RedirectToAction("List");

            //try to get an address attribute with the specified id
            var addressAttribute = await _addressAttributeService.GetAddressAttributeById(addressAttributeValue.AddressAttributeId);
            if (addressAttribute == null)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                addressAttributeValue = model.ToEntity(addressAttributeValue);
                await _addressAttributeService.UpdateAddressAttributeValue(addressAttributeValue);

                await UpdateValueLocales(addressAttributeValue, model);

                //activity log
                await _customerActivityService.InsertActivity("EditAddressAttributeValue",
                    string.Format(await _localizationService.GetResource("ActivityLog.EditAddressAttributeValue"), addressAttributeValue.Id),
                    addressAttributeValue);

                ViewBag.RefreshPage = true;

                return View(model);
            }

            //prepare model
            model = await _addressAttributeModelFactory.PrepareAddressAttributeValueModel(model, addressAttribute, addressAttributeValue, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> ValueDelete(int id)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //try to get an address attribute value with the specified id
            var addressAttributeValue = await _addressAttributeService.GetAddressAttributeValueById(id)
                ?? throw new ArgumentException("No address attribute value found with the specified id", nameof(id));

            await _addressAttributeService.DeleteAddressAttributeValue(addressAttributeValue);

            //activity log
            await _customerActivityService.InsertActivity("DeleteAddressAttributeValue",
                string.Format(await _localizationService.GetResource("ActivityLog.DeleteAddressAttributeValue"), addressAttributeValue.Id),
                addressAttributeValue);

            return new NullJsonResult();
        }

        #endregion
    }
}