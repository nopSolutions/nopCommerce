using System;
using System.Linq;
using System.Web.Mvc;
using Nop.Admin.Extensions;
using Nop.Admin.Models.Common;
using Nop.Core;
using Nop.Core.Domain.Common;
using Nop.Services.Common;
using Nop.Services.Localization;
using Nop.Services.Security;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Kendoui;
using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Controllers
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

        #endregion

        #region Constructors

        public AddressAttributeController(IAddressAttributeService addressAttributeService,
            ILanguageService languageService, 
            ILocalizedEntityService localizedEntityService,
            ILocalizationService localizationService,
            IWorkContext workContext,
            IPermissionService permissionService)
        {
            this._addressAttributeService = addressAttributeService;
            this._languageService = languageService;
            this._localizedEntityService = localizedEntityService;
            this._localizationService = localizationService;
            this._workContext = workContext;
            this._permissionService = permissionService;
        }

        #endregion
        
        #region Utilities

        [NonAction]
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

        [NonAction]
        protected  virtual void UpdateValueLocales(AddressAttributeValue addressAttributeValue, AddressAttributeValueModel model)
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

        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public ActionResult ListBlock()
        {
            return PartialView("ListBlock");
        }

        public ActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //we just redirect a user to the address settings page
            
            //select third tab
            const int addressFormFieldIndex = 2;
            SaveSelectedTabIndex(addressFormFieldIndex);
            return RedirectToAction("CustomerUser", "Setting");
        }

        [HttpPost]
        public ActionResult List(DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

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
        public ActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            var model = new AddressAttributeModel();
            //locales
            AddLocales(_languageService, model.Locales);
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public ActionResult Create(AddressAttributeModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var addressAttribute = model.ToEntity();
                _addressAttributeService.InsertAddressAttribute(addressAttribute);
                //locales
                UpdateAttributeLocales(addressAttribute, model);

                SuccessNotification(_localizationService.GetResource("Admin.Address.AddressAttributes.Added"));
                return continueEditing ? RedirectToAction("Edit", new { id = addressAttribute.Id }) : RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }

        //edit
        public ActionResult Edit(int id)
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
        public ActionResult Edit(AddressAttributeModel model, bool continueEditing)
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
                //locales
                UpdateAttributeLocales(addressAttribute, model);

                SuccessNotification(_localizationService.GetResource("Admin.Address.AddressAttributes.Updated"));
                if (continueEditing)
                {
                    //selected tab
                    SaveSelectedTabIndex();

                    return RedirectToAction("Edit", addressAttribute.Id);
                }
                return RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }

        //delete
        [HttpPost]
        public ActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            var addressAttribute = _addressAttributeService.GetAddressAttributeById(id);
            _addressAttributeService.DeleteAddressAttribute(addressAttribute);

            SuccessNotification(_localizationService.GetResource("Admin.Address.AddressAttributes.Deleted"));
            return RedirectToAction("List");
        }

        #endregion

        #region Address attribute values

        //list
        [HttpPost]
        public ActionResult ValueList(int addressAttributeId, DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

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
        public ActionResult ValueCreatePopup(int addressAttributeId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            var addressAttribute = _addressAttributeService.GetAddressAttributeById(addressAttributeId);
            if (addressAttribute == null)
                //No address attribute found with the specified id
                return RedirectToAction("List");

            var model = new AddressAttributeValueModel();
            model.AddressAttributeId = addressAttributeId;
            //locales
            AddLocales(_languageService, model.Locales);
            return View(model);
        }

        [HttpPost]
        public ActionResult ValueCreatePopup(string btnId, string formId, AddressAttributeValueModel model)
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
                UpdateValueLocales(cav, model);

                ViewBag.RefreshPage = true;
                ViewBag.btnId = btnId;
                ViewBag.formId = formId;
                return View(model);
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }

        //edit
        public ActionResult ValueEditPopup(int id)
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
        public ActionResult ValueEditPopup(string btnId, string formId, AddressAttributeValueModel model)
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

                ViewBag.RefreshPage = true;
                ViewBag.btnId = btnId;
                ViewBag.formId = formId;
                return View(model);
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }

        //delete
        [HttpPost]
        public ActionResult ValueDelete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            var cav = _addressAttributeService.GetAddressAttributeValueById(id);
            if (cav == null)
                throw new ArgumentException("No address attribute value found with the specified id");
            _addressAttributeService.DeleteAddressAttributeValue(cav);

            return new NullJsonResult();
        }


        #endregion
    }
}
