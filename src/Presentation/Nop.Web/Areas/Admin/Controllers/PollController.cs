using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Polls;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Polls;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Polls;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Validators;

namespace Nop.Web.Areas.Admin.Controllers;

public partial class PollController : BaseAdminController
{
    #region Fields

    protected readonly ILocalizationService _localizationService;
    protected readonly INotificationService _notificationService;
    protected readonly IPermissionService _permissionService;
    protected readonly IPollModelFactory _pollModelFactory;
    protected readonly IPollService _pollService;
    protected readonly IStoreMappingService _storeMappingService;
    protected readonly IStoreService _storeService;

    #endregion

    #region Ctor

    public PollController(ILocalizationService localizationService,
        INotificationService notificationService,
        IPermissionService permissionService,
        IPollModelFactory pollModelFactory,
        IPollService pollService,
        IStoreMappingService storeMappingService,
        IStoreService storeService)
    {
        _localizationService = localizationService;
        _notificationService = notificationService;
        _permissionService = permissionService;
        _pollModelFactory = pollModelFactory;
        _pollService = pollService;
        _storeMappingService = storeMappingService;
        _storeService = storeService;
    }

    #endregion

    #region Utilities

    protected virtual async Task SaveStoreMappingsAsync(Poll poll, PollModel model)
    {
        poll.LimitedToStores = model.SelectedStoreIds.Any();
        await _pollService.UpdatePollAsync(poll);

        //manage store mappings
        var existingStoreMappings = await _storeMappingService.GetStoreMappingsAsync(poll);
        foreach (var store in await _storeService.GetAllStoresAsync())
        {
            var existingStoreMapping = existingStoreMappings.FirstOrDefault(storeMapping => storeMapping.StoreId == store.Id);

            //new store mapping
            if (model.SelectedStoreIds.Contains(store.Id))
            {
                if (existingStoreMapping == null)
                    await _storeMappingService.InsertStoreMappingAsync(poll, store.Id);
            }
            //or remove existing one
            else if (existingStoreMapping != null)
                await _storeMappingService.DeleteStoreMappingAsync(existingStoreMapping);
        }
    }

    #endregion

    #region Polls

    public virtual IActionResult Index()
    {
        return RedirectToAction("List");
    }

    [CheckPermission(StandardPermission.ContentManagement.POLLS_VIEW)]
    public virtual async Task<IActionResult> List()
    {
        //prepare model
        var model = await _pollModelFactory.PreparePollSearchModelAsync(new PollSearchModel());

        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.ContentManagement.POLLS_VIEW)]
    public virtual async Task<IActionResult> List(PollSearchModel searchModel)
    {
        //prepare model
        var model = await _pollModelFactory.PreparePollListModelAsync(searchModel);

        return Json(model);
    }

    [CheckPermission(StandardPermission.ContentManagement.POLLS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> Create()
    {
        //prepare model
        var model = await _pollModelFactory.PreparePollModelAsync(new PollModel(), null);

        return View(model);
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    [CheckPermission(StandardPermission.ContentManagement.POLLS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> Create(PollModel model, bool continueEditing)
    {
        if (ModelState.IsValid)
        {
            var poll = model.ToEntity<Poll>();
            await _pollService.InsertPollAsync(poll);

            //save store mappings
            await SaveStoreMappingsAsync(poll, model);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.ContentManagement.Polls.Added"));

            if (!continueEditing)
                return RedirectToAction("List");

            return RedirectToAction("Edit", new { id = poll.Id });
        }

        //prepare model
        model = await _pollModelFactory.PreparePollModelAsync(model, null, true);

        //if we got this far, something failed, redisplay form
        return View(model);
    }

    [CheckPermission(StandardPermission.ContentManagement.POLLS_VIEW)]
    public virtual async Task<IActionResult> Edit(int id)
    {
        //try to get a poll with the specified id
        var poll = await _pollService.GetPollByIdAsync(id);
        if (poll == null)
            return RedirectToAction("List");

        //prepare model
        var model = await _pollModelFactory.PreparePollModelAsync(null, poll);

        return View(model);
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    [CheckPermission(StandardPermission.ContentManagement.POLLS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> Edit(PollModel model, bool continueEditing)
    {
        //try to get a poll with the specified id
        var poll = await _pollService.GetPollByIdAsync(model.Id);
        if (poll == null)
            return RedirectToAction("List");

        if (ModelState.IsValid)
        {
            poll = model.ToEntity(poll);
            await _pollService.UpdatePollAsync(poll);

            //save store mappings
            await SaveStoreMappingsAsync(poll, model);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.ContentManagement.Polls.Updated"));

            if (!continueEditing)
                return RedirectToAction("List");

            return RedirectToAction("Edit", new { id = poll.Id });
        }

        //prepare model
        model = await _pollModelFactory.PreparePollModelAsync(model, poll, true);

        //if we got this far, something failed, redisplay form
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.ContentManagement.POLLS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> Delete(int id)
    {
        //try to get a poll with the specified id
        var poll = await _pollService.GetPollByIdAsync(id);
        if (poll == null)
            return RedirectToAction("List");

        await _pollService.DeletePollAsync(poll);

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.ContentManagement.Polls.Deleted"));

        return RedirectToAction("List");
    }

    #endregion

    #region Poll answer

    [HttpPost]
    [CheckPermission(StandardPermission.ContentManagement.POLLS_VIEW)]
    public virtual async Task<IActionResult> PollAnswers(PollAnswerSearchModel searchModel)
    {
        //try to get a poll with the specified id
        var poll = await _pollService.GetPollByIdAsync(searchModel.PollId)
            ?? throw new ArgumentException("No poll found with the specified id");

        //prepare model
        var model = await _pollModelFactory.PreparePollAnswerListModelAsync(searchModel, poll);

        return Json(model);
    }

    //ValidateAttribute is used to force model validation
    [HttpPost]
    [CheckPermission(StandardPermission.ContentManagement.POLLS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> PollAnswerUpdate([Validate] PollAnswerModel model)
    {
        if (!ModelState.IsValid)
            return ErrorJson(ModelState.SerializeErrors());

        //try to get a poll answer with the specified id
        var pollAnswer = await _pollService.GetPollAnswerByIdAsync(model.Id)
            ?? throw new ArgumentException("No poll answer found with the specified id");

        pollAnswer = model.ToEntity(pollAnswer);

        await _pollService.UpdatePollAnswerAsync(pollAnswer);

        return new NullJsonResult();
    }

    //ValidateAttribute is used to force model validation
    [HttpPost]
    [CheckPermission(StandardPermission.ContentManagement.POLLS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> PollAnswerAdd(int pollId, [Validate] PollAnswerModel model)
    {
        if (!ModelState.IsValid)
            return ErrorJson(ModelState.SerializeErrors());

        //fill entity from model
        await _pollService.InsertPollAnswerAsync(model.ToEntity<PollAnswer>());

        return Json(new { Result = true });
    }

    [HttpPost]
    [CheckPermission(StandardPermission.ContentManagement.POLLS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> PollAnswerDelete(int id)
    {
        //try to get a poll answer with the specified id
        var pollAnswer = await _pollService.GetPollAnswerByIdAsync(id)
            ?? throw new ArgumentException("No poll answer found with the specified id", nameof(id));

        await _pollService.DeletePollAnswerAsync(pollAnswer);

        return new NullJsonResult();
    }

    #endregion
}