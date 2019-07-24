using System;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Polls;
using Nop.Services.Localization;
using Nop.Services.Polls;
using Nop.Services.Stores;
using Nop.Web.Factories;

namespace Nop.Web.Controllers
{
    public partial class PollController : BasePublicController
    {
        #region Fields

        private readonly ILocalizationService _localizationService;
        private readonly IPollModelFactory _pollModelFactory;
        private readonly IPollService _pollService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public PollController(ILocalizationService localizationService, 
            IPollModelFactory pollModelFactory,
            IPollService pollService,
            IStoreMappingService storeMappingService,
            IWorkContext workContext)
        {
            _localizationService = localizationService;
            _pollModelFactory = pollModelFactory;
            _pollService = pollService;
            _storeMappingService = storeMappingService;
            _workContext = workContext;
        }

        #endregion

        #region Methods

        [HttpPost]
        public virtual IActionResult Vote(int pollAnswerId)
        {
            var pollAnswer = _pollService.GetPollAnswerById(pollAnswerId);
            if (pollAnswer == null)
                return Json(new { error = "No poll answer found with the specified id" });

            var poll = pollAnswer.Poll;
            if (!poll.Published || !_storeMappingService.Authorize(poll))
                return Json(new { error = "Poll is not available" });

            if (_workContext.CurrentCustomer.IsGuest() && !poll.AllowGuestsToVote)
                return Json(new { error = _localizationService.GetResource("Polls.OnlyRegisteredUsersVote") });

            var alreadyVoted = _pollService.AlreadyVoted(poll.Id, _workContext.CurrentCustomer.Id);
            if (!alreadyVoted)
            {
                //vote
                pollAnswer.PollVotingRecords.Add(new PollVotingRecord
                {
                    PollAnswerId = pollAnswer.Id,
                    CustomerId = _workContext.CurrentCustomer.Id,
                    CreatedOnUtc = DateTime.UtcNow
                });

                //update totals
                pollAnswer.NumberOfVotes = pollAnswer.PollVotingRecords.Count;
                _pollService.UpdatePoll(poll);
            }

            return Json(new
            {
                html = RenderPartialViewToString("_Poll", _pollModelFactory.PreparePollModel(poll, true)),
            });
        }

        #endregion
    }
}