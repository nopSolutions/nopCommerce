using Nop.Core;
using Nop.Core.Caching;
using Nop.Plugin.Misc.Polls.Domain;
using Nop.Plugin.Misc.Polls.Public.Models;
using Nop.Plugin.Misc.Polls.Services;

namespace Nop.Plugin.Misc.Polls.Public.Factories;

/// <summary>
/// Represents the poll model factory
/// </summary>
public class PollModelFactory
{
    #region Fields

    private readonly IStaticCacheManager _staticCacheManager;
    private readonly IStoreContext _storeContext;
    private readonly IWorkContext _workContext;
    private readonly PollService _pollService;

    #endregion

    #region Ctor

    public PollModelFactory(IStaticCacheManager staticCacheManager,
        IStoreContext storeContext,
        IWorkContext workContext,
        PollService pollService)
    {
        _staticCacheManager = staticCacheManager;
        _storeContext = storeContext;
        _workContext = workContext;
        _pollService = pollService;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Prepare the poll model
    /// </summary>
    /// <param name="poll">Poll</param>
    /// <param name="setAlreadyVotedProperty">Whether to load a value indicating that customer already voted for this poll</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the poll model
    /// </returns>
    public async Task<PollModel> PreparePollModelAsync(Poll poll, bool setAlreadyVotedProperty)
    {
        ArgumentNullException.ThrowIfNull(poll);

        var customer = await _workContext.GetCurrentCustomerAsync();

        var model = new PollModel
        {
            Id = poll.Id,
            AlreadyVoted = setAlreadyVotedProperty && await _pollService.AlreadyVotedAsync(poll.Id, customer.Id),
            Name = poll.Name
        };
        var answers = await _pollService.GetPollAnswerByPollAsync(poll.Id);

        model.TotalVotes = answers.Sum(answer => answer.NumberOfVotes);
        foreach (var pa in answers)
        {
            model.Answers.Add(new()
            {
                Id = pa.Id,
                Name = pa.Name,
                NumberOfVotes = pa.NumberOfVotes,
                PercentOfTotalVotes = model.TotalVotes > 0 ? Convert.ToDouble(pa.NumberOfVotes) / Convert.ToDouble(model.TotalVotes) * Convert.ToDouble(100) : 0,
            });
        }

        return model;
    }

    /// <summary>
    /// Prepare the left side poll models
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the list of the poll model
    /// </returns>
    public async Task<PollModel> PrepareLeftSidePollModelAsync()
    {
        var store = await _storeContext.GetCurrentStoreAsync();
        var language = await _workContext.GetWorkingLanguageAsync();
        var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(PollsDefaults.LeftColumnPollsModelKey, language, store);

        var cachedPoll = await _staticCacheManager.GetAsync(cacheKey, async () =>
        {
            var polls = await _pollService.GetPollsAsync(store.Id, language.Id, loadShowInLeftSideOnly: true, pageSize: 1);

            if (!polls.Any())
                return null;

            return await PreparePollModelAsync(polls.Single(), false);
        });

        if (cachedPoll is null)
            return null;

        var customer = await _workContext.GetCurrentCustomerAsync();

        //return clone the cached model
        return cachedPoll with { AlreadyVoted = await _pollService.AlreadyVotedAsync(cachedPoll.Id, customer.Id) };
    }

    /// <summary>
    /// Prepare the home page poll models
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the list of the poll model
    /// </returns>
    public async Task<List<PollModel>> PrepareHomepagePollModelsAsync()
    {
        var customer = await _workContext.GetCurrentCustomerAsync();
        var store = await _storeContext.GetCurrentStoreAsync();
        var language = await _workContext.GetWorkingLanguageAsync();
        var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(PollsDefaults.HomepagePollsModelKey, language, store);

        var cachedPolls = await _staticCacheManager.GetAsync(cacheKey, async () =>
        {
            var polls = await _pollService.GetPollsAsync(store.Id, language.Id, loadShownOnHomepageOnly: true);
            var pollsModels = await polls.SelectAwait(async poll => await PreparePollModelAsync(poll, false)).ToListAsync();
            return pollsModels;
        });

        //"AlreadyVoted" property of "PollModel" object depends on the current customer. Let's update it.
        //But first we need to clone the cached model (the updated one should not be cached)
        var model = new List<PollModel>();
        foreach (var poll in cachedPolls)
        {
            var pollModel = poll with { };
            pollModel.AlreadyVoted = await _pollService.AlreadyVotedAsync(pollModel.Id, customer.Id);
            model.Add(pollModel);
        }

        return model;
    }

    #endregion
}