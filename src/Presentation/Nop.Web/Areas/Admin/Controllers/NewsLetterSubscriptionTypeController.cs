using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Messages;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Messages;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Web.Areas.Admin.Controllers;

public partial class NewsLetterSubscriptionTypeController : BaseAdminController
{
    #region Fields

    protected readonly ILocalizationService _localizationService;
    protected readonly ILocalizedEntityService _localizedEntityService;
    protected readonly INewsletterSubscriptionTypeModelFactory _newsletterSubscriptionTypeModelFactory;
    protected readonly INewsLetterSubscriptionTypeService _newsLetterSubscriptionTypeService;
    protected readonly INotificationService _notificationService;

    #endregion

    #region Ctor

    public NewsLetterSubscriptionTypeController(ILocalizationService localizationService,
        ILocalizedEntityService localizedEntityService,
        INewsletterSubscriptionTypeModelFactory newsletterSubscriptionTypeModelFactory,
        INewsLetterSubscriptionTypeService newsLetterSubscriptionTypeService,
        INotificationService notificationService)
    {
        _localizationService = localizationService;
        _localizedEntityService = localizedEntityService;
        _newsletterSubscriptionTypeModelFactory = newsletterSubscriptionTypeModelFactory;
        _newsLetterSubscriptionTypeService = newsLetterSubscriptionTypeService;
        _notificationService = notificationService;
    }

    #endregion

    #region Utilities

    protected virtual async Task UpdateSubscriptionTypeLocalesAsync(NewsLetterSubscriptionType subscriptionType, NewsLetterSubscriptionTypeModel model)
    {
        foreach (var localized in model.Locales)
        {
            await _localizedEntityService.SaveLocalizedValueAsync(subscriptionType,
                x => x.Name,
                localized.Name,
                localized.LanguageId);
        }
    }

    #endregion

    #region Methods

    public virtual IActionResult Index()
    {
        return RedirectToAction("List");
    }

    [CheckPermission(StandardPermission.Promotions.SUBSCRIBERS_VIEW)]
    public virtual async Task<IActionResult> List()
    {
        //prepare model
        var model = await _newsletterSubscriptionTypeModelFactory.PrepareNewsletterSubscriptionTypeSearchModelAsync(new NewsLetterSubscriptionTypeSearchModel());

        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Promotions.SUBSCRIBERS_VIEW)]
    public virtual async Task<IActionResult> List(NewsLetterSubscriptionTypeSearchModel searchModel)
    {
        //prepare model
        var model = await _newsletterSubscriptionTypeModelFactory.PrepareNewsletterSubscriptionTypeListModelAsync(searchModel);

        return Json(model);
    }

    [CheckPermission(StandardPermission.Promotions.SUBSCRIBERS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> Create()
    {
        //prepare model
        var model = await _newsletterSubscriptionTypeModelFactory.PrepareNewsletterSubscriptionTypeModelAsync(new NewsLetterSubscriptionTypeModel(), null);

        return View(model);
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    [CheckPermission(StandardPermission.Promotions.SUBSCRIBERS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> Create(NewsLetterSubscriptionTypeModel model, bool continueEditing)
    {
        if (ModelState.IsValid)
        {
            var subscriptionType = model.ToEntity<NewsLetterSubscriptionType>();
            await _newsLetterSubscriptionTypeService.InsertNewsLetterSubscriptionTypeAsync(subscriptionType);

            //locales                
            await UpdateSubscriptionTypeLocalesAsync(subscriptionType, model);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Promotions.NewsLetterSubscriptionType.Added"));

            return continueEditing ? RedirectToAction("Edit", new { id = subscriptionType.Id }) : RedirectToAction("List");
        }

        //prepare model
        model = await _newsletterSubscriptionTypeModelFactory.PrepareNewsletterSubscriptionTypeModelAsync(model, null, true);

        //if we got this far, something failed, redisplay form
        return View(model);
    }

    [CheckPermission(StandardPermission.Promotions.SUBSCRIBERS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> Edit(int id)
    {
        //try to get an product subscription type with the specified id
        var subscriptionType = await _newsLetterSubscriptionTypeService.GetNewsLetterSubscriptionTypeByIdAsync(id);
        if (subscriptionType == null)
            return RedirectToAction("List");

        //prepare model
        var model = await _newsletterSubscriptionTypeModelFactory.PrepareNewsletterSubscriptionTypeModelAsync(null, subscriptionType);

        return View(model);
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    [CheckPermission(StandardPermission.Promotions.SUBSCRIBERS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> Edit(NewsLetterSubscriptionTypeModel model, bool continueEditing)
    {
        //try to get an product subscription type with the specified id
        var subscriptionType = await _newsLetterSubscriptionTypeService.GetNewsLetterSubscriptionTypeByIdAsync(model.Id);
        if (subscriptionType == null)
            return RedirectToAction("List");

        if (ModelState.IsValid)
        {
            //fill entity from model
            subscriptionType = model.ToEntity(subscriptionType);
            await _newsLetterSubscriptionTypeService.UpdateNewsLetterSubscriptionTypeAsync(subscriptionType);

            //locales                
            await UpdateSubscriptionTypeLocalesAsync(subscriptionType, model);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Promotions.NewsLetterSubscriptionType.Updated"));

            return continueEditing ? RedirectToAction("Edit", new { id = subscriptionType.Id }) : RedirectToAction("List");
        }

        //prepare model
        model = await _newsletterSubscriptionTypeModelFactory.PrepareNewsletterSubscriptionTypeModelAsync(model, subscriptionType, true);

        //if we got this far, something failed, redisplay form
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Promotions.SUBSCRIBERS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> Delete(int id)
    {
        //try to get an product subscription type with the specified id
        var subscriptionType = await _newsLetterSubscriptionTypeService.GetNewsLetterSubscriptionTypeByIdAsync(id);
        if (subscriptionType == null)
            return RedirectToAction("List");

        try
        {
            await _newsLetterSubscriptionTypeService.DeleteNewsLetterSubscriptionTypeAsync(subscriptionType);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Promotions.NewsLetterSubscriptionType.Deleted"));

            return RedirectToAction("List");
        }
        catch (Exception exc) 
        {
            await _notificationService.ErrorNotificationAsync(exc);
            return RedirectToAction("Edit", new { id = subscriptionType.Id });
        }
    }

    #endregion
}
