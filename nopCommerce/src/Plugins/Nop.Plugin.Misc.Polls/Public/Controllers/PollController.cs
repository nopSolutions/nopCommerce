using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Plugin.Misc.Polls.Public.Factories;
using Nop.Plugin.Misc.Polls.Services;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Stores;
using Nop.Web.Controllers;

namespace Nop.Plugin.Misc.Polls.Public.Controllers;

[AutoValidateAntiforgeryToken]
public class PollController : BasePublicController
{
    #region Fields

    private readonly ICustomerService _customerService;
    private readonly ILocalizationService _localizationService;
    private readonly IStoreMappingService _storeMappingService;
    private readonly IWorkContext _workContext;
    private readonly PollModelFactory _pollModelFactory;
    private readonly PollService _pollService;

    #endregion

    #region Ctor

    public PollController(ICustomerService customerService,
        ILocalizationService localizationService,
        IStoreMappingService storeMappingService,
        IWorkContext workContext,
        PollModelFactory pollModelFactory,
        PollService pollService)
    {
        _customerService = customerService;
        _localizationService = localizationService;
        _storeMappingService = storeMappingService;
        _workContext = workContext;
        _pollModelFactory = pollModelFactory;
        _pollService = pollService;
    }

    #endregion

    #region Methods

    [HttpPost]
    public async Task<IActionResult> Vote(int pollAnswerId)
    {
        var pollAnswer = await _pollService.GetPollAnswerByIdAsync(pollAnswerId);
        if (pollAnswer == null)
            return Json(new { error = "No poll answer found with the specified id" });

        var poll = await _pollService.GetPollByIdAsync(pollAnswer.PollId);

        if (!poll.Published || !await _storeMappingService.AuthorizeAsync(poll))
            return Json(new { error = "Poll is not available" });

        var customer = await _workContext.GetCurrentCustomerAsync();
        if (await _customerService.IsGuestAsync(customer) && !poll.AllowGuestsToVote)
            return Json(new { error = await _localizationService.GetResourceAsync("Plugins.Misc.Polls.OnlyRegisteredUsersVote") });

        var alreadyVoted = await _pollService.AlreadyVotedAsync(poll.Id, customer.Id);
        if (!alreadyVoted)
        {
            //vote
            await _pollService.InsertPollVotingRecordAsync(new()
            {
                PollAnswerId = pollAnswer.Id,
                CustomerId = customer.Id,
                CreatedOnUtc = DateTime.UtcNow
            });

            //update totals
            pollAnswer.NumberOfVotes = (await _pollService.GetPollVotingRecordsByPollAnswerAsync(pollAnswer.Id)).Count;
            await _pollService.UpdatePollAnswerAsync(pollAnswer);
            await _pollService.UpdatePollAsync(poll);
        }

        return Json(new
        {
            html = await RenderPartialViewToStringAsync("~/Plugins/Misc.Polls/Public/Views/_Poll.cshtml", await _pollModelFactory.PreparePollModelAsync(poll, true)),
        });
    }

    #endregion
}