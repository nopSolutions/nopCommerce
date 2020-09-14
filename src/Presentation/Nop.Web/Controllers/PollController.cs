using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Polls;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Polls;
using Nop.Services.Stores;
using Nop.Web.Factories;

namespace Nop.Web.Controllers
{
    public partial class PollController : BasePublicController
    {
        #region Fields

        private readonly ICustomerService _customerService;
        private readonly ILocalizationService _localizationService;
        private readonly IPollModelFactory _pollModelFactory;
        private readonly IPollService _pollService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public PollController(ICustomerService customerService, 
            ILocalizationService localizationService, 
            IPollModelFactory pollModelFactory,
            IPollService pollService,
            IStoreMappingService storeMappingService,
            IWorkContext workContext)
        {
            _customerService = customerService;
            _localizationService = localizationService;
            _pollModelFactory = pollModelFactory;
            _pollService = pollService;
            _storeMappingService = storeMappingService;
            _workContext = workContext;
        }

        #endregion

        #region Methods

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public virtual async Task<IActionResult> Vote(int pollAnswerId)
        {
            var pollAnswer = await _pollService.GetPollAnswerById(pollAnswerId);
            if (pollAnswer == null)
                return Json(new { error = "No poll answer found with the specified id" });

            var poll = await _pollService.GetPollById(pollAnswer.PollId);

            if (!poll.Published || !await _storeMappingService.Authorize(poll))
                return Json(new { error = "Poll is not available" });

            if (await _customerService.IsGuest(await _workContext.GetCurrentCustomer()) && !poll.AllowGuestsToVote)
                return Json(new { error = await _localizationService.GetResource("Polls.OnlyRegisteredUsersVote") });

            var alreadyVoted = await _pollService.AlreadyVoted(poll.Id, (await _workContext.GetCurrentCustomer()).Id);
            if (!alreadyVoted)
            {
                //vote
                await _pollService.InsertPollVotingRecord(new PollVotingRecord
                {
                    PollAnswerId = pollAnswer.Id,
                    CustomerId = (await _workContext.GetCurrentCustomer()).Id,
                    CreatedOnUtc = DateTime.UtcNow
                });

                //update totals
                pollAnswer.NumberOfVotes = (await _pollService.GetPollVotingRecordsByPollAnswer(pollAnswer.Id)).Count;
                await _pollService.UpdatePollAnswer(pollAnswer);
                await _pollService.UpdatePoll(poll);
            }

            return Json(new
            {
                html = RenderPartialViewToString("_Poll", await _pollModelFactory.PreparePollModel(poll, true)),
            });
        }

        #endregion
    }
}