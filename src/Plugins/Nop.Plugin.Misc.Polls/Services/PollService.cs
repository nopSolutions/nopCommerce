using Nop.Core;
using Nop.Data;
using Nop.Plugin.Misc.Polls.Domain;
using Nop.Services.Stores;

namespace Nop.Plugin.Misc.Polls.Services;

/// <summary>
/// Poll service
/// </summary>
public class PollService
{
    #region Fields

    private readonly IRepository<Poll> _pollRepository;
    private readonly IRepository<PollAnswer> _pollAnswerRepository;
    private readonly IRepository<PollVotingRecord> _pollVotingRecordRepository;
    private readonly IStoreMappingService _storeMappingService;

    #endregion

    #region Ctor

    public PollService(IRepository<Poll> pollRepository,
        IRepository<PollAnswer> pollAnswerRepository,
        IRepository<PollVotingRecord> pollVotingRecordRepository,
        IStoreMappingService storeMappingService)
    {
        _pollRepository = pollRepository;
        _pollAnswerRepository = pollAnswerRepository;
        _pollVotingRecordRepository = pollVotingRecordRepository;
        _storeMappingService = storeMappingService;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Gets a poll
    /// </summary>
    /// <param name="pollId">The poll identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the poll
    /// </returns>
    public async Task<Poll> GetPollByIdAsync(int pollId)
    {
        return await _pollRepository.GetByIdAsync(pollId, cache => default);
    }

    /// <summary>
    /// Gets polls
    /// </summary>
    /// <param name="storeId">The store identifier; pass 0 to load all records</param>
    /// <param name="languageId">Language identifier; pass 0 to load all records</param>
    /// <param name="showHidden">Whether to show hidden records (not published, not started and expired)</param>
    /// <param name="loadShownOnHomepageOnly">Retrieve only the polls that are marked as show on the home page</param>
    /// <param name="loadShowInLeftSideOnly">Retrieve only the polls that are marked as show in the left-side column</param>
    /// <param name="systemKeyword">The poll system keyword; pass null to load all records</param>
    /// <param name="pageIndex">Page index</param>
    /// <param name="pageSize">Page size</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the polls
    /// </returns>
    public async Task<IPagedList<Poll>> GetPollsAsync(int storeId = 0, int languageId = 0, bool showHidden = false,
        bool loadShownOnHomepageOnly = false, bool loadShowInLeftSideOnly = false, string systemKeyword = null,
        int pageIndex = 0, int pageSize = int.MaxValue)
    {
        var query = _pollRepository.Table;

        if (!showHidden || storeId > 0)
            //apply store mapping constraints
            query = await _storeMappingService.ApplyStoreMapping(query, storeId);

        //whether to load not published, not started and expired polls
        if (!showHidden)
        {
            var utcNow = DateTime.UtcNow;
            query = query.Where(poll => poll.Published);
            query = query.Where(poll => !poll.StartDateUtc.HasValue || poll.StartDateUtc <= utcNow);
            query = query.Where(poll => !poll.EndDateUtc.HasValue || poll.EndDateUtc >= utcNow);
        }

        //load homepage polls only
        if (loadShownOnHomepageOnly)
            query = query.Where(poll => poll.ShowOnHomepage);

        //load left-side column polls only
        if (loadShowInLeftSideOnly)
            query = query.Where(poll => poll.ShowInLeftSideColumn);

        //filter by language
        if (languageId > 0)
            query = query.Where(poll => poll.LanguageId == languageId);

        //filter by system keyword
        if (!string.IsNullOrEmpty(systemKeyword))
            query = query.Where(poll => poll.SystemKeyword == systemKeyword);

        //order records by display order
        query = query.OrderBy(poll => poll.DisplayOrder).ThenBy(poll => poll.Id);

        //return paged list of polls
        return await query.ToPagedListAsync(pageIndex, pageSize);
    }

    /// <summary>
    /// Deletes a poll
    /// </summary>
    /// <param name="poll">The poll</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task DeletePollAsync(Poll poll)
    {
        await _pollRepository.DeleteAsync(poll);
    }

    /// <summary>
    /// Inserts a poll
    /// </summary>
    /// <param name="poll">Poll</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task InsertPollAsync(Poll poll)
    {
        await _pollRepository.InsertAsync(poll);
    }

    /// <summary>
    /// Updates the poll
    /// </summary>
    /// <param name="poll">Poll</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task UpdatePollAsync(Poll poll)
    {
        await _pollRepository.UpdateAsync(poll);
    }

    /// <summary>
    /// Gets a poll answer
    /// </summary>
    /// <param name="pollAnswerId">Poll answer identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the poll answer
    /// </returns>
    public async Task<PollAnswer> GetPollAnswerByIdAsync(int pollAnswerId)
    {
        return await _pollAnswerRepository.GetByIdAsync(pollAnswerId, cache => default);
    }

    /// <summary>
    /// Deletes a poll answer
    /// </summary>
    /// <param name="pollAnswer">Poll answer</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task DeletePollAnswerAsync(PollAnswer pollAnswer)
    {
        await _pollAnswerRepository.DeleteAsync(pollAnswer);
    }

    /// <summary>
    /// Gets a poll answers by parent poll
    /// </summary>
    /// <param name="pollId">The poll identifier</param>
    /// <param name="pageIndex">Page index</param>
    /// <param name="pageSize">Page size</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the poll answers
    /// </returns>
    public async Task<IPagedList<PollAnswer>> GetPollAnswerByPollAsync(int pollId, int pageIndex = 0, int pageSize = int.MaxValue)
    {
        var query = _pollAnswerRepository.Table.Where(pa => pa.PollId == pollId);

        //order records by display order
        query = query.OrderBy(pa => pa.DisplayOrder).ThenBy(pa => pa.Id);

        //return paged list of polls
        return await query.ToPagedListAsync(pageIndex, pageSize);
    }

    /// <summary>
    /// Inserts a poll answer
    /// </summary>
    /// <param name="pollAnswer">Poll answer</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task InsertPollAnswerAsync(PollAnswer pollAnswer)
    {
        await _pollAnswerRepository.InsertAsync(pollAnswer);
    }

    /// <summary>
    /// Updates the poll answer
    /// </summary>
    /// <param name="pollAnswer">Poll answer</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task UpdatePollAnswerAsync(PollAnswer pollAnswer)
    {
        await _pollAnswerRepository.UpdateAsync(pollAnswer);
    }

    /// <summary>
    /// Gets a value indicating whether customer already voted for this poll
    /// </summary>
    /// <param name="pollId">Poll identifier</param>
    /// <param name="customerId">Customer identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the result
    /// </returns>
    public async Task<bool> AlreadyVotedAsync(int pollId, int customerId)
    {
        if (pollId == 0 || customerId == 0)
            return false;

        return await
            (from pa in _pollAnswerRepository.Table
             join pvr in _pollVotingRecordRepository.Table on pa.Id equals pvr.PollAnswerId
             where pa.PollId == pollId && pvr.CustomerId == customerId
             select pvr)
            .AnyAsync();
    }

    /// <summary>
    /// Inserts a poll voting record
    /// </summary>
    /// <param name="pollVotingRecord">Voting record</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task InsertPollVotingRecordAsync(PollVotingRecord pollVotingRecord)
    {
        await _pollVotingRecordRepository.InsertAsync(pollVotingRecord);
    }

    /// <summary>
    /// Gets a poll voting records by parent answer
    /// </summary>
    /// <param name="pollAnswerId">Poll answer identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the poll voting records
    /// </returns>
    public async Task<IList<PollVotingRecord>> GetPollVotingRecordsByPollAnswerAsync(int pollAnswerId)
    {
        var query = _pollVotingRecordRepository.Table.Where(pa => pa.PollAnswerId == pollAnswerId);

        return await query.ToListAsync();
    }

    #endregion
}