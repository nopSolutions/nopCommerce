using System;
using System.Collections.Generic;
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

        protected virtual void UpdateAttributeGroupLocales(SpecificationAttributeGroup specificationAttributeGroup, SpecificationAttributeGroupModel model)
        {
            foreach (var localized in model.Locales)
            {
                _localizedEntityService.SaveLocalizedValue(specificationAttributeGroup,
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

            var model = _specificationAttributeModelFactory.PrepareSpecificationAttributeGroupSearchModel(new SpecificationAttributeGroupSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult SpecificationAttributeGroupList(SpecificationAttributeGroupSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedDataTablesJson();

            var model = _specificationAttributeModelFactory.PrepareSpecificationAttributeGroupListModel(searchModel);

            return Json(model);
        }

        [HttpPost]
        public virtual IActionResult SpecificationAttributeList(SpecificationAttributeSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedDataTablesJson();

            SpecificationAttributeGroup group = null;

            if (searchModel.SpecificationAttributeGroupId > 0)
            {
                group = _specificationAttributeService.GetSpecificationAttributeGroupById(searchModel.SpecificationAttributeGroupId)
                    ?? throw new ArgumentException("No specification attribute group found with the specified id");
            }

            var model = _specificationAttributeModelFactory.PrepareSpecificationAttributeListModel(searchModel, group);

            return Json(model);
        }

        public virtual IActionResult CreateSpecificationAttributeGroup()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            var model = _specificationAttributeModelFactory.PrepareSpecificationAttributeGroupModel(new SpecificationAttributeGroupModel(), null);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult CreateSpecificationAttributeGroup(SpecificationAttributeGroupModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var specificationAttributeGroup = model.ToEntity<SpecificationAttributeGroup>();
                _specificationAttributeService.InsertSpecificationAttributeGroup(specificationAttributeGroup);
                UpdateAttributeGroupLocales(specificationAttributeGroup, model);

                _customerActivityService.InsertActivity("AddNewSpecAttributeGroup",
                    string.Format(_localizationService.GetResource("ActivityLog.AddNewSpecAttributeGroup"), specificationAttributeGroup.Name), specificationAttributeGroup);

                _notificationService.SuccessNotification(_localizationService.GetResource("Admin.Catalog.Attributes.SpecificationAttributes.SpecificationAttributeGroup.Added"));

                if (!continueEditing)
                    return RedirectToAction("List");

                return RedirectToAction("EditSpecificationAttributeGroup", new { id = specificationAttributeGroup.Id });
            }

            model = _specificationAttributeModelFactory.PrepareSpecificationAttributeGroupModel(model, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual IActionResult CreateSpecificationAttribute()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            var model = _specificationAttributeModelFactory.PrepareSpecificationAttributeModel(new SpecificationAttributeModel(), null);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult CreateSpecificationAttribute(SpecificationAttributeModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var specificationAttribute = model.ToEntity<SpecificationAttribute>();
                _specificationAttributeService.InsertSpecificationAttribute(specificationAttribute);
                UpdateAttributeLocales(specificationAttribute, model);

                _customerActivityService.InsertActivity("AddNewSpecAttribute",
                    string.Format(_localizationService.GetResource("ActivityLog.AddNewSpecAttribute"), specificationAttribute.Name), specificationAttribute);

                _notificationService.SuccessNotification(_localizationService.GetResource("Admin.Catalog.Attributes.SpecificationAttributes.SpecificationAttribute.Added"));

                if (!continueEditing)
                    return RedirectToAction("List");
                
                return RedirectToAction("EditSpecificationAttribute", new { id = specificationAttribute.Id });
            }

            model = _specificationAttributeModelFactory.PrepareSpecificationAttributeModel(model, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual IActionResult EditSpecificationAttributeGroup(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            var specificationAttributeGroup = _specificationAttributeService.GetSpecificationAttributeGroupById(id);
            if (specificationAttributeGroup == null)
                return RedirectToAction("List");

            var model = _specificationAttributeModelFactory.PrepareSpecificationAttributeGroupModel(null, specificationAttributeGroup);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult EditSpecificationAttributeGroup(SpecificationAttributeGroupModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            var specificationAttributeGroup = _specificationAttributeService.GetSpecificationAttributeGroupById(model.Id);
            if (specificationAttributeGroup == null)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                specificationAttributeGroup = model.ToEntity(specificationAttributeGroup);
                _specificationAttributeService.UpdateSpecificationAttributeGroup(specificationAttributeGroup);
                UpdateAttributeGroupLocales(specificationAttributeGroup, model);

                _customerActivityService.InsertActivity("EditSpecAttributeGroup",
                    string.Format(_localizationService.GetResource("ActivityLog.EditSpecAttributeGroup"), specificationAttributeGroup.Name), specificationAttributeGroup);

                _notificationService.SuccessNotification(_localizationService.GetResource("Admin.Catalog.Attributes.SpecificationAttributes.SpecificationAttributeGroup.Updated"));

                if (!continueEditing)
                    return RedirectToAction("List");

                return RedirectToAction("EditSpecificationAttributeGroup", new { id = specificationAttributeGroup.Id });
            }

            model = _specificationAttributeModelFactory.PrepareSpecificationAttributeGroupModel(model, specificationAttributeGroup, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual IActionResult EditSpecificationAttribute(int id)
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
        public virtual IActionResult EditSpecificationAttribute(SpecificationAttributeModel model, bool continueEditing)
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

                _notificationService.SuccessNotification(_localizationService.GetResource("Admin.Catalog.Attributes.SpecificationAttributes.SpecificationAttribute.Updated"));

                if (!continueEditing)
                    return RedirectToAction("List");
                
                return RedirectToAction("EditSpecificationAttribute", new { id = specificationAttribute.Id });
            }

            //prepare model
            model = _specificationAttributeModelFactory.PrepareSpecificationAttributeModel(model, specificationAttribute, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual IActionResult DeleteSpecificationAttributeGroup(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            var specificationAttributeGroup = _specificationAttributeService.GetSpecificationAttributeGroupById(id);
            if (specificationAttributeGroup == null)
                return RedirectToAction("List");

            _specificationAttributeService.DeleteSpecificationAttributeGroup(specificationAttributeGroup);

            _customerActivityService.InsertActivity("DeleteSpecAttributeGroup",
                string.Format(_localizationService.GetResource("ActivityLog.DeleteSpecAttributeGroup"), specificationAttributeGroup.Name), specificationAttributeGroup);

            _notificationService.SuccessNotification(_localizationService.GetResource("Admin.Catalog.Attributes.SpecificationAttributes.SpecificationAttributeGroup.Deleted"));

            return RedirectToAction("List");
        }

        [HttpPost]
        public virtual IActionResult DeleteSpecificationAttribute(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            var specificationAttribute = _specificationAttributeService.GetSpecificationAttributeById(id);
            if (specificationAttribute == null)
                return RedirectToAction("List");

            _specificationAttributeService.DeleteSpecificationAttribute(specificationAttribute);

            _customerActivityService.InsertActivity("DeleteSpecAttribute",
                string.Format(_localizationService.GetResource("ActivityLog.DeleteSpecAttribute"), specificationAttribute.Name), specificationAttribute);

            _notificationService.SuccessNotification(_localizationService.GetResource("Admin.Catalog.Attributes.SpecificationAttributes.SpecificationAttribute.Deleted"));

            return RedirectToAction("List");
        }

        [HttpPost]
        public virtual IActionResult DeleteSelectedSpecificationAttributes(ICollection<int> selectedIds)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            if (selectedIds != null)
            {
                var specificationAttributes = _specificationAttributeService.GetSpecificationAttributeByIds(selectedIds.ToArray());
                _specificationAttributeService.DeleteSpecificationAttributes(specificationAttributes);

                foreach (var specificationAttribute in specificationAttributes)
                {
                    _customerActivityService.InsertActivity("DeleteSpecAttribute",
                        string.Format(_localizationService.GetResource("ActivityLog.DeleteSpecAttribute"), specificationAttribute.Name), specificationAttribute);
                }
            }

            return Json(new { Result = true });
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