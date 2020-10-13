using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        protected virtual async Task UpdateAttributeLocales(SpecificationAttribute specificationAttribute, SpecificationAttributeModel model)
        {
            foreach (var localized in model.Locales)
            {
                await _localizedEntityService.SaveLocalizedValue(specificationAttribute,
                    x => x.Name,
                    localized.Name,
                    localized.LanguageId);
            }
        }

        protected virtual async Task UpdateAttributeGroupLocales(SpecificationAttributeGroup specificationAttributeGroup, SpecificationAttributeGroupModel model)
        {
            foreach (var localized in model.Locales)
            {
                await _localizedEntityService.SaveLocalizedValue(specificationAttributeGroup,
                    x => x.Name,
                    localized.Name,
                    localized.LanguageId);
            }
        }

        protected virtual async Task UpdateOptionLocales(SpecificationAttributeOption specificationAttributeOption, SpecificationAttributeOptionModel model)
        {
            foreach (var localized in model.Locales)
            {
                await _localizedEntityService.SaveLocalizedValue(specificationAttributeOption,
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

        public virtual async Task<IActionResult> List()
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            var model = await _specificationAttributeModelFactory.PrepareSpecificationAttributeGroupSearchModel(new SpecificationAttributeGroupSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> SpecificationAttributeGroupList(SpecificationAttributeGroupSearchModel searchModel)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedDataTablesJson();

            var model = await _specificationAttributeModelFactory.PrepareSpecificationAttributeGroupListModel(searchModel);

            return Json(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> SpecificationAttributeList(SpecificationAttributeSearchModel searchModel)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedDataTablesJson();

            SpecificationAttributeGroup group = null;

            if (searchModel.SpecificationAttributeGroupId > 0)
            {
                group = await _specificationAttributeService.GetSpecificationAttributeGroupById(searchModel.SpecificationAttributeGroupId)
                    ?? throw new ArgumentException("No specification attribute group found with the specified id");
            }

            var model = await _specificationAttributeModelFactory.PrepareSpecificationAttributeListModel(searchModel, group);

            return Json(model);
        }

        public virtual async Task<IActionResult> CreateSpecificationAttributeGroup()
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            var model = await _specificationAttributeModelFactory.PrepareSpecificationAttributeGroupModel(new SpecificationAttributeGroupModel(), null);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> CreateSpecificationAttributeGroup(SpecificationAttributeGroupModel model, bool continueEditing)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var specificationAttributeGroup = model.ToEntity<SpecificationAttributeGroup>();
                await _specificationAttributeService.InsertSpecificationAttributeGroup(specificationAttributeGroup);
                await UpdateAttributeGroupLocales(specificationAttributeGroup, model);

                await _customerActivityService.InsertActivity("AddNewSpecAttributeGroup",
                    string.Format(await _localizationService.GetResource("ActivityLog.AddNewSpecAttributeGroup"), specificationAttributeGroup.Name), specificationAttributeGroup);

                _notificationService.SuccessNotification(await _localizationService.GetResource("Admin.Catalog.Attributes.SpecificationAttributes.SpecificationAttributeGroup.Added"));

                if (!continueEditing)
                    return RedirectToAction("List");

                return RedirectToAction("EditSpecificationAttributeGroup", new { id = specificationAttributeGroup.Id });
            }

            model = await _specificationAttributeModelFactory.PrepareSpecificationAttributeGroupModel(model, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual async Task<IActionResult> CreateSpecificationAttribute()
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            var model = await _specificationAttributeModelFactory.PrepareSpecificationAttributeModel(new SpecificationAttributeModel(), null);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> CreateSpecificationAttribute(SpecificationAttributeModel model, bool continueEditing)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var specificationAttribute = model.ToEntity<SpecificationAttribute>();
                await _specificationAttributeService.InsertSpecificationAttribute(specificationAttribute);
                await UpdateAttributeLocales(specificationAttribute, model);

                await _customerActivityService.InsertActivity("AddNewSpecAttribute",
                    string.Format(await _localizationService.GetResource("ActivityLog.AddNewSpecAttribute"), specificationAttribute.Name), specificationAttribute);

                _notificationService.SuccessNotification(await _localizationService.GetResource("Admin.Catalog.Attributes.SpecificationAttributes.SpecificationAttribute.Added"));

                if (!continueEditing)
                    return RedirectToAction("List");
                
                return RedirectToAction("EditSpecificationAttribute", new { id = specificationAttribute.Id });
            }

            model = await _specificationAttributeModelFactory.PrepareSpecificationAttributeModel(model, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual async Task<IActionResult> EditSpecificationAttributeGroup(int id)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            var specificationAttributeGroup = await _specificationAttributeService.GetSpecificationAttributeGroupById(id);
            if (specificationAttributeGroup == null)
                return RedirectToAction("List");

            var model = await _specificationAttributeModelFactory.PrepareSpecificationAttributeGroupModel(null, specificationAttributeGroup);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> EditSpecificationAttributeGroup(SpecificationAttributeGroupModel model, bool continueEditing)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            var specificationAttributeGroup = await _specificationAttributeService.GetSpecificationAttributeGroupById(model.Id);
            if (specificationAttributeGroup == null)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                specificationAttributeGroup = model.ToEntity(specificationAttributeGroup);
                await _specificationAttributeService.UpdateSpecificationAttributeGroup(specificationAttributeGroup);
                await UpdateAttributeGroupLocales(specificationAttributeGroup, model);

                await _customerActivityService.InsertActivity("EditSpecAttributeGroup",
                    string.Format(await _localizationService.GetResource("ActivityLog.EditSpecAttributeGroup"), specificationAttributeGroup.Name), specificationAttributeGroup);

                _notificationService.SuccessNotification(await _localizationService.GetResource("Admin.Catalog.Attributes.SpecificationAttributes.SpecificationAttributeGroup.Updated"));

                if (!continueEditing)
                    return RedirectToAction("List");

                return RedirectToAction("EditSpecificationAttributeGroup", new { id = specificationAttributeGroup.Id });
            }

            model = await _specificationAttributeModelFactory.PrepareSpecificationAttributeGroupModel(model, specificationAttributeGroup, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual async Task<IActionResult> EditSpecificationAttribute(int id)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            //try to get a specification attribute with the specified id
            var specificationAttribute = await _specificationAttributeService.GetSpecificationAttributeById(id);
            if (specificationAttribute == null)
                return RedirectToAction("List");

            //prepare model
            var model = await _specificationAttributeModelFactory.PrepareSpecificationAttributeModel(null, specificationAttribute);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> EditSpecificationAttribute(SpecificationAttributeModel model, bool continueEditing)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            //try to get a specification attribute with the specified id
            var specificationAttribute = await _specificationAttributeService.GetSpecificationAttributeById(model.Id);
            if (specificationAttribute == null)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                specificationAttribute = model.ToEntity(specificationAttribute);
                await _specificationAttributeService.UpdateSpecificationAttribute(specificationAttribute);

                await UpdateAttributeLocales(specificationAttribute, model);

                //activity log
                await _customerActivityService.InsertActivity("EditSpecAttribute",
                    string.Format(await _localizationService.GetResource("ActivityLog.EditSpecAttribute"), specificationAttribute.Name), specificationAttribute);

                _notificationService.SuccessNotification(await _localizationService.GetResource("Admin.Catalog.Attributes.SpecificationAttributes.SpecificationAttribute.Updated"));

                if (!continueEditing)
                    return RedirectToAction("List");
                
                return RedirectToAction("EditSpecificationAttribute", new { id = specificationAttribute.Id });
            }

            //prepare model
            model = await _specificationAttributeModelFactory.PrepareSpecificationAttributeModel(model, specificationAttribute, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> DeleteSpecificationAttributeGroup(int id)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            var specificationAttributeGroup = await _specificationAttributeService.GetSpecificationAttributeGroupById(id);
            if (specificationAttributeGroup == null)
                return RedirectToAction("List");

            await _specificationAttributeService.DeleteSpecificationAttributeGroup(specificationAttributeGroup);

            await _customerActivityService.InsertActivity("DeleteSpecAttributeGroup",
                string.Format(await _localizationService.GetResource("ActivityLog.DeleteSpecAttributeGroup"), specificationAttributeGroup.Name), specificationAttributeGroup);

            _notificationService.SuccessNotification(await _localizationService.GetResource("Admin.Catalog.Attributes.SpecificationAttributes.SpecificationAttributeGroup.Deleted"));

            return RedirectToAction("List");
        }

        [HttpPost]
        public virtual async Task<IActionResult> DeleteSpecificationAttribute(int id)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            var specificationAttribute = await _specificationAttributeService.GetSpecificationAttributeById(id);

            if (specificationAttribute == null)
                return RedirectToAction("List");

            await _specificationAttributeService.DeleteSpecificationAttribute(specificationAttribute);

            await _customerActivityService.InsertActivity("DeleteSpecAttribute",
                string.Format(await _localizationService.GetResource("ActivityLog.DeleteSpecAttribute"), specificationAttribute.Name), specificationAttribute);

            _notificationService.SuccessNotification(await _localizationService.GetResource("Admin.Catalog.Attributes.SpecificationAttributes.SpecificationAttribute.Deleted"));

            return RedirectToAction("List");
        }

        [HttpPost]
        public virtual async Task<IActionResult> DeleteSelectedSpecificationAttributes(ICollection<int> selectedIds)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            if (selectedIds != null)
            {
                var specificationAttributes = await _specificationAttributeService.GetSpecificationAttributeByIds(selectedIds.ToArray());
                await _specificationAttributeService.DeleteSpecificationAttributes(specificationAttributes);

                foreach (var specificationAttribute in specificationAttributes)
                {
                    await _customerActivityService.InsertActivity("DeleteSpecAttribute",
                        string.Format(await _localizationService.GetResource("ActivityLog.DeleteSpecAttribute"), specificationAttribute.Name), specificationAttribute);
                }
            }

            return Json(new { Result = true });
        }

        #endregion

        #region Specification attribute options

        [HttpPost]
        public virtual async Task<IActionResult> OptionList(SpecificationAttributeOptionSearchModel searchModel)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedDataTablesJson();

            //try to get a specification attribute with the specified id
            var specificationAttribute = await _specificationAttributeService.GetSpecificationAttributeById(searchModel.SpecificationAttributeId)
                ?? throw new ArgumentException("No specification attribute found with the specified id");

            //prepare model
            var model = await _specificationAttributeModelFactory.PrepareSpecificationAttributeOptionListModel(searchModel, specificationAttribute);

            return Json(model);
        }

        public virtual async Task<IActionResult> OptionCreatePopup(int specificationAttributeId)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            //try to get a specification attribute with the specified id
            var specificationAttribute = await _specificationAttributeService.GetSpecificationAttributeById(specificationAttributeId);
            if (specificationAttribute == null)
                return RedirectToAction("List");

            //prepare model
            var model = await _specificationAttributeModelFactory
                .PrepareSpecificationAttributeOptionModel(new SpecificationAttributeOptionModel(), specificationAttribute, null);

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> OptionCreatePopup(SpecificationAttributeOptionModel model)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            //try to get a specification attribute with the specified id
            var specificationAttribute = await _specificationAttributeService.GetSpecificationAttributeById(model.SpecificationAttributeId);
            if (specificationAttribute == null)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                var sao = model.ToEntity<SpecificationAttributeOption>();

                //clear "Color" values if it's disabled
                if (!model.EnableColorSquaresRgb)
                    sao.ColorSquaresRgb = null;

                await _specificationAttributeService.InsertSpecificationAttributeOption(sao);

                await UpdateOptionLocales(sao, model);

                ViewBag.RefreshPage = true;

                return View(model);
            }

            //prepare model
            model = await _specificationAttributeModelFactory.PrepareSpecificationAttributeOptionModel(model, specificationAttribute, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual async Task<IActionResult> OptionEditPopup(int id)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            //try to get a specification attribute option with the specified id
            var specificationAttributeOption = await _specificationAttributeService.GetSpecificationAttributeOptionById(id);
            if (specificationAttributeOption == null)
                return RedirectToAction("List");

            //try to get a specification attribute with the specified id
            var specificationAttribute = await _specificationAttributeService
                .GetSpecificationAttributeById(specificationAttributeOption.SpecificationAttributeId);
            if (specificationAttribute == null)
                return RedirectToAction("List");

            //prepare model
            var model = await _specificationAttributeModelFactory
                .PrepareSpecificationAttributeOptionModel(null, specificationAttribute, specificationAttributeOption);

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> OptionEditPopup(SpecificationAttributeOptionModel model)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            //try to get a specification attribute option with the specified id
            var specificationAttributeOption = await _specificationAttributeService.GetSpecificationAttributeOptionById(model.Id);
            if (specificationAttributeOption == null)
                return RedirectToAction("List");

            //try to get a specification attribute with the specified id
            var specificationAttribute = await _specificationAttributeService
                .GetSpecificationAttributeById(specificationAttributeOption.SpecificationAttributeId);
            if (specificationAttribute == null)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                specificationAttributeOption = model.ToEntity(specificationAttributeOption);

                //clear "Color" values if it's disabled
                if (!model.EnableColorSquaresRgb)
                    specificationAttributeOption.ColorSquaresRgb = null;

                await _specificationAttributeService.UpdateSpecificationAttributeOption(specificationAttributeOption);

                await UpdateOptionLocales(specificationAttributeOption, model);

                ViewBag.RefreshPage = true;

                return View(model);
            }

            //prepare model
            model = await _specificationAttributeModelFactory
                .PrepareSpecificationAttributeOptionModel(model, specificationAttribute, specificationAttributeOption, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> OptionDelete(int id, int specificationAttributeId)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            //try to get a specification attribute option with the specified id
            var specificationAttributeOption = await _specificationAttributeService.GetSpecificationAttributeOptionById(id)
                ?? throw new ArgumentException("No specification attribute option found with the specified id", nameof(id));

            await _specificationAttributeService.DeleteSpecificationAttributeOption(specificationAttributeOption);

            return new NullJsonResult();
        }

        [HttpGet]
        public virtual async Task<IActionResult> GetOptionsByAttributeId(string attributeId)
        {
            //do not make any permission validation here 
            //because this method could be used on some other pages (such as product editing)
            //if (!await _permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
            //    return AccessDeniedView();

            //this action method gets called via an ajax request
            if (string.IsNullOrEmpty(attributeId))
                throw new ArgumentNullException(nameof(attributeId));

            var options = await _specificationAttributeService.GetSpecificationAttributeOptionsBySpecificationAttribute(Convert.ToInt32(attributeId));
            var result = (from o in options
                          select new { id = o.Id, name = o.Name }).ToList();
            return Json(result);
        }

        #endregion

        #region Mapped products

        [HttpPost]
        public virtual async Task<IActionResult> UsedByProducts(SpecificationAttributeProductSearchModel searchModel)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedDataTablesJson();

            //try to get a specification attribute with the specified id
            var specificationAttribute = await _specificationAttributeService.GetSpecificationAttributeById(searchModel.SpecificationAttributeId)
                ?? throw new ArgumentException("No specification attribute found with the specified id");

            //prepare model
            var model = await _specificationAttributeModelFactory.PrepareSpecificationAttributeProductListModel(searchModel, specificationAttribute);

            return Json(model);
        }

        #endregion
    }
}