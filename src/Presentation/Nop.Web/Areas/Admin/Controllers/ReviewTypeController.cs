using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Catalog;
using Nop.Services.Catalog;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Web.Areas.Admin.Controllers
{
    /// <summary>
    /// Represent a review type controller
    /// </summary>
    public partial class ReviewTypeController : BaseAdminController
    {
        #region Fields

        protected ICustomerActivityService CustomerActivityService { get; }
        protected ILocalizationService LocalizationService { get; }
        protected ILocalizedEntityService LocalizedEntityService { get; }
        protected INotificationService NotificationService { get; }
        protected IPermissionService PermissionService { get; }
        protected IReviewTypeModelFactory ReviewTypeModelFactory { get; }
        protected IReviewTypeService ReviewTypeService { get; }

        #endregion

        #region Ctor

        public ReviewTypeController(ICustomerActivityService customerActivityService,
            ILocalizationService localizationService,
            ILocalizedEntityService localizedEntityService,
            INotificationService notificationService,
            IPermissionService permissionService,
            IReviewTypeModelFactory reviewTypeModelFactory,
            IReviewTypeService reviewTypeService)
        {
            ReviewTypeModelFactory = reviewTypeModelFactory;
            ReviewTypeService = reviewTypeService;
            CustomerActivityService = customerActivityService;
            LocalizedEntityService = localizedEntityService;
            LocalizationService = localizationService;
            NotificationService = notificationService;
            PermissionService = permissionService;
        }

        #endregion

        #region Utilities

        protected virtual async Task UpdateReviewTypeLocalesAsync(ReviewType reviewType, ReviewTypeModel model)
        {
            foreach (var localized in model.Locales)
            {
                await LocalizedEntityService.SaveLocalizedValueAsync(reviewType,
                    x => x.Name,                    
                    localized.Name,
                    localized.LanguageId);

                await LocalizedEntityService.SaveLocalizedValueAsync(reviewType,
                   x => x.Description,
                   localized.Description,
                   localized.LanguageId);
            }
        }

        #endregion

        #region Review type

        public virtual IActionResult Index()
        {
            return RedirectToAction("List");
        }

        public virtual async Task<IActionResult> List()
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //select an appropriate card
            SaveSelectedCardName("catalogsettings-review-types");

            //we just redirect a user to the catalog settings page
            return RedirectToAction("Catalog", "Setting");
        }

        [HttpPost]
        public virtual async Task<IActionResult> List(ReviewTypeSearchModel searchModel)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await ReviewTypeModelFactory.PrepareReviewTypeListModelAsync(searchModel);

            return Json(model);
        }

        public virtual async Task<IActionResult> Create()
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //prepare model
            var model = await ReviewTypeModelFactory.PrepareReviewTypeModelAsync(new ReviewTypeModel(), null);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> Create(ReviewTypeModel model, bool continueEditing)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var reviewType = model.ToEntity<ReviewType>();
                await ReviewTypeService.InsertReviewTypeAsync(reviewType);                

                //activity log
                await CustomerActivityService.InsertActivityAsync("AddNewReviewType",
                    string.Format(await LocalizationService.GetResourceAsync("ActivityLog.AddNewReviewType"), reviewType.Id), reviewType);

                //locales                
                await UpdateReviewTypeLocalesAsync(reviewType, model);

                NotificationService.SuccessNotification(await LocalizationService.GetResourceAsync("Admin.Settings.ReviewType.Added"));

                return continueEditing ? RedirectToAction("Edit", new { id = reviewType.Id }) : RedirectToAction("List");                
            }

            //prepare model
            model = await ReviewTypeModelFactory.PrepareReviewTypeModelAsync(model, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual async Task<IActionResult> Edit(int id)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //try to get an product review type with the specified id
            var reviewType = await ReviewTypeService.GetReviewTypeByIdAsync(id);
            if (reviewType == null)
                return RedirectToAction("List");

            //prepare model
            var model = await ReviewTypeModelFactory.PrepareReviewTypeModelAsync(null, reviewType);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> Edit(ReviewTypeModel model, bool continueEditing)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //try to get an review type with the specified id
            var reviewType = await ReviewTypeService.GetReviewTypeByIdAsync(model.Id);
            if (reviewType == null)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                reviewType = model.ToEntity(reviewType);
                await ReviewTypeService.UpdateReviewTypeAsync(reviewType);

                //activity log
                await CustomerActivityService.InsertActivityAsync("EditReviewType",
                    string.Format(await LocalizationService.GetResourceAsync("ActivityLog.EditReviewType"), reviewType.Id),
                    reviewType);

                //locales
                await UpdateReviewTypeLocalesAsync(reviewType, model);

                NotificationService.SuccessNotification(await LocalizationService.GetResourceAsync("Admin.Settings.ReviewType.Updated"));

                return continueEditing ? RedirectToAction("Edit", new { id = reviewType.Id }) : RedirectToAction("List");                
            }

            //prepare model
            model = await ReviewTypeModelFactory.PrepareReviewTypeModelAsync(model, reviewType, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //try to get an review type with the specified id
            var reviewType = await ReviewTypeService.GetReviewTypeByIdAsync(id);
            if (reviewType == null)
                return RedirectToAction("List");

            try
            {
                await ReviewTypeService.DeleteReviewTypeAsync(reviewType);

                //activity log
                await CustomerActivityService.InsertActivityAsync("DeleteReviewType",
                    string.Format(await LocalizationService.GetResourceAsync("ActivityLog.DeleteReviewType"), reviewType),
                    reviewType);

                NotificationService.SuccessNotification(await LocalizationService.GetResourceAsync("Admin.Settings.ReviewType.Deleted"));

                return RedirectToAction("List");
            }
            catch (Exception exc)
            {
                await NotificationService.ErrorNotificationAsync(exc);
                return RedirectToAction("Edit", new { id = reviewType.Id });
            }            
        }

        #endregion
    }
}
