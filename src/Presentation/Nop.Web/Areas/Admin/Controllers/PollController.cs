using System;
using System.Linq;
using System.Threading.Tasks;
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

namespace Nop.Web.Areas.Admin.Controllers
{
    public partial class PollController : BaseAdminController
    {
        #region Fields

        protected ILocalizationService LocalizationService { get; }
        protected INotificationService NotificationService { get; }
        protected IPermissionService PermissionService { get; }
        protected IPollModelFactory PollModelFactory { get; }
        protected IPollService PollService { get; }
        protected IStoreMappingService StoreMappingService { get; }
        protected IStoreService StoreService { get; }

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
            LocalizationService = localizationService;
            NotificationService = notificationService;
            PermissionService = permissionService;
            PollModelFactory = pollModelFactory;
            PollService = pollService;
            StoreMappingService = storeMappingService;
            StoreService = storeService;
        }

        #endregion

        #region Utilities

        protected virtual async Task SaveStoreMappingsAsync(Poll poll, PollModel model)
        {
            poll.LimitedToStores = model.SelectedStoreIds.Any();
            await PollService.UpdatePollAsync(poll);

            //manage store mappings
            var existingStoreMappings = await StoreMappingService.GetStoreMappingsAsync(poll);
            foreach (var store in await StoreService.GetAllStoresAsync())
            {
                var existingStoreMapping = existingStoreMappings.FirstOrDefault(storeMapping => storeMapping.StoreId == store.Id);

                //new store mapping
                if (model.SelectedStoreIds.Contains(store.Id))
                {
                    if (existingStoreMapping == null)
                        await StoreMappingService.InsertStoreMappingAsync(poll, store.Id);
                }
                //or remove existing one
                else if (existingStoreMapping != null)
                    await StoreMappingService.DeleteStoreMappingAsync(existingStoreMapping);
            }
        }

        #endregion

        #region Polls

        public virtual IActionResult Index()
        {
            return RedirectToAction("List");
        }

        public virtual async Task<IActionResult> List()
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManagePolls))
                return AccessDeniedView();

            //prepare model
            var model = await PollModelFactory.PreparePollSearchModelAsync(new PollSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> List(PollSearchModel searchModel)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManagePolls))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await PollModelFactory.PreparePollListModelAsync(searchModel);

            return Json(model);
        }

        public virtual async Task<IActionResult> Create()
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManagePolls))
                return AccessDeniedView();

            //prepare model
            var model = await PollModelFactory.PreparePollModelAsync(new PollModel(), null);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> Create(PollModel model, bool continueEditing)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManagePolls))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var poll = model.ToEntity<Poll>();
                await PollService.InsertPollAsync(poll);

                //save store mappings
                await SaveStoreMappingsAsync(poll, model);

                NotificationService.SuccessNotification(await LocalizationService.GetResourceAsync("Admin.ContentManagement.Polls.Added"));

                if (!continueEditing)
                    return RedirectToAction("List");

                return RedirectToAction("Edit", new { id = poll.Id });
            }

            //prepare model
            model = await PollModelFactory.PreparePollModelAsync(model, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual async Task<IActionResult> Edit(int id)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManagePolls))
                return AccessDeniedView();

            //try to get a poll with the specified id
            var poll = await PollService.GetPollByIdAsync(id);
            if (poll == null)
                return RedirectToAction("List");

            //prepare model
            var model = await PollModelFactory.PreparePollModelAsync(null, poll);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> Edit(PollModel model, bool continueEditing)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManagePolls))
                return AccessDeniedView();

            //try to get a poll with the specified id
            var poll = await PollService.GetPollByIdAsync(model.Id);
            if (poll == null)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                poll = model.ToEntity(poll);
                await PollService.UpdatePollAsync(poll);

                //save store mappings
                await SaveStoreMappingsAsync(poll, model);

                NotificationService.SuccessNotification(await LocalizationService.GetResourceAsync("Admin.ContentManagement.Polls.Updated"));

                if (!continueEditing)
                    return RedirectToAction("List");

                return RedirectToAction("Edit", new { id = poll.Id });
            }

            //prepare model
            model = await PollModelFactory.PreparePollModelAsync(model, poll, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManagePolls))
                return AccessDeniedView();

            //try to get a poll with the specified id
            var poll = await PollService.GetPollByIdAsync(id);
            if (poll == null)
                return RedirectToAction("List");

            await PollService.DeletePollAsync(poll);

            NotificationService.SuccessNotification(await LocalizationService.GetResourceAsync("Admin.ContentManagement.Polls.Deleted"));

            return RedirectToAction("List");
        }

        #endregion

        #region Poll answer

        [HttpPost]
        public virtual async Task<IActionResult> PollAnswers(PollAnswerSearchModel searchModel)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManagePolls))
                return await AccessDeniedDataTablesJson();

            //try to get a poll with the specified id
            var poll = await PollService.GetPollByIdAsync(searchModel.PollId)
                ?? throw new ArgumentException("No poll found with the specified id");

            //prepare model
            var model = await PollModelFactory.PreparePollAnswerListModelAsync(searchModel, poll);

            return Json(model);
        }

        //ValidateAttribute is used to force model validation
        [HttpPost]
        public virtual async Task<IActionResult> PollAnswerUpdate([Validate] PollAnswerModel model)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManagePolls))
                return AccessDeniedView();

            if (!ModelState.IsValid)
                return ErrorJson(ModelState.SerializeErrors());

            //try to get a poll answer with the specified id
            var pollAnswer = await PollService.GetPollAnswerByIdAsync(model.Id)
                ?? throw new ArgumentException("No poll answer found with the specified id");

            pollAnswer = model.ToEntity(pollAnswer);

            await PollService.UpdatePollAnswerAsync(pollAnswer);

            return new NullJsonResult();
        }

        //ValidateAttribute is used to force model validation
        [HttpPost]
        public virtual async Task<IActionResult> PollAnswerAdd(int pollId, [Validate] PollAnswerModel model)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManagePolls))
                return AccessDeniedView();

            if (!ModelState.IsValid)
                return ErrorJson(ModelState.SerializeErrors());

            //fill entity from model
            await PollService.InsertPollAnswerAsync(model.ToEntity<PollAnswer>());

            return Json(new { Result = true });
        }

        [HttpPost]
        public virtual async Task<IActionResult> PollAnswerDelete(int id)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManagePolls))
                return AccessDeniedView();

            //try to get a poll answer with the specified id
            var pollAnswer = await PollService.GetPollAnswerByIdAsync(id)
                ?? throw new ArgumentException("No poll answer found with the specified id", nameof(id));

            await PollService.DeletePollAnswerAsync(pollAnswer);

            return new NullJsonResult();
        }

        #endregion
    }
}