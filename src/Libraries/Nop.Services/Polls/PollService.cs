using System;
using System.Linq;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Polls;
using Nop.Core.Events;

namespace Nop.Services.Polls
{
    /// <summary>
    /// Poll service
    /// </summary>
    public partial class PollService : IPollService
    {
        #region Constants
        private const string POLLS_BY_ID_KEY = "Nop.polls.id-{0}";
        private const string POLLS_PATTERN_KEY = "Nop.polls.";
        #endregion

        #region Fields

        private readonly IRepository<Poll> _pollRepository;
        private readonly IRepository<PollAnswer> _pollAnswerRepository;
        private readonly ICacheManager _cacheManager;
        private readonly IEventPublisher _eventPublisher;

        #endregion

        #region Ctor

        public PollService(IRepository<Poll> pollRepository, 
            IRepository<PollAnswer> pollAnswerRepository,
            ICacheManager cacheManager, IEventPublisher eventPublisher)
        {
            _pollRepository = pollRepository;
            _pollAnswerRepository = pollAnswerRepository;
            _cacheManager = cacheManager;
            _eventPublisher = eventPublisher;
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

            string key = string.Format(POLLS_BY_ID_KEY, pollId);
            return _cacheManager.Get(key, () =>
            {
                var poll = _pollRepository.GetById(pollId);
                return poll;
            });
        }

        /// <summary>
        /// Gets a poll
        /// </summary>
        /// <param name="systemKeyword">The poll system keyword</param>
        /// <returns>Poll</returns>
        public virtual Poll GetPollBySystemKeyword(string systemKeyword)
        {
            if (String.IsNullOrWhiteSpace(systemKeyword))
                return null;

            var query = from p in _pollRepository.Table
                        where p.SystemKeyword == systemKeyword
                        select p;
            var poll = query.FirstOrDefault();
            return poll;
        }
        
        /// <summary>
        /// Gets poll collection
        /// </summary>
        /// <param name="languageId">Language identifier. 0 if you want to get all polls</param>
        /// <param name="loadShownOnHomePageOnly">Retrieve only shown on home page polls</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Poll collection</returns>
        public virtual IPagedList<Poll> GetPolls(int languageId, bool loadShownOnHomePageOnly,
            int pageIndex, int pageSize, bool showHidden = false)
        {
            var query = _pollRepository.Table;
            if (!showHidden)
            {
                var utcNow = DateTime.UtcNow;
                query = query.Where(p => p.Published);
                query = query.Where(p => !p.StartDateUtc.HasValue || p.StartDateUtc <= utcNow);
                query = query.Where(p => !p.EndDateUtc.HasValue || p.EndDateUtc >= utcNow);
            }
            if (loadShownOnHomePageOnly)
            {
                query = query.Where(p => p.ShowOnHomePage);
            }
            if (languageId > 0)
            {
                query = query.Where(p => p.LanguageId == languageId);
            }
            query = query.OrderBy(p => p.DisplayOrder);

            var polls = new PagedList<Poll>(query, pageIndex, pageSize);
            return polls;
        }

        /// <summary>
        /// Deletes a poll
        /// </summary>
        /// <param name="poll">The poll</param>
        public virtual void DeletePoll(Poll poll)
        {
            if (poll == null)
                throw new ArgumentNullException("poll");

            _pollRepository.Delete(poll);

            _cacheManager.RemoveByPattern(POLLS_PATTERN_KEY);

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
                throw new ArgumentNullException("poll");

            _pollRepository.Insert(poll);

            _cacheManager.RemoveByPattern(POLLS_PATTERN_KEY);

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
                throw new ArgumentNullException("poll");

            _pollRepository.Update(poll);

            _cacheManager.RemoveByPattern(POLLS_PATTERN_KEY);

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

            var query = from pa in _pollAnswerRepository.Table
                        where pa.Id == pollAnswerId
                        select pa;
            var pollAnswer = query.SingleOrDefault();
            return pollAnswer;
        }
        
        /// <summary>
        /// Deletes a poll answer
        /// </summary>
        /// <param name="pollAnswer">Poll answer</param>
        public virtual void DeletePollAnswer(PollAnswer pollAnswer)
        {
            if (pollAnswer == null)
                throw new ArgumentNullException("pollAnswer");

            _pollAnswerRepository.Delete(pollAnswer);

            _cacheManager.RemoveByPattern(POLLS_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityDeleted(pollAnswer);
        }

        #endregion
    }
}
