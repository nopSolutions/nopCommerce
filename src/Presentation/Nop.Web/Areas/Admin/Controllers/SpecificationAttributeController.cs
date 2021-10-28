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

        protected ICustomerActivityService CustomerActivityService { get; }
        protected ILocalizationService LocalizationService { get; }
        protected ILocalizedEntityService LocalizedEntityService { get; }
        protected INotificationService NotificationService { get; }
        protected IPermissionService PermissionService { get; }
        protected ISpecificationAttributeModelFactory SpecificationAttributeModelFactory { get; }
        protected ISpecificationAttributeService SpecificationAttributeService { get; }

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
            CustomerActivityService = customerActivityService;
            LocalizationService = localizationService;
            LocalizedEntityService = localizedEntityService;
            NotificationService = notificationService;
            PermissionService = permissionService;
            SpecificationAttributeModelFactory = specificationAttributeModelFactory;
            SpecificationAttributeService = specificationAttributeService;
        }

        #endregion

        #region Utilities

        protected virtual async Task UpdateAttributeLocalesAsync(SpecificationAttribute specificationAttribute, SpecificationAttributeModel model)
        {
            foreach (var localized in model.Locales)
            {
                await LocalizedEntityService.SaveLocalizedValueAsync(specificationAttribute,
                    x => x.Name,
                    localized.Name,
                    localized.LanguageId);
            }
        }

        protected virtual async Task UpdateAttributeGroupLocalesAsync(SpecificationAttributeGroup specificationAttributeGroup, SpecificationAttributeGroupModel model)
        {
            foreach (var localized in model.Locales)
            {
                await LocalizedEntityService.SaveLocalizedValueAsync(specificationAttributeGroup,
                    x => x.Name,
                    localized.Name,
                    localized.LanguageId);
            }
        }

        protected virtual async Task UpdateOptionLocalesAsync(SpecificationAttributeOption specificationAttributeOption, SpecificationAttributeOptionModel model)
        {
            foreach (var localized in model.Locales)
            {
                await LocalizedEntityService.SaveLocalizedValueAsync(specificationAttributeOption,
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
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            var model = await SpecificationAttributeModelFactory.PrepareSpecificationAttributeGroupSearchModelAsync(new SpecificationAttributeGroupSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> SpecificationAttributeGroupList(SpecificationAttributeGroupSearchModel searchModel)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageAttributes))
                return await AccessDeniedDataTablesJson();

            var model = await SpecificationAttributeModelFactory.PrepareSpecificationAttributeGroupListModelAsync(searchModel);

            return Json(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> SpecificationAttributeList(SpecificationAttributeSearchModel searchModel)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageAttributes))
                return await AccessDeniedDataTablesJson();

            SpecificationAttributeGroup group = null;

            if (searchModel.SpecificationAttributeGroupId > 0)
            {
                group = await SpecificationAttributeService.GetSpecificationAttributeGroupByIdAsync(searchModel.SpecificationAttributeGroupId)
                    ?? throw new ArgumentException("No specification attribute group found with the specified id");
            }

            var model = await SpecificationAttributeModelFactory.PrepareSpecificationAttributeListModelAsync(searchModel, group);

            return Json(model);
        }

        public virtual async Task<IActionResult> CreateSpecificationAttributeGroup()
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            var model = await SpecificationAttributeModelFactory.PrepareSpecificationAttributeGroupModelAsync(new SpecificationAttributeGroupModel(), null);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> CreateSpecificationAttributeGroup(SpecificationAttributeGroupModel model, bool continueEditing)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var specificationAttributeGroup = model.ToEntity<SpecificationAttributeGroup>();
                await SpecificationAttributeService.InsertSpecificationAttributeGroupAsync(specificationAttributeGroup);
                await UpdateAttributeGroupLocalesAsync(specificationAttributeGroup, model);

                await CustomerActivityService.InsertActivityAsync("AddNewSpecAttributeGroup",
                    string.Format(await LocalizationService.GetResourceAsync("ActivityLog.AddNewSpecAttributeGroup"), specificationAttributeGroup.Name), specificationAttributeGroup);

                NotificationService.SuccessNotification(await LocalizationService.GetResourceAsync("Admin.Catalog.Attributes.SpecificationAttributes.SpecificationAttributeGroup.Added"));

                if (!continueEditing)
                    return RedirectToAction("List");

                return RedirectToAction("EditSpecificationAttributeGroup", new { id = specificationAttributeGroup.Id });
            }

            model = await SpecificationAttributeModelFactory.PrepareSpecificationAttributeGroupModelAsync(model, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual async Task<IActionResult> CreateSpecificationAttribute()
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            var model = await SpecificationAttributeModelFactory.PrepareSpecificationAttributeModelAsync(new SpecificationAttributeModel(), null);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> CreateSpecificationAttribute(SpecificationAttributeModel model, bool continueEditing)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var specificationAttribute = model.ToEntity<SpecificationAttribute>();
                await SpecificationAttributeService.InsertSpecificationAttributeAsync(specificationAttribute);
                await UpdateAttributeLocalesAsync(specificationAttribute, model);

                await CustomerActivityService.InsertActivityAsync("AddNewSpecAttribute",
                    string.Format(await LocalizationService.GetResourceAsync("ActivityLog.AddNewSpecAttribute"), specificationAttribute.Name), specificationAttribute);

                NotificationService.SuccessNotification(await LocalizationService.GetResourceAsync("Admin.Catalog.Attributes.SpecificationAttributes.SpecificationAttribute.Added"));

                if (!continueEditing)
                    return RedirectToAction("List");
                
                return RedirectToAction("EditSpecificationAttribute", new { id = specificationAttribute.Id });
            }

            model = await SpecificationAttributeModelFactory.PrepareSpecificationAttributeModelAsync(model, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual async Task<IActionResult> EditSpecificationAttributeGroup(int id)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            var specificationAttributeGroup = await SpecificationAttributeService.GetSpecificationAttributeGroupByIdAsync(id);
            if (specificationAttributeGroup == null)
                return RedirectToAction("List");

            var model = await SpecificationAttributeModelFactory.PrepareSpecificationAttributeGroupModelAsync(null, specificationAttributeGroup);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> EditSpecificationAttributeGroup(SpecificationAttributeGroupModel model, bool continueEditing)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            var specificationAttributeGroup = await SpecificationAttributeService.GetSpecificationAttributeGroupByIdAsync(model.Id);
            if (specificationAttributeGroup == null)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                specificationAttributeGroup = model.ToEntity(specificationAttributeGroup);
                await SpecificationAttributeService.UpdateSpecificationAttributeGroupAsync(specificationAttributeGroup);
                await UpdateAttributeGroupLocalesAsync(specificationAttributeGroup, model);

                await CustomerActivityService.InsertActivityAsync("EditSpecAttributeGroup",
                    string.Format(await LocalizationService.GetResourceAsync("ActivityLog.EditSpecAttributeGroup"), specificationAttributeGroup.Name), specificationAttributeGroup);

                NotificationService.SuccessNotification(await LocalizationService.GetResourceAsync("Admin.Catalog.Attributes.SpecificationAttributes.SpecificationAttributeGroup.Updated"));

                if (!continueEditing)
                    return RedirectToAction("List");

                return RedirectToAction("EditSpecificationAttributeGroup", new { id = specificationAttributeGroup.Id });
            }

            model = await SpecificationAttributeModelFactory.PrepareSpecificationAttributeGroupModelAsync(model, specificationAttributeGroup, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual async Task<IActionResult> EditSpecificationAttribute(int id)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            //try to get a specification attribute with the specified id
            var specificationAttribute = await SpecificationAttributeService.GetSpecificationAttributeByIdAsync(id);
            if (specificationAttribute == null)
                return RedirectToAction("List");

            //prepare model
            var model = await SpecificationAttributeModelFactory.PrepareSpecificationAttributeModelAsync(null, specificationAttribute);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> EditSpecificationAttribute(SpecificationAttributeModel model, bool continueEditing)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            //try to get a specification attribute with the specified id
            var specificationAttribute = await SpecificationAttributeService.GetSpecificationAttributeByIdAsync(model.Id);
            if (specificationAttribute == null)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                specificationAttribute = model.ToEntity(specificationAttribute);
                await SpecificationAttributeService.UpdateSpecificationAttributeAsync(specificationAttribute);

                await UpdateAttributeLocalesAsync(specificationAttribute, model);

                //activity log
                await CustomerActivityService.InsertActivityAsync("EditSpecAttribute",
                    string.Format(await LocalizationService.GetResourceAsync("ActivityLog.EditSpecAttribute"), specificationAttribute.Name), specificationAttribute);

                NotificationService.SuccessNotification(await LocalizationService.GetResourceAsync("Admin.Catalog.Attributes.SpecificationAttributes.SpecificationAttribute.Updated"));

                if (!continueEditing)
                    return RedirectToAction("List");
                
                return RedirectToAction("EditSpecificationAttribute", new { id = specificationAttribute.Id });
            }

            //prepare model
            model = await SpecificationAttributeModelFactory.PrepareSpecificationAttributeModelAsync(model, specificationAttribute, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> DeleteSpecificationAttributeGroup(int id)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            var specificationAttributeGroup = await SpecificationAttributeService.GetSpecificationAttributeGroupByIdAsync(id);
            if (specificationAttributeGroup == null)
                return RedirectToAction("List");

            await SpecificationAttributeService.DeleteSpecificationAttributeGroupAsync(specificationAttributeGroup);

            await CustomerActivityService.InsertActivityAsync("DeleteSpecAttributeGroup",
                string.Format(await LocalizationService.GetResourceAsync("ActivityLog.DeleteSpecAttributeGroup"), specificationAttributeGroup.Name), specificationAttributeGroup);

            NotificationService.SuccessNotification(await LocalizationService.GetResourceAsync("Admin.Catalog.Attributes.SpecificationAttributes.SpecificationAttributeGroup.Deleted"));

            return RedirectToAction("List");
        }

        [HttpPost]
        public virtual async Task<IActionResult> DeleteSpecificationAttribute(int id)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            var specificationAttribute = await SpecificationAttributeService.GetSpecificationAttributeByIdAsync(id);

            if (specificationAttribute == null)
                return RedirectToAction("List");

            await SpecificationAttributeService.DeleteSpecificationAttributeAsync(specificationAttribute);

            await CustomerActivityService.InsertActivityAsync("DeleteSpecAttribute",
                string.Format(await LocalizationService.GetResourceAsync("ActivityLog.DeleteSpecAttribute"), specificationAttribute.Name), specificationAttribute);

            NotificationService.SuccessNotification(await LocalizationService.GetResourceAsync("Admin.Catalog.Attributes.SpecificationAttributes.SpecificationAttribute.Deleted"));

            return RedirectToAction("List");
        }

        [HttpPost]
        public virtual async Task<IActionResult> DeleteSelectedSpecificationAttributes(ICollection<int> selectedIds)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            if (selectedIds == null || selectedIds.Count() == 0)
                return NoContent();

            var specificationAttributes = await SpecificationAttributeService.GetSpecificationAttributeByIdsAsync(selectedIds.ToArray());
            await SpecificationAttributeService.DeleteSpecificationAttributesAsync(specificationAttributes);

            foreach (var specificationAttribute in specificationAttributes)
            {
                await CustomerActivityService.InsertActivityAsync("DeleteSpecAttribute",
                    string.Format(await LocalizationService.GetResourceAsync("ActivityLog.DeleteSpecAttribute"), specificationAttribute.Name), specificationAttribute);
            }

            return Json(new { Result = true });
        }

        #endregion

        #region Specification attribute options

        [HttpPost]
        public virtual async Task<IActionResult> OptionList(SpecificationAttributeOptionSearchModel searchModel)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageAttributes))
                return await AccessDeniedDataTablesJson();

            //try to get a specification attribute with the specified id
            var specificationAttribute = await SpecificationAttributeService.GetSpecificationAttributeByIdAsync(searchModel.SpecificationAttributeId)
                ?? throw new ArgumentException("No specification attribute found with the specified id");

            //prepare model
            var model = await SpecificationAttributeModelFactory.PrepareSpecificationAttributeOptionListModelAsync(searchModel, specificationAttribute);

            return Json(model);
        }

        public virtual async Task<IActionResult> OptionCreatePopup(int specificationAttributeId)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            //try to get a specification attribute with the specified id
            var specificationAttribute = await SpecificationAttributeService.GetSpecificationAttributeByIdAsync(specificationAttributeId);
            if (specificationAttribute == null)
                return RedirectToAction("List");

            //prepare model
            var model = await SpecificationAttributeModelFactory
                .PrepareSpecificationAttributeOptionModelAsync(new SpecificationAttributeOptionModel(), specificationAttribute, null);

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> OptionCreatePopup(SpecificationAttributeOptionModel model)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            //try to get a specification attribute with the specified id
            var specificationAttribute = await SpecificationAttributeService.GetSpecificationAttributeByIdAsync(model.SpecificationAttributeId);
            if (specificationAttribute == null)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                var sao = model.ToEntity<SpecificationAttributeOption>();

                //clear "Color" values if it's disabled
                if (!model.EnableColorSquaresRgb)
                    sao.ColorSquaresRgb = null;

                await SpecificationAttributeService.InsertSpecificationAttributeOptionAsync(sao);

                await UpdateOptionLocalesAsync(sao, model);

                ViewBag.RefreshPage = true;

                return View(model);
            }

            //prepare model
            model = await SpecificationAttributeModelFactory.PrepareSpecificationAttributeOptionModelAsync(model, specificationAttribute, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual async Task<IActionResult> OptionEditPopup(int id)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            //try to get a specification attribute option with the specified id
            var specificationAttributeOption = await SpecificationAttributeService.GetSpecificationAttributeOptionByIdAsync(id);
            if (specificationAttributeOption == null)
                return RedirectToAction("List");

            //try to get a specification attribute with the specified id
            var specificationAttribute = await SpecificationAttributeService
                .GetSpecificationAttributeByIdAsync(specificationAttributeOption.SpecificationAttributeId);
            if (specificationAttribute == null)
                return RedirectToAction("List");

            //prepare model
            var model = await SpecificationAttributeModelFactory
                .PrepareSpecificationAttributeOptionModelAsync(null, specificationAttribute, specificationAttributeOption);

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> OptionEditPopup(SpecificationAttributeOptionModel model)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            //try to get a specification attribute option with the specified id
            var specificationAttributeOption = await SpecificationAttributeService.GetSpecificationAttributeOptionByIdAsync(model.Id);
            if (specificationAttributeOption == null)
                return RedirectToAction("List");

            //try to get a specification attribute with the specified id
            var specificationAttribute = await SpecificationAttributeService
                .GetSpecificationAttributeByIdAsync(specificationAttributeOption.SpecificationAttributeId);
            if (specificationAttribute == null)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                specificationAttributeOption = model.ToEntity(specificationAttributeOption);

                //clear "Color" values if it's disabled
                if (!model.EnableColorSquaresRgb)
                    specificationAttributeOption.ColorSquaresRgb = null;

                await SpecificationAttributeService.UpdateSpecificationAttributeOptionAsync(specificationAttributeOption);

                await UpdateOptionLocalesAsync(specificationAttributeOption, model);

                ViewBag.RefreshPage = true;

                return View(model);
            }

            //prepare model
            model = await SpecificationAttributeModelFactory
                .PrepareSpecificationAttributeOptionModelAsync(model, specificationAttribute, specificationAttributeOption, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> OptionDelete(int id, int specificationAttributeId)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            //try to get a specification attribute option with the specified id
            var specificationAttributeOption = await SpecificationAttributeService.GetSpecificationAttributeOptionByIdAsync(id)
                ?? throw new ArgumentException("No specification attribute option found with the specified id", nameof(id));

            await SpecificationAttributeService.DeleteSpecificationAttributeOptionAsync(specificationAttributeOption);

            return new NullJsonResult();
        }

        [HttpGet]
        public virtual async Task<IActionResult> GetOptionsByAttributeId(string attributeId)
        {
            //do not make any permission validation here 
            //because this method could be used on some other pages (such as product editing)
            //if (!await PermissionService.Authorize(StandardPermissionProvider.ManageAttributes))
            //    return AccessDeniedView();

            //this action method gets called via an ajax request
            if (string.IsNullOrEmpty(attributeId))
                throw new ArgumentNullException(nameof(attributeId));

            var options = await SpecificationAttributeService.GetSpecificationAttributeOptionsBySpecificationAttributeAsync(Convert.ToInt32(attributeId));
            var result = (from o in options
                          select new { id = o.Id, name = o.Name }).ToList();
            return Json(result);
        }

        #endregion

        #region Mapped products

        [HttpPost]
        public virtual async Task<IActionResult> UsedByProducts(SpecificationAttributeProductSearchModel searchModel)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageAttributes))
                return await AccessDeniedDataTablesJson();

            //try to get a specification attribute with the specified id
            var specificationAttribute = await SpecificationAttributeService.GetSpecificationAttributeByIdAsync(searchModel.SpecificationAttributeId)
                ?? throw new ArgumentException("No specification attribute found with the specified id");

            //prepare model
            var model = await SpecificationAttributeModelFactory.PrepareSpecificationAttributeProductListModelAsync(searchModel, specificationAttribute);

            return Json(model);
        }

        #endregion
    }
}