using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Nop.Web.Areas.Admin.Extensions;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Core.Domain.Catalog;
using Nop.Services.Catalog;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Security;
using Nop.Web.Framework.Kendoui;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Web.Areas.Admin.Controllers
{
    public partial class SpecificationAttributeController : BaseAdminController
    {
        #region Fields

        private readonly ISpecificationAttributeService _specificationAttributeService;
        private readonly ILanguageService _languageService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly ILocalizationService _localizationService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IPermissionService _permissionService;

        #endregion Fields

        #region Ctor

        public SpecificationAttributeController(ISpecificationAttributeService specificationAttributeService,
            ILanguageService languageService, 
            ILocalizedEntityService localizedEntityService,
            ILocalizationService localizationService, 
            ICustomerActivityService customerActivityService,
            IPermissionService permissionService)
        {
            this._specificationAttributeService = specificationAttributeService;
            this._languageService = languageService;
            this._localizedEntityService = localizedEntityService;
            this._localizationService = localizationService;
            this._customerActivityService = customerActivityService;
            this._permissionService = permissionService;
        }

        #endregion
        
        #region Utilities

        protected virtual void UpdateAttributeLocales(SpecificationAttribute specificationAttribute, SpecificationAttributeModel model)
        {
            foreach (var localized in model.Locales)
            {
                _localizedEntityService.SaveLocalizedValue(specificationAttribute,
                    x => x.Name,
                    localized.Name,
                    localized.LanguageId);
            }
        }

        protected virtual void UpdateOptionLocales(SpecificationAttributeOption specificationAttributeOption, SpecificationAttributeOptionModel model)
        {
            foreach (var localized in model.Locales)
            {
                _localizedEntityService.SaveLocalizedValue(specificationAttributeOption,
                    x => x.Name,
                    localized.Name,
                    localized.LanguageId);
            }
        }

        #endregion
        
        #region Specification attributes

        //list
        public virtual IActionResult Index()
        {
            return RedirectToAction("List");
        }

        public virtual IActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            return View();
        }

        [HttpPost]
        public virtual IActionResult List(DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedKendoGridJson();

            var specificationAttributes = _specificationAttributeService
                .GetSpecificationAttributes(command.Page - 1, command.PageSize);
            var gridModel = new DataSourceResult
            {
                Data = specificationAttributes.Select(x => x.ToModel()),
                Total = specificationAttributes.TotalCount
            };

            return Json(gridModel);
        }
        
        //create
        public virtual IActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            var model = new SpecificationAttributeModel();
            //locales
            AddLocales(_languageService, model.Locales);
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult Create(SpecificationAttributeModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var specificationAttribute = model.ToEntity();
                _specificationAttributeService.InsertSpecificationAttribute(specificationAttribute);
                UpdateAttributeLocales(specificationAttribute, model);

                //activity log
                _customerActivityService.InsertActivity("AddNewSpecAttribute", _localizationService.GetResource("ActivityLog.AddNewSpecAttribute"), specificationAttribute.Name);

                SuccessNotification(_localizationService.GetResource("Admin.Catalog.Attributes.SpecificationAttributes.Added"));

                if (continueEditing)
                {
                    //selected tab
                    SaveSelectedTabName();

                    return RedirectToAction("Edit", new { id = specificationAttribute.Id });
                }
                return RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }

        //edit
        public virtual IActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            var specificationAttribute = _specificationAttributeService.GetSpecificationAttributeById(id);
            if (specificationAttribute == null)
                //No specification attribute found with the specified id
                return RedirectToAction("List");

            var model = specificationAttribute.ToModel();
            //locales
            AddLocales(_languageService, model.Locales, (locale, languageId) =>
            {
                locale.Name = specificationAttribute.GetLocalized(x => x.Name, languageId, false, false);
            });

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult Edit(SpecificationAttributeModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            var specificationAttribute = _specificationAttributeService.GetSpecificationAttributeById(model.Id);
            if (specificationAttribute == null)
                //No specification attribute found with the specified id
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                specificationAttribute = model.ToEntity(specificationAttribute);
                _specificationAttributeService.UpdateSpecificationAttribute(specificationAttribute);

                UpdateAttributeLocales(specificationAttribute, model);

                //activity log
                _customerActivityService.InsertActivity("EditSpecAttribute", _localizationService.GetResource("ActivityLog.EditSpecAttribute"), specificationAttribute.Name);

                SuccessNotification(_localizationService.GetResource("Admin.Catalog.Attributes.SpecificationAttributes.Updated"));

                if (continueEditing)
                {
                    //selected tab
                    SaveSelectedTabName();

                    return RedirectToAction("Edit",  new {id = specificationAttribute.Id});
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
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            var specificationAttribute = _specificationAttributeService.GetSpecificationAttributeById(id);
            if (specificationAttribute == null)
                //No specification attribute found with the specified id
                return RedirectToAction("List");

            _specificationAttributeService.DeleteSpecificationAttribute(specificationAttribute);

            //activity log
            _customerActivityService.InsertActivity("DeleteSpecAttribute", _localizationService.GetResource("ActivityLog.DeleteSpecAttribute"), specificationAttribute.Name);

            SuccessNotification(_localizationService.GetResource("Admin.Catalog.Attributes.SpecificationAttributes.Deleted"));
            return RedirectToAction("List");
        }

        #endregion

        #region Specification attribute options

        //list
        [HttpPost]
        public virtual IActionResult OptionList(int specificationAttributeId, DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedKendoGridJson();

            var options = _specificationAttributeService.GetSpecificationAttributeOptionsBySpecificationAttribute(specificationAttributeId);
            var gridModel = new DataSourceResult
            {
                Data = options.Select(x => 
                    {
                        var model = x.ToModel();
                        //in order to save performance to do not check whether a product is deleted, etc
                        model.NumberOfAssociatedProducts = _specificationAttributeService
                            .GetProductSpecificationAttributeCount(0, x.Id);
                        //locales
                        //AddLocales(_languageService, model.Locales, (locale, languageId) =>
                        //{
                        //    locale.Name = x.GetLocalized(y => y.Name, languageId, false, false);
                        //});
                        return model;
                    }),
                Total = options.Count()
            };

            return Json(gridModel);
        }

        //create
        public virtual IActionResult OptionCreatePopup(int specificationAttributeId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            var model = new SpecificationAttributeOptionModel
            {
                SpecificationAttributeId = specificationAttributeId
            };
            //locales
            AddLocales(_languageService, model.Locales);
            return View(model);
        }

        [HttpPost]
        public virtual IActionResult OptionCreatePopup(SpecificationAttributeOptionModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            var specificationAttribute = _specificationAttributeService.GetSpecificationAttributeById(model.SpecificationAttributeId);
            if (specificationAttribute == null)
                //No specification attribute found with the specified id
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                var sao = model.ToEntity();
                //clear "Color" values if it's disabled
                if (!model.EnableColorSquaresRgb)
                    sao.ColorSquaresRgb = null;

                _specificationAttributeService.InsertSpecificationAttributeOption(sao);
                UpdateOptionLocales(sao, model);

                ViewBag.RefreshPage = true;
                return View(model);
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }

        //edit
        public virtual IActionResult OptionEditPopup(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            var sao = _specificationAttributeService.GetSpecificationAttributeOptionById(id);
            if (sao == null)
                //No specification attribute option found with the specified id
                return RedirectToAction("List");

            var model = sao.ToModel();
            //"Color" value
            model.EnableColorSquaresRgb = !string.IsNullOrEmpty(sao.ColorSquaresRgb);
            //locales
            AddLocales(_languageService, model.Locales, (locale, languageId) =>
            {
                locale.Name = sao.GetLocalized(x => x.Name, languageId, false, false);
            });

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult OptionEditPopup(SpecificationAttributeOptionModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            var sao = _specificationAttributeService.GetSpecificationAttributeOptionById(model.Id);
            if (sao == null)
                //No specification attribute option found with the specified id
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                sao = model.ToEntity(sao);
                //clear "Color" values if it's disabled
                if (!model.EnableColorSquaresRgb)
                    sao.ColorSquaresRgb = null;

                _specificationAttributeService.UpdateSpecificationAttributeOption(sao);

                UpdateOptionLocales(sao, model);

                ViewBag.RefreshPage = true;
                return View(model);
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }

        //delete
        [HttpPost]
        public virtual IActionResult OptionDelete(int id, int specificationAttributeId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            var sao = _specificationAttributeService.GetSpecificationAttributeOptionById(id);
            if (sao == null)
                throw new ArgumentException("No specification attribute option found with the specified id");

            _specificationAttributeService.DeleteSpecificationAttributeOption(sao);

            return new NullJsonResult();
        }

        //ajax
        [HttpGet]
        public virtual IActionResult GetOptionsByAttributeId(string attributeId)
        {
            //do not make any permission validation here 
            //because this method could be used on some other pages (such as product editing)
            //if (!_permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
            //    return AccessDeniedView();

            // This action method gets called via an ajax request
            if (string.IsNullOrEmpty(attributeId))
                throw new ArgumentNullException(nameof(attributeId));

            var options = _specificationAttributeService.GetSpecificationAttributeOptionsBySpecificationAttribute(Convert.ToInt32(attributeId));
            var result = (from o in options
                          select new { id = o.Id, name = o.Name }).ToList();
            return Json(result);
        }

        #endregion
    }
}