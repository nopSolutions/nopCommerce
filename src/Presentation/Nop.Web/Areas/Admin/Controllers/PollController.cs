using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Areas.Admin.Extensions;
using Nop.Web.Areas.Admin.Models.Polls;
using Nop.Core.Domain.Polls;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Polls;
using Nop.Services.Security;
using Nop.Web.Framework.Kendoui;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Services.Stores;

namespace Nop.Web.Areas.Admin.Controllers
{
    public partial class PollController : BaseAdminController
	{
        #region Fields

        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly ILanguageService _languageService;
        private readonly ILocalizationService _localizationService;
        private readonly IPermissionService _permissionService;
        private readonly IPollService _pollService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IStoreService _storeService;

		#endregion

		#region Ctor

        public PollController(IDateTimeHelper dateTimeHelper, 
            ILanguageService languageService,
            ILocalizationService localizationService,
            IPermissionService permissionService,
            IPollService pollService, 
            IStoreMappingService storeMappingService,
            IStoreService storeService)
        {
            this._dateTimeHelper = dateTimeHelper;
            this._languageService = languageService;
            this._localizationService = localizationService;
            this._permissionService = permissionService;
            this._pollService = pollService;
            this._storeMappingService = storeMappingService;
            this._storeService = storeService;
        }

        #endregion

        #region Utilities

        protected virtual void PrepareLanguagesModel(PollModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            var languages = _languageService.GetAllLanguages(true);
            foreach (var language in languages)
            {
                model.AvailableLanguages.Add(new SelectListItem
                {
                    Text = language.Name,
                    Value = language.Id.ToString()
                });
            }
        }

        protected virtual void PrepareStoresMappingModel(PollModel model, Poll poll, bool excludeProperties)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            //prepare selected stores
            if (!excludeProperties && poll != null)
                model.SelectedStoreIds = _storeMappingService.GetStoresIdsWithAccess(poll).ToList();

            //prepare all available stores
            foreach (var store in _storeService.GetAllStores())
            {
                model.AvailableStores.Add(new SelectListItem
                {
                    Text = store.Name,
                    Value = store.Id.ToString(),
                    Selected = model.SelectedStoreIds.Contains(store.Id)
                });
            }
        }

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

            var model = new PollListModel();

            //prepare stores
            model.AvailableStores.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            foreach (var store in _storeService.GetAllStores())
                model.AvailableStores.Add(new SelectListItem { Text = store.Name, Value = store.Id.ToString() });

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult List(PollListModel listModel, DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePolls))
                return AccessDeniedKendoGridJson();

            var polls = _pollService.GetPolls(listModel.SearchStoreId, 
                showHidden: true, pageIndex: command.Page - 1, pageSize: command.PageSize);
            var gridModel = new DataSourceResult
            {
                Data = polls.Select(poll =>
                {
                    var model = poll.ToModel();

                    //get user dates
                    if (poll.StartDateUtc.HasValue)
                        model.StartDate = _dateTimeHelper.ConvertToUserTime(poll.StartDateUtc.Value, DateTimeKind.Utc);
                    if (poll.EndDateUtc.HasValue)
                        model.EndDate = _dateTimeHelper.ConvertToUserTime(poll.EndDateUtc.Value, DateTimeKind.Utc);

                    //get language name
                    model.LanguageName = _languageService.GetLanguageById(poll.LanguageId)?.Name;

                    return model;
                }),
                Total = polls.TotalCount
            };

            return Json(gridModel);
        }

        public virtual IActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePolls))
                return AccessDeniedView();

            var model = new PollModel();

            //languages
            PrepareLanguagesModel(model);
            
            //prepare stores
            PrepareStoresMappingModel(model, null, false);
            
            //default values
            model.Published = true;
            model.ShowOnHomePage = true;

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult Create(PollModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePolls))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var poll = model.ToEntity();
                poll.StartDateUtc = model.StartDate;
                poll.EndDateUtc = model.EndDate;
                _pollService.InsertPoll(poll);

                //save store mappings
                SaveStoreMappings(poll, model);

                SuccessNotification(_localizationService.GetResource("Admin.ContentManagement.Polls.Added"));

                if (continueEditing)
                {
                    //selected tab
                    SaveSelectedTabName();

                    return RedirectToAction("Edit", new { id = poll.Id });
                }
                return RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            PrepareLanguagesModel(model);
            PrepareStoresMappingModel(model, null, true);

            return View(model);
        }

        public virtual IActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePolls))
                return AccessDeniedView();

            var poll = _pollService.GetPollById(id);
            if (poll == null)
                //No poll found with the specified id
                return RedirectToAction("List");
            
            var model = poll.ToModel();

            model.StartDate = poll.StartDateUtc;
            model.EndDate = poll.EndDateUtc;
            
            //languages
            PrepareLanguagesModel(model);

            //prepare stores
            PrepareStoresMappingModel(model, poll, false);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult Edit(PollModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePolls))
                return AccessDeniedView();

            var poll = _pollService.GetPollById(model.Id);
            if (poll == null)
                //No poll found with the specified id
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

                if (continueEditing)
                {
                    //selected tab
                    SaveSelectedTabName();

                    return RedirectToAction("Edit", new { id = poll.Id });
                }
                return RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            PrepareLanguagesModel(model);
            PrepareStoresMappingModel(model, poll, true);

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePolls))
                return AccessDeniedView();

            var poll = _pollService.GetPollById(id);
            if (poll == null)
                //No poll found with the specified id
                return RedirectToAction("List");
            
            _pollService.DeletePoll(poll);

            SuccessNotification(_localizationService.GetResource("Admin.ContentManagement.Polls.Deleted"));
            return RedirectToAction("List");
        }

        #endregion

        #region Poll answer

        [HttpPost]
        public virtual IActionResult PollAnswers(int pollId, DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePolls))
                return AccessDeniedKendoGridJson();

            var poll = _pollService.GetPollById(pollId)
                ?? throw new ArgumentException("No poll found with the specified id", nameof(pollId));
            
            var answers = poll.PollAnswers.OrderBy(pollAnswer => pollAnswer.DisplayOrder).ToList();
            var gridModel = new DataSourceResult
            {
                Data = answers.Select(pollAnswer => new PollAnswerModel
                {
                    Id = pollAnswer.Id,
                    PollId = pollAnswer.PollId,
                    Name = pollAnswer.Name,
                    NumberOfVotes = pollAnswer.NumberOfVotes,
                    DisplayOrder = pollAnswer.DisplayOrder
                }),
                Total = answers.Count
            };

            return Json(gridModel);
        }
        
        [HttpPost]
        public virtual IActionResult PollAnswerUpdate(PollAnswerModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePolls))
                return AccessDeniedView();
            
            if (!ModelState.IsValid)
                return Json(new DataSourceResult { Errors = ModelState.SerializeErrors() });

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

            var pollAnswer = _pollService.GetPollAnswerById(id) 
                ?? throw new ArgumentException("No poll answer found with the specified id", nameof(id));
            
            _pollService.DeletePollAnswer(pollAnswer);

            return new NullJsonResult();
        }

        #endregion
    }
}