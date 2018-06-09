using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Polls;
using Nop.Services.Localization;
using Nop.Services.Polls;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Polls;
using Nop.Web.Framework.Kendoui;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Web.Areas.Admin.Controllers
{
    public partial class PollController : BaseAdminController
    {
        #region Fields

        private readonly ILocalizationService _localizationService;
        private readonly IPermissionService _permissionService;
        private readonly IPollModelFactory _pollModelFactory;
        private readonly IPollService _pollService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IStoreService _storeService;

        #endregion

        #region Ctor

        public PollController(ILocalizationService localizationService,
            IPermissionService permissionService,
            IPollModelFactory pollModelFactory,
            IPollService pollService,
            IStoreMappingService storeMappingService,
            IStoreService storeService)
        {
            this._localizationService = localizationService;
            this._permissionService = permissionService;
            this._pollModelFactory = pollModelFactory;
            this._pollService = pollService;
            this._storeMappingService = storeMappingService;
            this._storeService = storeService;
        }

        #endregion

        #region Utilities

        protected virtual void SaveStoreMappings(Poll poll, PollModel model)
        {
            poll.LimitedToStores = model.SelectedStoreIds.Any();

            //manage store mappings
            var existingStoreMappings = _storeMappingService.GetStoreMappings(poll);
            foreach (var store in _storeService.GetAllStores())
            {
                var existingStoreMapping = existingStoreMappings.FirstOrDefault(storeMapping => storeMapping.StoreId == store.Id);

                //new store mapping
                if (model.SelectedStoreIds.Contains(store.Id))
                {
                    if (existingStoreMapping == null)
                        _storeMappingService.InsertStoreMapping(poll, store.Id);
                }
                //or remove existing one
                else if (existingStoreMapping != null)
                    _storeMappingService.DeleteStoreMapping(existingStoreMapping);
            }
        }

        #endregion

        #region Polls

        public virtual IActionResult Index()
        {
            return RedirectToAction("List");
        }

        public virtual IActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePolls))
                return AccessDeniedView();

            //prepare model
            var model = _pollModelFactory.PreparePollSearchModel(new PollSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult List(PollSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePolls))
                return AccessDeniedKendoGridJson();

            //prepare model
            var model = _pollModelFactory.PreparePollListModel(searchModel);

            return Json(model);
        }

        public virtual IActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePolls))
                return AccessDeniedView();

            //prepare model
            var model = _pollModelFactory.PreparePollModel(new PollModel(), null);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult Create(PollModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePolls))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var poll = model.ToEntity<Poll>();
                poll.StartDateUtc = model.StartDate;
                poll.EndDateUtc = model.EndDate;
                _pollService.InsertPoll(poll);

                //save store mappings
                SaveStoreMappings(poll, model);

                SuccessNotification(_localizationService.GetResource("Admin.ContentManagement.Polls.Added"));

                if (!continueEditing)
                    return RedirectToAction("List");

                //selected tab
                SaveSelectedTabName();

                return RedirectToAction("Edit", new { id = poll.Id });
            }

            //prepare model
            model = _pollModelFactory.PreparePollModel(model, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual IActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePolls))
                return AccessDeniedView();

            //try to get a poll with the specified id
            var poll = _pollService.GetPollById(id);
            if (poll == null)
                return RedirectToAction("List");

            //prepare model
            var model = _pollModelFactory.PreparePollModel(null, poll);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult Edit(PollModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePolls))
                return AccessDeniedView();

            //try to get a poll with the specified id
            var poll = _pollService.GetPollById(model.Id);
            if (poll == null)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                poll = model.ToEntity(poll);
                poll.StartDateUtc = model.StartDate;
                poll.EndDateUtc = model.EndDate;
                _pollService.UpdatePoll(poll);

                //save store mappings
                SaveStoreMappings(poll, model);

                SuccessNotification(_localizationService.GetResource("Admin.ContentManagement.Polls.Updated"));

                if (!continueEditing)
                    return RedirectToAction("List");

                //selected tab
                SaveSelectedTabName();

                return RedirectToAction("Edit", new { id = poll.Id });
            }

            //prepare model
            model = _pollModelFactory.PreparePollModel(model, poll, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual IActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePolls))
                return AccessDeniedView();

            //try to get a poll with the specified id
            var poll = _pollService.GetPollById(id);
            if (poll == null)
                return RedirectToAction("List");

            _pollService.DeletePoll(poll);

            SuccessNotification(_localizationService.GetResource("Admin.ContentManagement.Polls.Deleted"));

            return RedirectToAction("List");
        }

        #endregion

        #region Poll answer

        [HttpPost]
        public virtual IActionResult PollAnswers(PollAnswerSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePolls))
                return AccessDeniedKendoGridJson();

            //try to get a poll with the specified id
            var poll = _pollService.GetPollById(searchModel.PollId)
                ?? throw new ArgumentException("No poll found with the specified id");

            //prepare model
            var model = _pollModelFactory.PreparePollAnswerListModel(searchModel, poll);

            return Json(model);
        }

        [HttpPost]
        public virtual IActionResult PollAnswerUpdate(PollAnswerModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePolls))
                return AccessDeniedView();

            if (!ModelState.IsValid)
                return Json(new DataSourceResult { Errors = ModelState.SerializeErrors() });

            //try to get a poll answer with the specified id
            var pollAnswer = _pollService.GetPollAnswerById(model.Id)
                ?? throw new ArgumentException("No poll answer found with the specified id");

            pollAnswer.Name = model.Name;
            pollAnswer.DisplayOrder = model.DisplayOrder;
            _pollService.UpdatePoll(pollAnswer.Poll);

            return new NullJsonResult();
        }

        [HttpPost]
        public virtual IActionResult PollAnswerAdd(int pollId, PollAnswerModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePolls))
                return AccessDeniedView();

            if (!ModelState.IsValid)
                return Json(new DataSourceResult { Errors = ModelState.SerializeErrors() });

            //try to get a poll with the specified id
            var poll = _pollService.GetPollById(pollId)
                ?? throw new ArgumentException("No poll found with the specified id", nameof(pollId));

            poll.PollAnswers.Add(new PollAnswer
            {
                Name = model.Name,
                DisplayOrder = model.DisplayOrder
            });
            _pollService.UpdatePoll(poll);

            return new NullJsonResult();
        }

        [HttpPost]
        public virtual IActionResult PollAnswerDelete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePolls))
                return AccessDeniedView();

            //try to get a poll answer with the specified id
            var pollAnswer = _pollService.GetPollAnswerById(id)
                ?? throw new ArgumentException("No poll answer found with the specified id", nameof(id));

            _pollService.DeletePollAnswer(pollAnswer);

            return new NullJsonResult();
        }

        #endregion
    }
}