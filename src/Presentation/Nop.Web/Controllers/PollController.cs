using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Polls;
using Nop.Services.Localization;
using Nop.Services.Polls;
using Nop.Web.Factories;

namespace Nop.Web.Controllers
{
    public partial class PollController : BasePublicController
    {
        #region Fields

        private readonly IPollModelFactory _pollModelFactory;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;
        private readonly IPollService _pollService;

        #endregion

        #region Constructors

        public PollController(IPollModelFactory pollModelFactory,
            ILocalizationService localizationService,
            IWorkContext workContext,
            IPollService pollService)
        {
            this._pollModelFactory = pollModelFactory;
            this._localizationService = localizationService;
            this._workContext = workContext;
            this._pollService = pollService;
        }

        #endregion

        #region Methods

        [HttpPost]
        public virtual ActionResult Vote(int pollAnswerId)
        {
            var pollAnswer = _pollService.GetPollAnswerById(pollAnswerId);
            if (pollAnswer == null)
                return Json(new
                {
                    error = "No poll answer found with the specified id",
                });

            var poll = pollAnswer.Poll;
            if (!poll.Published)
                return Json(new
                {
                    error = "Poll is not available",
                });

            if (_workContext.CurrentCustomer.IsGuest() && !poll.AllowGuestsToVote)
                return Json(new
                {
                    error = _localizationService.GetResource("Polls.OnlyRegisteredUsersVote"),
                });

            bool alreadyVoted = _pollService.AlreadyVoted(poll.Id, _workContext.CurrentCustomer.Id);
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
                html = this.RenderPartialViewToString("_Poll", _pollModelFactory.PreparePollModel(poll, true)),
            });
        }
#endregion

    }
}