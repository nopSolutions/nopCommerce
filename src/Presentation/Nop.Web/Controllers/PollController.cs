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
    [AutoValidateAntiforgeryToken]
    public partial class PollController : BasePublicController
    {
        #region Fields

        protected ICustomerService CustomerService { get; }
        protected ILocalizationService LocalizationService { get; }
        protected IPollModelFactory PollModelFactory { get; }
        protected IPollService PollService { get; }
        protected IStoreMappingService StoreMappingService { get; }
        protected IWorkContext WorkContext { get; }

        #endregion

        #region Ctor

        public PollController(ICustomerService customerService, 
            ILocalizationService localizationService, 
            IPollModelFactory pollModelFactory,
            IPollService pollService,
            IStoreMappingService storeMappingService,
            IWorkContext workContext)
        {
            CustomerService = customerService;
            LocalizationService = localizationService;
            PollModelFactory = pollModelFactory;
            PollService = pollService;
            StoreMappingService = storeMappingService;
            WorkContext = workContext;
        }

        #endregion

        #region Methods

        [HttpPost]
        public virtual async Task<IActionResult> Vote(int pollAnswerId)
        {
            var pollAnswer = await PollService.GetPollAnswerByIdAsync(pollAnswerId);
            if (pollAnswer == null)
                return Json(new { error = "No poll answer found with the specified id" });

            var poll = await PollService.GetPollByIdAsync(pollAnswer.PollId);

            if (!poll.Published || !await StoreMappingService.AuthorizeAsync(poll))
                return Json(new { error = "Poll is not available" });

            var customer = await WorkContext.GetCurrentCustomerAsync();
            if (await CustomerService.IsGuestAsync(customer) && !poll.AllowGuestsToVote)
                return Json(new { error = await LocalizationService.GetResourceAsync("Polls.OnlyRegisteredUsersVote") });

            var alreadyVoted = await PollService.AlreadyVotedAsync(poll.Id, customer.Id);
            if (!alreadyVoted)
            {
                //vote
                await PollService.InsertPollVotingRecordAsync(new PollVotingRecord
                {
                    PollAnswerId = pollAnswer.Id,
                    CustomerId = customer.Id,
                    CreatedOnUtc = DateTime.UtcNow
                });

                //update totals
                pollAnswer.NumberOfVotes = (await PollService.GetPollVotingRecordsByPollAnswerAsync(pollAnswer.Id)).Count;
                await PollService.UpdatePollAnswerAsync(pollAnswer);
                await PollService.UpdatePollAsync(poll);
            }

            return Json(new
            {
                html = await RenderPartialViewToStringAsync("_Poll", await PollModelFactory.PreparePollModelAsync(poll, true)),
            });
        }

        #endregion
    }
}