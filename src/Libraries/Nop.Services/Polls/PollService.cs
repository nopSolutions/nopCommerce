using System;
using System.Linq;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Domain.Polls;
using Nop.Data;
using Nop.Services.Stores;

namespace Nop.Services.Polls
{
    /// <summary>
    /// Poll service
    /// </summary>
    public partial class PollService : IPollService
    {
        #region Fields

        protected IRepository<Poll> PollRepository { get; }
        protected IRepository<PollAnswer> PollAnswerRepository { get; }
        protected IRepository<PollVotingRecord> PollVotingRecordRepository { get; }
        protected IStoreMappingService StoreMappingService { get; }

        #endregion

        #region Ctor

        public PollService(
            IRepository<Poll> pollRepository,
            IRepository<PollAnswer> pollAnswerRepository,
            IRepository<PollVotingRecord> pollVotingRecordRepository,
            IStoreMappingService storeMappingService)
        {
            PollRepository = pollRepository;
            PollAnswerRepository = pollAnswerRepository;
            PollVotingRecordRepository = pollVotingRecordRepository;
            StoreMappingService = storeMappingService;
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
        public virtual async Task<Poll> GetPollByIdAsync(int pollId)
        {
            return await PollRepository.GetByIdAsync(pollId, cache => default);
        }

        /// <summary>
        /// Gets polls
        /// </summary>
        /// <param name="storeId">The store identifier; pass 0 to load all records</param>
        /// <param name="languageId">Language identifier; pass 0 to load all records</param>
        /// <param name="showHidden">Whether to show hidden records (not published, not started and expired)</param>
        /// <param name="loadShownOnHomepageOnly">Retrieve only shown on home page polls</param>
        /// <param name="systemKeyword">The poll system keyword; pass null to load all records</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the polls
        /// </returns>
        public virtual async Task<IPagedList<Poll>> GetPollsAsync(int storeId, int languageId = 0, bool showHidden = false,
            bool loadShownOnHomepageOnly = false, string systemKeyword = null,
            int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = PollRepository.Table;

            //whether to load not published, not started and expired polls
            if (!showHidden)
            {
                var utcNow = DateTime.UtcNow;
                query = query.Where(poll => poll.Published);
                query = query.Where(poll => !poll.StartDateUtc.HasValue || poll.StartDateUtc <= utcNow);
                query = query.Where(poll => !poll.EndDateUtc.HasValue || poll.EndDateUtc >= utcNow);
            }

            //apply store mapping constraints
            query = await StoreMappingService.ApplyStoreMapping(query, storeId);

            //load homepage polls only
            if (loadShownOnHomepageOnly)
                query = query.Where(poll => poll.ShowOnHomepage);

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
        public virtual async Task DeletePollAsync(Poll poll)
        {
            await PollRepository.DeleteAsync(poll);
        }

        /// <summary>
        /// Inserts a poll
        /// </summary>
        /// <param name="poll">Poll</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task InsertPollAsync(Poll poll)
        {
            await PollRepository.InsertAsync(poll);
        }

        /// <summary>
        /// Updates the poll
        /// </summary>
        /// <param name="poll">Poll</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task UpdatePollAsync(Poll poll)
        {
            await PollRepository.UpdateAsync(poll);
        }

        /// <summary>
        /// Gets a poll answer
        /// </summary>
        /// <param name="pollAnswerId">Poll answer identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the poll answer
        /// </returns>
        public virtual async Task<PollAnswer> GetPollAnswerByIdAsync(int pollAnswerId)
        {
            return await PollAnswerRepository.GetByIdAsync(pollAnswerId, cache => default);
        }

        /// <summary>
        /// Deletes a poll answer
        /// </summary>
        /// <param name="pollAnswer">Poll answer</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeletePollAnswerAsync(PollAnswer pollAnswer)
        {
            await PollAnswerRepository.DeleteAsync(pollAnswer);
        }

        /// <summary>
        /// Gets a poll answers by parent poll
        /// </summary>
        /// <param name="pollId">The poll identifier</param>
        /// <returns>Poll answer</returns>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task<IPagedList<PollAnswer>> GetPollAnswerByPollAsync(int pollId, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = PollAnswerRepository.Table.Where(pa => pa.PollId == pollId);

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
        public virtual async Task InsertPollAnswerAsync(PollAnswer pollAnswer)
        {
            await PollAnswerRepository.InsertAsync(pollAnswer);
        }

        /// <summary>
        /// Updates the poll answer
        /// </summary>
        /// <param name="pollAnswer">Poll answer</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task UpdatePollAnswerAsync(PollAnswer pollAnswer)
        {
            await PollAnswerRepository.UpdateAsync(pollAnswer);
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
        public virtual async Task<bool> AlreadyVotedAsync(int pollId, int customerId)
        {
            if (pollId == 0 || customerId == 0)
                return false;

            var result = await
                (from pa in PollAnswerRepository.Table
                 join pvr in PollVotingRecordRepository.Table on pa.Id equals pvr.PollAnswerId
                 where pa.PollId == pollId && pvr.CustomerId == customerId
                 select pvr)
                .AnyAsync();
            return result;
        }

        /// <summary>
        /// Inserts a poll voting record
        /// </summary>
        /// <param name="pollVotingRecord">Voting record</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task InsertPollVotingRecordAsync(PollVotingRecord pollVotingRecord)
        {
            await PollVotingRecordRepository.InsertAsync(pollVotingRecord);
        }

        /// <summary>
        /// Gets a poll voting records by parent answer
        /// </summary>
        /// <param name="pollAnswerId">Poll answer identifier</param>
        /// <returns>Poll answer</returns>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task<IPagedList<PollVotingRecord>> GetPollVotingRecordsByPollAnswerAsync(int pollAnswerId, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = PollVotingRecordRepository.Table.Where(pa => pa.PollAnswerId == pollAnswerId);

            //return paged list of poll voting records
            return await query.ToPagedListAsync(pageIndex, pageSize);
        }

        #endregion
    }
}