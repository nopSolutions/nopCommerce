using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Catalog;
using Nop.Services.Catalog;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Web.Areas.Admin.Controllers
{
    public partial class SpecificationAttributeController : BaseAdminController
    {
        #region Fields

        private readonly ICustomerActivityService _customerActivityService;
        private readonly ILocalizationService _localizationService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly INotificationService _notificationService;
        private readonly IPermissionService _permissionService;
        private readonly ISpecificationAttributeModelFactory _specificationAttributeModelFactory;
        private readonly ISpecificationAttributeService _specificationAttributeService;

        #endregion Fields

        #region Ctor

        public SpecificationAttributeController(ICustomerActivityService customerActivityService,
            ILocalizationService localizationService,
            ILocalizedEntityService localizedEntityService,
            INotificationService notificationService,
            IPermissionService permissionService,
            ISpecificationAttributeModelFactory specificationAttributeModelFactory,
            ISpecificationAttributeService specificationAttributeService)
        {
            _customerActivityService = customerActivityService;
            _localizationService = localizationService;
            _localizedEntityService = localizedEntityService;
            _notificationService = notificationService;
            _permissionService = permissionService;
            _specificationAttributeModelFactory = specificationAttributeModelFactory;
            _specificationAttributeService = specificationAttributeService;
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

        public virtual IActionResult Index()
        {
            return RedirectToAction("List");
        }

        public virtual IActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            //prepare model
            var model = _specificationAttributeModelFactory.PrepareSpecificationAttributeSearchModel(new SpecificationAttributeSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult List(SpecificationAttributeSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedDataTablesJson();

            //prepare model
            var model = _specificationAttributeModelFactory.PrepareSpecificationAttributeListModel(searchModel);

            return Json(model);
        }

        public virtual IActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            //prepare model
            var model = _specificationAttributeModelFactory.PrepareSpecificationAttributeModel(new SpecificationAttributeModel(), null);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult Create(SpecificationAttributeModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var specificationAttribute = model.ToEntity<SpecificationAttribute>();
                _specificationAttributeService.InsertSpecificationAttribute(specificationAttribute);
                UpdateAttributeLocales(specificationAttribute, model);

                //activity log
                _customerActivityService.InsertActivity("AddNewSpecAttribute",
                    string.Format(_localizationService.GetResource("ActivityLog.AddNewSpecAttribute"), specificationAttribute.Name), specificationAttribute);

                _notificationService.SuccessNotification(_localizationService.GetResource("Admin.Catalog.Attributes.SpecificationAttributes.Added"));

                if (!continueEditing)
                    return RedirectToAction("List");
                
                return RedirectToAction("Edit", new { id = specificationAttribute.Id });
            }

            //prepare model
            model = _specificationAttributeModelFactory.PrepareSpecificationAttributeModel(model, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        //edit
        public virtual IActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            //try to get a specification attribute with the specified id
            var specificationAttribute = _specificationAttributeService.GetSpecificationAttributeById(id);
            if (specificationAttribute == null)
                return RedirectToAction("List");

            //prepare model
            var model = _specificationAttributeModelFactory.PrepareSpecificationAttributeModel(null, specificationAttribute);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult Edit(SpecificationAttributeModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            //try to get a specification attribute with the specified id
            var specificationAttribute = _specificationAttributeService.GetSpecificationAttributeById(model.Id);
            if (specificationAttribute == null)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                specificationAttribute = model.ToEntity(specificationAttribute);
                _specificationAttributeService.UpdateSpecificationAttribute(specificationAttribute);

                UpdateAttributeLocales(specificationAttribute, model);

                //activity log
                _customerActivityService.InsertActivity("EditSpecAttribute",
                    string.Format(_localizationService.GetResource("ActivityLog.EditSpecAttribute"), specificationAttribute.Name), specificationAttribute);

                _notificationService.SuccessNotification(_localizationService.GetResource("Admin.Catalog.Attributes.SpecificationAttributes.Updated"));

                if (!continueEditing)
                    return RedirectToAction("List");
                
                return RedirectToAction("Edit", new { id = specificationAttribute.Id });
            }

            //prepare model
            model = _specificationAttributeModelFactory.PrepareSpecificationAttributeModel(model, specificationAttribute, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual IActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            //try to get a specification attribute with the specified id
            var specificationAttribute = _specificationAttributeService.GetSpecificationAttributeById(id);
            if (specificationAttribute == null)
                return RedirectToAction("List");

            _specificationAttributeService.DeleteSpecificationAttribute(specificationAttribute);

            //activity log
            _customerActivityService.InsertActivity("DeleteSpecAttribute",
                string.Format(_localizationService.GetResource("ActivityLog.DeleteSpecAttribute"), specificationAttribute.Name), specificationAttribute);

            _notificationService.SuccessNotification(_localizationService.GetResource("Admin.Catalog.Attributes.SpecificationAttributes.Deleted"));

            return RedirectToAction("List");
        }

        #endregion

        #region Specification attribute options

        [HttpPost]
        public virtual IActionResult OptionList(SpecificationAttributeOptionSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedDataTablesJson();

            //try to get a specification attribute with the specified id
            var specificationAttribute = _specificationAttributeService.GetSpecificationAttributeById(searchModel.SpecificationAttributeId)
                ?? throw new ArgumentException("No specification attribute found with the specified id");

            //prepare model
            var model = _specificationAttributeModelFactory.PrepareSpecificationAttributeOptionListModel(searchModel, specificationAttribute);

            return Json(model);
        }

        public virtual IActionResult OptionCreatePopup(int specificationAttributeId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            //try to get a specification attribute with the specified id
            var specificationAttribute = _specificationAttributeService.GetSpecificationAttributeById(specificationAttributeId);
            if (specificationAttribute == null)
                return RedirectToAction("List");

            //prepare model
            var model = _specificationAttributeModelFactory
                .PrepareSpecificationAttributeOptionModel(new SpecificationAttributeOptionModel(), specificationAttribute, null);

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult OptionCreatePopup(SpecificationAttributeOptionModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            //try to get a specification attribute with the specified id
            var specificationAttribute = _specificationAttributeService.GetSpecificationAttributeById(model.SpecificationAttributeId);
            if (specificationAttribute == null)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                var sao = model.ToEntity<SpecificationAttributeOption>();

                //clear "Color" values if it's disabled
                if (!model.EnableColorSquaresRgb)
                    sao.ColorSquaresRgb = null;

                _specificationAttributeService.InsertSpecificationAttributeOption(sao);

                UpdateOptionLocales(sao, model);

                ViewBag.RefreshPage = true;

                return View(model);
            }

            //prepare model
            model = _specificationAttributeModelFactory.PrepareSpecificationAttributeOptionModel(model, specificationAttribute, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual IActionResult OptionEditPopup(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            //try to get a specification attribute option with the specified id
            var specificationAttributeOption = _specificationAttributeService.GetSpecificationAttributeOptionById(id);
            if (specificationAttributeOption == null)
                return RedirectToAction("List");

            //try to get a specification attribute with the specified id
            var specificationAttribute = _specificationAttributeService
                .GetSpecificationAttributeById(specificationAttributeOption.SpecificationAttributeId);
            if (specificationAttribute == null)
                return RedirectToAction("List");

            //prepare model
            var model = _specificationAttributeModelFactory
                .PrepareSpecificationAttributeOptionModel(null, specificationAttribute, specificationAttributeOption);

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult OptionEditPopup(SpecificationAttributeOptionModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            //try to get a specification attribute option with the specified id
            var specificationAttributeOption = _specificationAttributeService.GetSpecificationAttributeOptionById(model.Id);
            if (specificationAttributeOption == null)
                return RedirectToAction("List");

            //try to get a specification attribute with the specified id
            var specificationAttribute = _specificationAttributeService
                .GetSpecificationAttributeById(specificationAttributeOption.SpecificationAttributeId);
            if (specificationAttribute == null)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                specificationAttributeOption = model.ToEntity(specificationAttributeOption);

                //clear "Color" values if it's disabled
                if (!model.EnableColorSquaresRgb)
                    specificationAttributeOption.ColorSquaresRgb = null;

                _specificationAttributeService.UpdateSpecificationAttributeOption(specificationAttributeOption);

                UpdateOptionLocales(specificationAttributeOption, model);

                ViewBag.RefreshPage = true;

                return View(model);
            }

            //prepare model
            model = _specificationAttributeModelFactory
                .PrepareSpecificationAttributeOptionModel(model, specificationAttribute, specificationAttributeOption, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual IActionResult OptionDelete(int id, int specificationAttributeId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            //try to get a specification attribute option with the specified id
            var specificationAttributeOption = _specificationAttributeService.GetSpecificationAttributeOptionById(id)
                ?? throw new ArgumentException("No specification attribute option found with the specified id", nameof(id));

            _specificationAttributeService.DeleteSpecificationAttributeOption(specificationAttributeOption);

            return new NullJsonResult();
        }

        [HttpGet]
        public virtual IActionResult GetOptionsByAttributeId(string attributeId)
        {
            //do not make any permission validation here 
            //because this method could be used on some other pages (such as product editing)
            //if (!_permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
            //    return AccessDeniedView();

            //this action method gets called via an ajax request
            if (string.IsNullOrEmpty(attributeId))
                throw new ArgumentNullException(nameof(attributeId));

            var options = _specificationAttributeService.GetSpecificationAttributeOptionsBySpecificationAttribute(Convert.ToInt32(attributeId));
            var result = (from o in options
                          select new { id = o.Id, name = o.Name }).ToList();
            return Json(result);
        }

        #endregion

        #region Mapped products

        [HttpPost]
        public virtual IActionResult UsedByProducts(SpecificationAttributeProductSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedDataTablesJson();

            //try to get a specification attribute with the specified id
            var specificationAttribute = _specificationAttributeService.GetSpecificationAttributeById(searchModel.SpecificationAttributeId)
                ?? throw new ArgumentException("No specification attribute found with the specified id");

            //prepare model
            var model = _specificationAttributeModelFactory.PrepareSpecificationAttributeProductListModel(searchModel, specificationAttribute);

            return Json(model);
        }

        #endregion
    }
}