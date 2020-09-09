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

        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;
        private readonly IPermissionService _permissionService;
        private readonly IPollModelFactory _pollModelFactory;
        private readonly IPollService _pollService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IStoreService _storeService;

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

        protected virtual async Task SaveStoreMappings(Poll poll, PollModel model)
        {
            poll.LimitedToStores = model.SelectedStoreIds.Any();
            await _pollService.UpdatePoll(poll);

            //manage store mappings
            var existingStoreMappings = await _storeMappingService.GetStoreMappings(poll);
            foreach (var store in await _storeService.GetAllStores())
            {
                var existingStoreMapping = existingStoreMappings.FirstOrDefault(storeMapping => storeMapping.StoreId == store.Id);

                //new store mapping
                if (model.SelectedStoreIds.Contains(store.Id))
                {
                    if (existingStoreMapping == null)
                        await _storeMappingService.InsertStoreMapping(poll, store.Id);
                }
                //or remove existing one
                else if (existingStoreMapping != null)
                    await _storeMappingService.DeleteStoreMapping(existingStoreMapping);
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
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManagePolls))
                return AccessDeniedView();

            //prepare model
            var model = await _pollModelFactory.PreparePollSearchModel(new PollSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> List(PollSearchModel searchModel)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManagePolls))
                return AccessDeniedDataTablesJson();

            //prepare model
            var model = await _pollModelFactory.PreparePollListModel(searchModel);

            return Json(model);
        }

        public virtual async Task<IActionResult> Create()
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManagePolls))
                return AccessDeniedView();

            //prepare model
            var model = await _pollModelFactory.PreparePollModel(new PollModel(), null);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> Create(PollModel model, bool continueEditing)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManagePolls))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var poll = model.ToEntity<Poll>();
                await _pollService.InsertPoll(poll);

                //save store mappings
                await SaveStoreMappings(poll, model);

                _notificationService.SuccessNotification(await _localizationService.GetResource("Admin.ContentManagement.Polls.Added"));

                if (!continueEditing)
                    return RedirectToAction("List");

                return RedirectToAction("Edit", new { id = poll.Id });
            }

            //prepare model
            model = await _pollModelFactory.PreparePollModel(model, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual async Task<IActionResult> Edit(int id)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManagePolls))
                return AccessDeniedView();

            //try to get a poll with the specified id
            var poll = await _pollService.GetPollById(id);
            if (poll == null)
                return RedirectToAction("List");

            //prepare model
            var model = await _pollModelFactory.PreparePollModel(null, poll);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> Edit(PollModel model, bool continueEditing)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManagePolls))
                return AccessDeniedView();

            //try to get a poll with the specified id
            var poll = await _pollService.GetPollById(model.Id);
            if (poll == null)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                poll = model.ToEntity(poll);
                await _pollService.UpdatePoll(poll);

                //save store mappings
                await SaveStoreMappings(poll, model);

                _notificationService.SuccessNotification(await _localizationService.GetResource("Admin.ContentManagement.Polls.Updated"));

                if (!continueEditing)
                    return RedirectToAction("List");

                return RedirectToAction("Edit", new { id = poll.Id });
            }

            //prepare model
            model = await _pollModelFactory.PreparePollModel(model, poll, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManagePolls))
                return AccessDeniedView();

            //try to get a poll with the specified id
            var poll = await _pollService.GetPollById(id);
            if (poll == null)
                return RedirectToAction("List");

            await _pollService.DeletePoll(poll);

            _notificationService.SuccessNotification(await _localizationService.GetResource("Admin.ContentManagement.Polls.Deleted"));

            return RedirectToAction("List");
        }

        #endregion

        #region Poll answer

        [HttpPost]
        public virtual async Task<IActionResult> PollAnswers(PollAnswerSearchModel searchModel)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManagePolls))
                return AccessDeniedDataTablesJson();

            //try to get a poll with the specified id
            var poll = await _pollService.GetPollById(searchModel.PollId)
                ?? throw new ArgumentException("No poll found with the specified id");

            //prepare model
            var model = await _pollModelFactory.PreparePollAnswerListModel(searchModel, poll);

            return Json(model);
        }

        //ValidateAttribute is used to force model validation
        [HttpPost]
        public virtual async Task<IActionResult> PollAnswerUpdate([Validate] PollAnswerModel model)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManagePolls))
                return AccessDeniedView();

            if (!ModelState.IsValid)
                return ErrorJson(ModelState.SerializeErrors());

            //try to get a poll answer with the specified id
            var pollAnswer = await _pollService.GetPollAnswerById(model.Id)
                ?? throw new ArgumentException("No poll answer found with the specified id");

            pollAnswer = model.ToEntity(pollAnswer);

            await _pollService.UpdatePollAnswer(pollAnswer);

            return new NullJsonResult();
        }

        //ValidateAttribute is used to force model validation
        [HttpPost]
        public virtual async Task<IActionResult> PollAnswerAdd(int pollId, [Validate] PollAnswerModel model)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManagePolls))
                return AccessDeniedView();

            if (!ModelState.IsValid)
                return ErrorJson(ModelState.SerializeErrors());

            //fill entity from model
            await _pollService.InsertPollAnswer(model.ToEntity<PollAnswer>());

            return Json(new { Result = true });
        }

        [HttpPost]
        public virtual async Task<IActionResult> PollAnswerDelete(int id)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManagePolls))
                return AccessDeniedView();

            //try to get a poll answer with the specified id
            var pollAnswer = await _pollService.GetPollAnswerById(id)
                ?? throw new ArgumentException("No poll answer found with the specified id", nameof(id));

            await _pollService.DeletePollAnswer(pollAnswer);

            return new NullJsonResult();
        }

        #endregion
    }
}