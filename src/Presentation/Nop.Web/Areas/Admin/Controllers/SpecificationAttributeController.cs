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

        protected virtual async Task UpdateAttributeLocalesAsync(SpecificationAttribute specificationAttribute, SpecificationAttributeModel model)
        {
            foreach (var localized in model.Locales)
            {
                await _localizedEntityService.SaveLocalizedValueAsync(specificationAttribute,
                    x => x.Name,
                    localized.Name,
                    localized.LanguageId);
            }
        }

        protected virtual async Task UpdateAttributeGroupLocalesAsync(SpecificationAttributeGroup specificationAttributeGroup, SpecificationAttributeGroupModel model)
        {
            foreach (var localized in model.Locales)
            {
                await _localizedEntityService.SaveLocalizedValueAsync(specificationAttributeGroup,
                    x => x.Name,
                    localized.Name,
                    localized.LanguageId);
            }
        }

        protected virtual async Task UpdateOptionLocalesAsync(SpecificationAttributeOption specificationAttributeOption, SpecificationAttributeOptionModel model)
        {
            foreach (var localized in model.Locales)
            {
                await _localizedEntityService.SaveLocalizedValueAsync(specificationAttributeOption,
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
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            var model = await _specificationAttributeModelFactory.PrepareSpecificationAttributeGroupSearchModelAsync(new SpecificationAttributeGroupSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> SpecificationAttributeGroupList(SpecificationAttributeGroupSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageAttributes))
                return await AccessDeniedDataTablesJson();

            var model = await _specificationAttributeModelFactory.PrepareSpecificationAttributeGroupListModelAsync(searchModel);

            return Json(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> SpecificationAttributeList(SpecificationAttributeSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageAttributes))
                return await AccessDeniedDataTablesJson();

            SpecificationAttributeGroup group = null;

            if (searchModel.SpecificationAttributeGroupId > 0)
            {
                group = await _specificationAttributeService.GetSpecificationAttributeGroupByIdAsync(searchModel.SpecificationAttributeGroupId)
                    ?? throw new ArgumentException("No specification attribute group found with the specified id");
            }

            var model = await _specificationAttributeModelFactory.PrepareSpecificationAttributeListModelAsync(searchModel, group);

            return Json(model);
        }

        public virtual async Task<IActionResult> CreateSpecificationAttributeGroup()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            var model = await _specificationAttributeModelFactory.PrepareSpecificationAttributeGroupModelAsync(new SpecificationAttributeGroupModel(), null);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> CreateSpecificationAttributeGroup(SpecificationAttributeGroupModel model, bool continueEditing)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var specificationAttributeGroup = model.ToEntity<SpecificationAttributeGroup>();
                await _specificationAttributeService.InsertSpecificationAttributeGroupAsync(specificationAttributeGroup);
                await UpdateAttributeGroupLocalesAsync(specificationAttributeGroup, model);

                await _customerActivityService.InsertActivityAsync("AddNewSpecAttributeGroup",
                    string.Format(await _localizationService.GetResourceAsync("ActivityLog.AddNewSpecAttributeGroup"), specificationAttributeGroup.Name), specificationAttributeGroup);

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Catalog.Attributes.SpecificationAttributes.SpecificationAttributeGroup.Added"));

                if (!continueEditing)
                    return RedirectToAction("List");

                return RedirectToAction("EditSpecificationAttributeGroup", new { id = specificationAttributeGroup.Id });
            }

            model = await _specificationAttributeModelFactory.PrepareSpecificationAttributeGroupModelAsync(model, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual async Task<IActionResult> CreateSpecificationAttribute()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            var model = await _specificationAttributeModelFactory.PrepareSpecificationAttributeModelAsync(new SpecificationAttributeModel(), null);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> CreateSpecificationAttribute(SpecificationAttributeModel model, bool continueEditing)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var specificationAttribute = model.ToEntity<SpecificationAttribute>();
                await _specificationAttributeService.InsertSpecificationAttributeAsync(specificationAttribute);
                await UpdateAttributeLocalesAsync(specificationAttribute, model);

                await _customerActivityService.InsertActivityAsync("AddNewSpecAttribute",
                    string.Format(await _localizationService.GetResourceAsync("ActivityLog.AddNewSpecAttribute"), specificationAttribute.Name), specificationAttribute);

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Catalog.Attributes.SpecificationAttributes.SpecificationAttribute.Added"));

                if (!continueEditing)
                    return RedirectToAction("List");
                
                return RedirectToAction("EditSpecificationAttribute", new { id = specificationAttribute.Id });
            }

            model = await _specificationAttributeModelFactory.PrepareSpecificationAttributeModelAsync(model, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual async Task<IActionResult> EditSpecificationAttributeGroup(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            var specificationAttributeGroup = await _specificationAttributeService.GetSpecificationAttributeGroupByIdAsync(id);
            if (specificationAttributeGroup == null)
                return RedirectToAction("List");

            var model = await _specificationAttributeModelFactory.PrepareSpecificationAttributeGroupModelAsync(null, specificationAttributeGroup);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> EditSpecificationAttributeGroup(SpecificationAttributeGroupModel model, bool continueEditing)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            var specificationAttributeGroup = await _specificationAttributeService.GetSpecificationAttributeGroupByIdAsync(model.Id);
            if (specificationAttributeGroup == null)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                specificationAttributeGroup = model.ToEntity(specificationAttributeGroup);
                await _specificationAttributeService.UpdateSpecificationAttributeGroupAsync(specificationAttributeGroup);
                await UpdateAttributeGroupLocalesAsync(specificationAttributeGroup, model);

                await _customerActivityService.InsertActivityAsync("EditSpecAttributeGroup",
                    string.Format(await _localizationService.GetResourceAsync("ActivityLog.EditSpecAttributeGroup"), specificationAttributeGroup.Name), specificationAttributeGroup);

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Catalog.Attributes.SpecificationAttributes.SpecificationAttributeGroup.Updated"));

                if (!continueEditing)
                    return RedirectToAction("List");

                return RedirectToAction("EditSpecificationAttributeGroup", new { id = specificationAttributeGroup.Id });
            }

            model = await _specificationAttributeModelFactory.PrepareSpecificationAttributeGroupModelAsync(model, specificationAttributeGroup, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual async Task<IActionResult> EditSpecificationAttribute(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            //try to get a specification attribute with the specified id
            var specificationAttribute = await _specificationAttributeService.GetSpecificationAttributeByIdAsync(id);
            if (specificationAttribute == null)
                return RedirectToAction("List");

            //prepare model
            var model = await _specificationAttributeModelFactory.PrepareSpecificationAttributeModelAsync(null, specificationAttribute);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> EditSpecificationAttribute(SpecificationAttributeModel model, bool continueEditing)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            //try to get a specification attribute with the specified id
            var specificationAttribute = await _specificationAttributeService.GetSpecificationAttributeByIdAsync(model.Id);
            if (specificationAttribute == null)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                specificationAttribute = model.ToEntity(specificationAttribute);
                await _specificationAttributeService.UpdateSpecificationAttributeAsync(specificationAttribute);

                await UpdateAttributeLocalesAsync(specificationAttribute, model);

                //activity log
                await _customerActivityService.InsertActivityAsync("EditSpecAttribute",
                    string.Format(await _localizationService.GetResourceAsync("ActivityLog.EditSpecAttribute"), specificationAttribute.Name), specificationAttribute);

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Catalog.Attributes.SpecificationAttributes.SpecificationAttribute.Updated"));

                if (!continueEditing)
                    return RedirectToAction("List");
                
                return RedirectToAction("EditSpecificationAttribute", new { id = specificationAttribute.Id });
            }

            //prepare model
            model = await _specificationAttributeModelFactory.PrepareSpecificationAttributeModelAsync(model, specificationAttribute, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> DeleteSpecificationAttributeGroup(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            var specificationAttributeGroup = await _specificationAttributeService.GetSpecificationAttributeGroupByIdAsync(id);
            if (specificationAttributeGroup == null)
                return RedirectToAction("List");

            await _specificationAttributeService.DeleteSpecificationAttributeGroupAsync(specificationAttributeGroup);

            await _customerActivityService.InsertActivityAsync("DeleteSpecAttributeGroup",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.DeleteSpecAttributeGroup"), specificationAttributeGroup.Name), specificationAttributeGroup);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Catalog.Attributes.SpecificationAttributes.SpecificationAttributeGroup.Deleted"));

            return RedirectToAction("List");
        }

        [HttpPost]
        public virtual async Task<IActionResult> DeleteSpecificationAttribute(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            var specificationAttribute = await _specificationAttributeService.GetSpecificationAttributeByIdAsync(id);

            if (specificationAttribute == null)
                return RedirectToAction("List");

            await _specificationAttributeService.DeleteSpecificationAttributeAsync(specificationAttribute);

            await _customerActivityService.InsertActivityAsync("DeleteSpecAttribute",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.DeleteSpecAttribute"), specificationAttribute.Name), specificationAttribute);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Catalog.Attributes.SpecificationAttributes.SpecificationAttribute.Deleted"));

            return RedirectToAction("List");
        }

        [HttpPost]
        public virtual async Task<IActionResult> DeleteSelectedSpecificationAttributes(ICollection<int> selectedIds)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            if (selectedIds == null || selectedIds.Count == 0)
                return NoContent();

            var specificationAttributes = await _specificationAttributeService.GetSpecificationAttributeByIdsAsync(selectedIds.ToArray());
            await _specificationAttributeService.DeleteSpecificationAttributesAsync(specificationAttributes);

            foreach (var specificationAttribute in specificationAttributes)
            {
                await _customerActivityService.InsertActivityAsync("DeleteSpecAttribute",
                    string.Format(await _localizationService.GetResourceAsync("ActivityLog.DeleteSpecAttribute"), specificationAttribute.Name), specificationAttribute);
            }

            return Json(new { Result = true });
        }

        #endregion

        #region Specification attribute options

        [HttpPost]
        public virtual async Task<IActionResult> OptionList(SpecificationAttributeOptionSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageAttributes))
                return await AccessDeniedDataTablesJson();

            //try to get a specification attribute with the specified id
            var specificationAttribute = await _specificationAttributeService.GetSpecificationAttributeByIdAsync(searchModel.SpecificationAttributeId)
                ?? throw new ArgumentException("No specification attribute found with the specified id");

            //prepare model
            var model = await _specificationAttributeModelFactory.PrepareSpecificationAttributeOptionListModelAsync(searchModel, specificationAttribute);

            return Json(model);
        }

        public virtual async Task<IActionResult> OptionCreatePopup(int specificationAttributeId)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            //try to get a specification attribute with the specified id
            var specificationAttribute = await _specificationAttributeService.GetSpecificationAttributeByIdAsync(specificationAttributeId);
            if (specificationAttribute == null)
                return RedirectToAction("List");

            //prepare model
            var model = await _specificationAttributeModelFactory
                .PrepareSpecificationAttributeOptionModelAsync(new SpecificationAttributeOptionModel(), specificationAttribute, null);

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> OptionCreatePopup(SpecificationAttributeOptionModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            //try to get a specification attribute with the specified id
            var specificationAttribute = await _specificationAttributeService.GetSpecificationAttributeByIdAsync(model.SpecificationAttributeId);
            if (specificationAttribute == null)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                var sao = model.ToEntity<SpecificationAttributeOption>();

                //clear "Color" values if it's disabled
                if (!model.EnableColorSquaresRgb)
                    sao.ColorSquaresRgb = null;

                await _specificationAttributeService.InsertSpecificationAttributeOptionAsync(sao);

                await UpdateOptionLocalesAsync(sao, model);

                ViewBag.RefreshPage = true;

                return View(model);
            }

            //prepare model
            model = await _specificationAttributeModelFactory.PrepareSpecificationAttributeOptionModelAsync(model, specificationAttribute, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual async Task<IActionResult> OptionEditPopup(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            //try to get a specification attribute option with the specified id
            var specificationAttributeOption = await _specificationAttributeService.GetSpecificationAttributeOptionByIdAsync(id);
            if (specificationAttributeOption == null)
                return RedirectToAction("List");

            //try to get a specification attribute with the specified id
            var specificationAttribute = await _specificationAttributeService
                .GetSpecificationAttributeByIdAsync(specificationAttributeOption.SpecificationAttributeId);
            if (specificationAttribute == null)
                return RedirectToAction("List");

            //prepare model
            var model = await _specificationAttributeModelFactory
                .PrepareSpecificationAttributeOptionModelAsync(null, specificationAttribute, specificationAttributeOption);

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> OptionEditPopup(SpecificationAttributeOptionModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            //try to get a specification attribute option with the specified id
            var specificationAttributeOption = await _specificationAttributeService.GetSpecificationAttributeOptionByIdAsync(model.Id);
            if (specificationAttributeOption == null)
                return RedirectToAction("List");

            //try to get a specification attribute with the specified id
            var specificationAttribute = await _specificationAttributeService
                .GetSpecificationAttributeByIdAsync(specificationAttributeOption.SpecificationAttributeId);
            if (specificationAttribute == null)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                specificationAttributeOption = model.ToEntity(specificationAttributeOption);

                //clear "Color" values if it's disabled
                if (!model.EnableColorSquaresRgb)
                    specificationAttributeOption.ColorSquaresRgb = null;

                await _specificationAttributeService.UpdateSpecificationAttributeOptionAsync(specificationAttributeOption);

                await UpdateOptionLocalesAsync(specificationAttributeOption, model);

                ViewBag.RefreshPage = true;

                return View(model);
            }

            //prepare model
            model = await _specificationAttributeModelFactory
                .PrepareSpecificationAttributeOptionModelAsync(model, specificationAttribute, specificationAttributeOption, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> OptionDelete(int id, int specificationAttributeId)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            //try to get a specification attribute option with the specified id
            var specificationAttributeOption = await _specificationAttributeService.GetSpecificationAttributeOptionByIdAsync(id)
                ?? throw new ArgumentException("No specification attribute option found with the specified id", nameof(id));

            await _specificationAttributeService.DeleteSpecificationAttributeOptionAsync(specificationAttributeOption);

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

            var options = await _specificationAttributeService.GetSpecificationAttributeOptionsBySpecificationAttributeAsync(Convert.ToInt32(attributeId));
            var result = (from o in options
                          select new { id = o.Id, name = o.Name }).ToList();
            return Json(result);
        }

        #endregion

        #region Mapped products

        [HttpPost]
        public virtual async Task<IActionResult> UsedByProducts(SpecificationAttributeProductSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageAttributes))
                return await AccessDeniedDataTablesJson();

            //try to get a specification attribute with the specified id
            var specificationAttribute = await _specificationAttributeService.GetSpecificationAttributeByIdAsync(searchModel.SpecificationAttributeId)
                ?? throw new ArgumentException("No specification attribute found with the specified id");

            //prepare model
            var model = await _specificationAttributeModelFactory.PrepareSpecificationAttributeProductListModelAsync(searchModel, specificationAttribute);

            return Json(model);
        }

        #endregion
    }
}