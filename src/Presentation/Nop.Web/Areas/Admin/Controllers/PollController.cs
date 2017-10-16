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

namespace Nop.Web.Areas.Admin.Controllers
{
    public partial class PollController : BaseAdminController
	{
		#region Fields

        private readonly IPollService _pollService;
        private readonly ILanguageService _languageService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly ILocalizationService _localizationService;
        private readonly IPermissionService _permissionService;

		#endregion

		#region Ctor

        public PollController(IPollService pollService, ILanguageService languageService,
            IDateTimeHelper dateTimeHelper, ILocalizationService localizationService,
            IPermissionService permissionService)
        {
            this._pollService = pollService;
            this._languageService = languageService;
            this._dateTimeHelper = dateTimeHelper;
            this._localizationService = localizationService;
            this._permissionService = permissionService;
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

            return View();
        }

        [HttpPost]
        public virtual IActionResult List(DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePolls))
                return AccessDeniedKendoGridJson();

            var polls = _pollService.GetPolls(0, false, null, command.Page - 1, command.PageSize, true);
            var gridModel = new DataSourceResult
            {
                Data = polls.Select(x =>
                {
                    var m = x.ToModel();
                    if (x.StartDateUtc.HasValue)
                        m.StartDate = _dateTimeHelper.ConvertToUserTime(x.StartDateUtc.Value, DateTimeKind.Utc);
                    if (x.EndDateUtc.HasValue)
                        m.EndDate = _dateTimeHelper.ConvertToUserTime(x.EndDateUtc.Value, DateTimeKind.Utc);
                    m.LanguageName = _languageService.GetLanguageById(x.LanguageId)?.Name;
                    return m;
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

            var poll = _pollService.GetPollById(pollId);
            if (poll == null)
                throw new ArgumentException("No poll found with the specified id", "pollId");

            var answers = poll.PollAnswers.OrderBy(x=>x.DisplayOrder).ToList();

            var gridModel = new DataSourceResult
            {
                Data = answers.Select(x =>  new PollAnswerModel
                {
                    Id = x.Id,
                    PollId = x.PollId,
                    Name = x.Name,
                    NumberOfVotes = x.NumberOfVotes,
                    DisplayOrder = x.DisplayOrder
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
            {
                return Json(new DataSourceResult { Errors = ModelState.SerializeErrors() });
            }

            var pollAnswer = _pollService.GetPollAnswerById(model.Id);
            if (pollAnswer == null)
                throw new ArgumentException("No poll answer found with the specified id");

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
            {
                return Json(new DataSourceResult { Errors = ModelState.SerializeErrors() });
            }

            var poll = _pollService.GetPollById(pollId);
            if (poll == null)
                throw new ArgumentException("No poll found with the specified id", "pollId");

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

            var pollAnswer = _pollService.GetPollAnswerById(id);
            if (pollAnswer == null)
                throw new ArgumentException("No poll answer found with the specified id", "id");

            //int pollId = pollAnswer.PollId;
            _pollService.DeletePollAnswer(pollAnswer);

            return new NullJsonResult();
        }

        #endregion
    }
}