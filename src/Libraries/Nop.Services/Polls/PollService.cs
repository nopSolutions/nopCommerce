using System;
using System.Linq;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Polls;
using Nop.Core.Domain.Stores;
using Nop.Data;
using Nop.Services.Caching.Extensions;
using Nop.Services.Events;

namespace Nop.Services.Polls
{
    /// <summary>
    /// Poll service
    /// </summary>
    public partial class PollService : IPollService
    {
        #region Fields

        private readonly CatalogSettings _catalogSettings;
        private readonly IEventPublisher _eventPublisher;
        private readonly IRepository<Poll> _pollRepository;
        private readonly IRepository<PollAnswer> _pollAnswerRepository;
        private readonly IRepository<PollVotingRecord> _pollVotingRecordRepository;
        private readonly IRepository<StoreMapping> _storeMappingRepository;

        #endregion

        #region Ctor

        public PollService(CatalogSettings catalogSettings,
            IEventPublisher eventPublisher,
            IRepository<Poll> pollRepository,
            IRepository<PollAnswer> pollAnswerRepository,
            IRepository<PollVotingRecord> pollVotingRecordRepository,
             IRepository<StoreMapping> storeMappingRepository)
        {
            _catalogSettings = catalogSettings;
            _eventPublisher = eventPublisher;
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
        public virtual Poll GetPollById(int pollId)
        {
            if (pollId == 0)
                return null;

            return _pollRepository.ToCachedGetById(pollId);
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
        public virtual IPagedList<Poll> GetPolls(int storeId, int languageId = 0, bool showHidden = false,
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
            return new PagedList<Poll>(query, pageIndex, pageSize);
        }

        /// <summary>
        /// Deletes a poll
        /// </summary>
        /// <param name="poll">The poll</param>
        public virtual void DeletePoll(Poll poll)
        {
            if (poll == null)
                throw new ArgumentNullException(nameof(poll));

            _pollRepository.Delete(poll);

            //event notification
            _eventPublisher.EntityDeleted(poll);
        }

        /// <summary>
        /// Inserts a poll
        /// </summary>
        /// <param name="poll">Poll</param>
        public virtual void InsertPoll(Poll poll)
        {
            if (poll == null)
                throw new ArgumentNullException(nameof(poll));

            _pollRepository.Insert(poll);

            //event notification
            _eventPublisher.EntityInserted(poll);
        }

        /// <summary>
        /// Updates the poll
        /// </summary>
        /// <param name="poll">Poll</param>
        public virtual void UpdatePoll(Poll poll)
        {
            if (poll == null)
                throw new ArgumentNullException(nameof(poll));

            _pollRepository.Update(poll);

            //event notification
            _eventPublisher.EntityUpdated(poll);
        }

        /// <summary>
        /// Gets a poll answer
        /// </summary>
        /// <param name="pollAnswerId">Poll answer identifier</param>
        /// <returns>Poll answer</returns>
        public virtual PollAnswer GetPollAnswerById(int pollAnswerId)
        {
            if (pollAnswerId == 0)
                return null;

            return _pollAnswerRepository.ToCachedGetById(pollAnswerId);
        }

        /// <summary>
        /// Deletes a poll answer
        /// </summary>
        /// <param name="pollAnswer">Poll answer</param>
        public virtual void DeletePollAnswer(PollAnswer pollAnswer)
        {
            if (pollAnswer == null)
                throw new ArgumentNullException(nameof(pollAnswer));

            _pollAnswerRepository.Delete(pollAnswer);

            //event notification
            _eventPublisher.EntityDeleted(pollAnswer);
        }

        /// <summary>
        /// Gets a poll answers by parent poll
        /// </summary>
        /// <param name="pollId">The poll identifier</param>
        /// <returns>Poll answer</returns>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        public virtual IPagedList<PollAnswer> GetPollAnswerByPoll(int pollId, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = _pollAnswerRepository.Table.Where(pa => pa.PollId == pollId);

            //order records by display order
            query = query.OrderBy(pa => pa.DisplayOrder).ThenBy(pa => pa.Id);

            //return paged list of polls
            return new PagedList<PollAnswer>(query, pageIndex, pageSize);
        }

        /// <summary>
        /// Inserts a poll answer
        /// </summary>
        /// <param name="pollAnswer">Poll answer</param>
        public virtual void InsertPollAnswer(PollAnswer pollAnswer)
        {
            if (pollAnswer == null)
                throw new ArgumentNullException(nameof(pollAnswer));

            _pollAnswerRepository.Insert(pollAnswer);

            //event notification
            _eventPublisher.EntityInserted(pollAnswer);
        }

        /// <summary>
        /// Updates the poll answer
        /// </summary>
        /// <param name="pollAnswer">Poll answer</param>
        public virtual void UpdatePollAnswer(PollAnswer pollAnswer)
        {
            if (pollAnswer == null)
                throw new ArgumentNullException(nameof(pollAnswer));

            _pollAnswerRepository.Update(pollAnswer);

            //event notification
            _eventPublisher.EntityUpdated(pollAnswer);
        }

        /// <summary>
        /// Gets a value indicating whether customer already voted for this poll
        /// </summary>
        /// <param name="pollId">Poll identifier</param>
        /// <param name="customerId">Customer identifier</param>
        /// <returns>Result</returns>
        public virtual bool AlreadyVoted(int pollId, int customerId)
        {
            if (pollId == 0 || customerId == 0)
                return false;

            var result = (from pa in _pollAnswerRepository.Table
                          join pvr in _pollVotingRecordRepository.Table on pa.Id equals pvr.PollAnswerId
                          where pa.PollId == pollId && pvr.CustomerId == customerId
                          select pvr).Any();
            return result;
        }

        /// <summary>
        /// Inserts a poll voting record
        /// </summary>
        /// <param name="pollVotingRecord">Voting record</param>
        public virtual void InsertPollVotingRecord(PollVotingRecord pollVotingRecord)
        {
            if (pollVotingRecord == null)
                throw new ArgumentNullException(nameof(pollVotingRecord));

            _pollVotingRecordRepository.Insert(pollVotingRecord);

            //event notification
            _eventPublisher.EntityInserted(pollVotingRecord);
        }

        /// <summary>
        /// Gets a poll voting records by parent answer
        /// </summary>
        /// <param name="pollAnswerId">Poll answer identifier</param>
        /// <returns>Poll answer</returns>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        public virtual IPagedList<PollVotingRecord> GetPollVotingRecordsByPollAnswer(int pollAnswerId, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = _pollVotingRecordRepository.Table.Where(pa => pa.PollAnswerId == pollAnswerId);

            //return paged list of poll voting records
            return new PagedList<PollVotingRecord>(query, pageIndex, pageSize);
        }

        #endregion
    }
}