using System;
using System.Linq;
using System.Threading.Tasks;
using LinqToDB;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Polls;
using Nop.Core.Domain.Stores;
using Nop.Data;

namespace Nop.Services.Polls
{
    /// <summary>
    /// Poll service
    /// </summary>
    public partial class PollService : IPollService
    {
        #region Fields

        private readonly CatalogSettings _catalogSettings;
        private readonly IRepository<Poll> _pollRepository;
        private readonly IRepository<PollAnswer> _pollAnswerRepository;
        private readonly IRepository<PollVotingRecord> _pollVotingRecordRepository;
        private readonly IRepository<StoreMapping> _storeMappingRepository;

        #endregion

        #region Ctor

        public PollService(CatalogSettings catalogSettings,
            IRepository<Poll> pollRepository,
            IRepository<PollAnswer> pollAnswerRepository,
            IRepository<PollVotingRecord> pollVotingRecordRepository,
             IRepository<StoreMapping> storeMappingRepository)
        {
            _catalogSettings = catalogSettings;
            _pollRepository = pollRepository;
            _pollAnswerRepository = pollAnswerRepository;
            _pollVotingRecordRepository = pollVotingRecordRepository;
            _storeMappingRepository = storeMappingRepository;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets a poll
        /// </summary>
        /// <param name="pollId">The poll identifier</param>
        /// <returns>Poll</returns>
        public virtual async Task<Poll> GetPollById(int pollId)
        {
            return await _pollRepository.GetById(pollId, cache => default);
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
        /// <returns>Polls</returns>
        public virtual async Task<IPagedList<Poll>> GetPolls(int storeId, int languageId = 0, bool showHidden = false,
            bool loadShownOnHomepageOnly = false, string systemKeyword = null,
            int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = _pollRepository.Table;

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

            //filter by language
            if (languageId > 0)
                query = query.Where(poll => poll.LanguageId == languageId);

            //filter by system keyword
            if (!string.IsNullOrEmpty(systemKeyword))
                query = query.Where(poll => poll.SystemKeyword == systemKeyword);

            //filter by store
            if (storeId > 0 && !_catalogSettings.IgnoreStoreLimitations)
            {
                query = from poll in query
                    join storeMapping in _storeMappingRepository.Table
                        on new
                        {
                            poll.Id,
                            Name = nameof(Poll)
                        }
                        equals new
                        {
                            Id = storeMapping.EntityId,
                            Name = storeMapping.EntityName
                        } 
                        into storeMappingsWithNulls
                    from storeMapping in storeMappingsWithNulls.DefaultIfEmpty()
                    where !poll.LimitedToStores || storeMapping.StoreId == storeId
                    select poll;
            }

            //order records by display order
            query = query.OrderBy(poll => poll.DisplayOrder).ThenBy(poll => poll.Id);

            //return paged list of polls
            return await query.ToPagedList(pageIndex, pageSize);
        }

        /// <summary>
        /// Deletes a poll
        /// </summary>
        /// <param name="poll">The poll</param>
        public virtual async Task DeletePoll(Poll poll)
        {
            await _pollRepository.Delete(poll);
        }

        /// <summary>
        /// Inserts a poll
        /// </summary>
        /// <param name="poll">Poll</param>
        public virtual async Task InsertPoll(Poll poll)
        {
            await _pollRepository.Insert(poll);
        }

        /// <summary>
        /// Updates the poll
        /// </summary>
        /// <param name="poll">Poll</param>
        public virtual async Task UpdatePoll(Poll poll)
        {
            await _pollRepository.Update(poll);
        }

        /// <summary>
        /// Gets a poll answer
        /// </summary>
        /// <param name="pollAnswerId">Poll answer identifier</param>
        /// <returns>Poll answer</returns>
        public virtual async Task<PollAnswer> GetPollAnswerById(int pollAnswerId)
        {
            return await _pollAnswerRepository.GetById(pollAnswerId, cache => default);
        }

        /// <summary>
        /// Deletes a poll answer
        /// </summary>
        /// <param name="pollAnswer">Poll answer</param>
        public virtual async Task DeletePollAnswer(PollAnswer pollAnswer)
        {
            await _pollAnswerRepository.Delete(pollAnswer);
        }

        /// <summary>
        /// Gets a poll answers by parent poll
        /// </summary>
        /// <param name="pollId">The poll identifier</param>
        /// <returns>Poll answer</returns>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        public virtual async Task<IPagedList<PollAnswer>> GetPollAnswerByPoll(int pollId, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = _pollAnswerRepository.Table.Where(pa => pa.PollId == pollId);

            //order records by display order
            query = query.OrderBy(pa => pa.DisplayOrder).ThenBy(pa => pa.Id);

            //return paged list of polls
            return await query.ToPagedList(pageIndex, pageSize);
        }

        /// <summary>
        /// Inserts a poll answer
        /// </summary>
        /// <param name="pollAnswer">Poll answer</param>
        public virtual async Task InsertPollAnswer(PollAnswer pollAnswer)
        {
            await _pollAnswerRepository.Insert(pollAnswer);
        }

        /// <summary>
        /// Updates the poll answer
        /// </summary>
        /// <param name="pollAnswer">Poll answer</param>
        public virtual async Task UpdatePollAnswer(PollAnswer pollAnswer)
        {
            await _pollAnswerRepository.Update(pollAnswer);
        }

        /// <summary>
        /// Gets a value indicating whether customer already voted for this poll
        /// </summary>
        /// <param name="pollId">Poll identifier</param>
        /// <param name="customerId">Customer identifier</param>
        /// <returns>Result</returns>
        public virtual async Task<bool> AlreadyVoted(int pollId, int customerId)
        {
            if (pollId == 0 || customerId == 0)
                return false;

            var result = await (from pa in _pollAnswerRepository.Table
                          join pvr in _pollVotingRecordRepository.Table on pa.Id equals pvr.PollAnswerId
                          where pa.PollId == pollId && pvr.CustomerId == customerId
                          select pvr).AnyAsync();
            return result;
        }

        /// <summary>
        /// Inserts a poll voting record
        /// </summary>
        /// <param name="pollVotingRecord">Voting record</param>
        public virtual async Task InsertPollVotingRecord(PollVotingRecord pollVotingRecord)
        {
            await _pollVotingRecordRepository.Insert(pollVotingRecord);
        }

        /// <summary>
        /// Gets a poll voting records by parent answer
        /// </summary>
        /// <param name="pollAnswerId">Poll answer identifier</param>
        /// <returns>Poll answer</returns>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        public virtual async Task<IPagedList<PollVotingRecord>> GetPollVotingRecordsByPollAnswer(int pollAnswerId, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = _pollVotingRecordRepository.Table.Where(pa => pa.PollAnswerId == pollAnswerId);

            //return paged list of poll voting records
            return await query.ToPagedList(pageIndex, pageSize);
        }

        #endregion
    }
}