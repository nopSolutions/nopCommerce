//------------------------------------------------------------------------------
// The contents of this file are subject to the nopCommerce Public License Version 1.0 ("License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at  http://www.nopCommerce.com/License.aspx. 
// 
// Software distributed under the License is distributed on an "AS IS" basis, WITHOUT WARRANTY OF ANY KIND, either express or implied. 
// See the License for the specific language governing rights and limitations under the License.
// 
// The Original Code is nopCommerce.
// The Initial Developer of the Original Code is NopSolutions.
// All Rights Reserved.
// 
// Contributor(s): _______. 
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Text;
using NopSolutions.NopCommerce.BusinessLogic.Caching;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.Data;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.BusinessLogic.Infrastructure;

namespace NopSolutions.NopCommerce.BusinessLogic.Content.Polls
{
    /// <summary>
    /// Poll service
    /// </summary>
    public partial class PollService : IPollService
    {
        #region Constants
        private const string POLLS_BY_ID_KEY = "Nop.polls.id-{0}";
        private const string POLLANSWERS_BY_POLLID_KEY = "Nop.pollanswers.pollid-{0}";
        private const string POLLS_PATTERN_KEY = "Nop.polls.";
        private const string POLLANSWERS_PATTERN_KEY = "Nop.pollanswers.";
        #endregion

        #region Fields

        /// <summary>
        /// Object context
        /// </summary>
        private readonly NopObjectContext _context;

        /// <summary>
        /// Cache manager
        /// </summary>
        private readonly ICacheManager _cacheManager;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="context">Object context</param>
        public PollService(NopObjectContext context)
        {
            this._context = context;
            this._cacheManager = new NopRequestCache();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets a poll
        /// </summary>
        /// <param name="pollId">The poll identifier</param>
        /// <returns>Poll</returns>
        public Poll GetPollById(int pollId)
        {
            if (pollId <= 0)
                return null;

            string key = string.Format(POLLS_BY_ID_KEY, pollId);
            object obj2 = _cacheManager.Get(key);
            if (this.CacheEnabled && (obj2 != null))
            {
                return (Poll)obj2;
            }

            
            var query = from p in _context.Polls
                        where p.PollId == pollId
                        select p;
            var poll = query.SingleOrDefault();

            if (this.CacheEnabled)
            {
                _cacheManager.Add(key, poll);
            }
            return poll;
        }

        /// <summary>
        /// Gets a poll
        /// </summary>
        /// <param name="systemKeyword">The poll system keyword</param>
        /// <returns>Poll</returns>
        public Poll GetPollBySystemKeyword(string systemKeyword)
        {
            if (String.IsNullOrWhiteSpace(systemKeyword))
                return null;
                        
            var query = from p in _context.Polls
                        where p.SystemKeyword == systemKeyword
                        select p;
            var poll = query.FirstOrDefault();

            return poll;
        }

        /// <summary>
        /// Gets all polls
        /// </summary>
        /// <param name="languageId">Language identifier. 0 if you want to get all news</param>
        /// <returns>Poll collection</returns>
        public List<Poll> GetAllPolls(int languageId)
        {
            return GetPolls(languageId, 0);
        }

        /// <summary>
        /// Gets poll collection
        /// </summary>
        /// <param name="languageId">Language identifier. 0 if you want to get all polls</param>
        /// <param name="pollCount">Poll count to load. 0 if you want to get all polls</param>
        /// <returns>Poll collection</returns>
        public List<Poll> GetPolls(int languageId, int pollCount)
        {
            return GetPolls(languageId, pollCount, false);
        }

        /// <summary>
        /// Gets poll collection
        /// </summary>
        /// <param name="languageId">Language identifier. 0 if you want to get all polls</param>
        /// <param name="pollCount">Poll count to load. 0 if you want to get all polls</param>
        /// <param name="loadShownOnHomePageOnly">Retrieve only shown on home page polls</param>
        /// <returns>Poll collection</returns>
        public List<Poll> GetPolls(int languageId, int pollCount, bool loadShownOnHomePageOnly)
        {
            bool showHidden = NopContext.Current.IsAdmin;

            
            var query = (IQueryable<Poll>)_context.Polls;
            if (!showHidden)
            {
                query = query.Where(p => p.Published);
                query = query.Where(p => !p.StartDate.HasValue || p.StartDate <= DateTime.UtcNow);
                query = query.Where(p => !p.EndDate.HasValue || p.EndDate >= DateTime.UtcNow);
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
            if (pollCount > 0)
            {
                query = query.Take(pollCount);
            }

            var polls = query.ToList();

            return polls;
        }

        /// <summary>
        /// Deletes a poll
        /// </summary>
        /// <param name="pollId">The poll identifier</param>
        public void DeletePoll(int pollId)
        {
            var poll = GetPollById(pollId);
            if (poll == null)
                return;

            
            if (!_context.IsAttached(poll))
                _context.Polls.Attach(poll);
            _context.DeleteObject(poll);
            _context.SaveChanges();
            
            if (this.CacheEnabled)
            {
                _cacheManager.RemoveByPattern(POLLS_PATTERN_KEY);
                _cacheManager.RemoveByPattern(POLLANSWERS_PATTERN_KEY);
            }
        }
        
        /// <summary>
        /// Inserts a poll
        /// </summary>
        /// <param name="poll">Poll</param>
        public void InsertPoll(Poll poll)
        {
            if (poll == null)
                throw new ArgumentNullException("poll");

            poll.Name = CommonHelper.EnsureNotNull(poll.Name);
            poll.Name = CommonHelper.EnsureMaximumLength(poll.Name, 400);
            poll.SystemKeyword = CommonHelper.EnsureNotNull(poll.SystemKeyword);
            poll.SystemKeyword = CommonHelper.EnsureMaximumLength(poll.SystemKeyword, 400);
            poll.SystemKeyword = poll.SystemKeyword.Trim();

            
            
            _context.Polls.AddObject(poll);
            _context.SaveChanges();

            if (this.CacheEnabled)
            {
                _cacheManager.RemoveByPattern(POLLS_PATTERN_KEY);
                _cacheManager.RemoveByPattern(POLLANSWERS_PATTERN_KEY);
            }
        }

        /// <summary>
        /// Updates the poll
        /// </summary>
        /// <param name="poll">Poll</param>
        public void UpdatePoll(Poll poll)
        {
            if (poll == null)
                throw new ArgumentNullException("poll");

            poll.Name = CommonHelper.EnsureNotNull(poll.Name);
            poll.Name = CommonHelper.EnsureMaximumLength(poll.Name, 400);
            poll.SystemKeyword = CommonHelper.EnsureNotNull(poll.SystemKeyword);
            poll.SystemKeyword = CommonHelper.EnsureMaximumLength(poll.SystemKeyword, 400);
            poll.SystemKeyword = poll.SystemKeyword.Trim();

            
            if (!_context.IsAttached(poll))
                _context.Polls.Attach(poll);

            _context.SaveChanges();

            if (this.CacheEnabled)
            {
                _cacheManager.RemoveByPattern(POLLS_PATTERN_KEY);
                _cacheManager.RemoveByPattern(POLLANSWERS_PATTERN_KEY);
            }
        }

        /// <summary>
        /// Is voting record already exists
        /// </summary>
        /// <param name="pollId">Poll identifier</param>
        /// <param name="customerId">Customer identifier</param>
        /// <returns>Poll</returns>
        public bool PollVotingRecordExists(int pollId, int customerId)
        {
            
            var query = from pvr in _context.PollVotingRecords
                        join pa in _context.PollAnswers on pvr.PollAnswerId equals pa.PollAnswerId
                        where pa.PollId == pollId &&
                        pvr.CustomerId == customerId
                        select pvr;
            int count = query.Count();
            return count > 0;
        }

        /// <summary>
        /// Gets a poll answer
        /// </summary>
        /// <param name="pollAnswerId">Poll answer identifier</param>
        /// <returns>Poll answer</returns>
        public PollAnswer GetPollAnswerById(int pollAnswerId)
        {
            if (pollAnswerId == 0)
                return null;

            
            var query = from pa in _context.PollAnswers
                        where pa.PollAnswerId == pollAnswerId
                        select pa;
            var pollAnswer = query.SingleOrDefault();
            return pollAnswer;
        }

        /// <summary>
        /// Gets a poll answers by poll identifier
        /// </summary>
        /// <param name="pollId">Poll identifier</param>
        /// <returns>Poll answer collection</returns>
        public List<PollAnswer> GetPollAnswersByPollId(int pollId)
        {
            string key = string.Format(POLLANSWERS_BY_POLLID_KEY, pollId);
            object obj2 = _cacheManager.Get(key);
            if (this.CacheEnabled && (obj2 != null))
            {
                return (List<PollAnswer>)obj2;
            }

            
            var query = from pa in _context.PollAnswers
                        orderby pa.DisplayOrder
                        where pa.PollId == pollId
                        select pa;
            var pollAnswers = query.ToList();

            if (this.CacheEnabled)
            {
                _cacheManager.Add(key, pollAnswers);
            }
            return pollAnswers;
        }

        /// <summary>
        /// Deletes a poll answer
        /// </summary>
        /// <param name="pollAnswerId">Poll answer identifier</param>
        public void DeletePollAnswer(int pollAnswerId)
        {
            var pollAnswer = GetPollAnswerById(pollAnswerId);
            if (pollAnswer == null)
                return;

            
            if (!_context.IsAttached(pollAnswer))
                _context.PollAnswers.Attach(pollAnswer);
            _context.DeleteObject(pollAnswer);
            _context.SaveChanges();

            if (this.CacheEnabled)
            {
                _cacheManager.RemoveByPattern(POLLS_PATTERN_KEY);
                _cacheManager.RemoveByPattern(POLLANSWERS_PATTERN_KEY);
            }
        }

        /// <summary>
        /// Inserts a poll answer
        /// </summary>
        /// <param name="pollAnswer">Poll answer</param>
        public void InsertPollAnswer(PollAnswer pollAnswer)
        {
            if (pollAnswer == null)
                throw new ArgumentNullException("pollAnswer");

            pollAnswer.Name = CommonHelper.EnsureNotNull(pollAnswer.Name);
            pollAnswer.Name = CommonHelper.EnsureMaximumLength(pollAnswer.Name, 400);

            
           
            _context.PollAnswers.AddObject(pollAnswer);
            _context.SaveChanges();

            if (this.CacheEnabled)
            {
                _cacheManager.RemoveByPattern(POLLS_PATTERN_KEY);
                _cacheManager.RemoveByPattern(POLLANSWERS_PATTERN_KEY);
            }
        }

        /// <summary>
        /// Updates the poll answer
        /// </summary>
        /// <param name="pollAnswer">Poll answer</param>
        public void UpdatePollAnswer(PollAnswer pollAnswer)
        {
            if (pollAnswer == null)
                throw new ArgumentNullException("pollAnswer");

            pollAnswer.Name = CommonHelper.EnsureNotNull(pollAnswer.Name);
            pollAnswer.Name = CommonHelper.EnsureMaximumLength(pollAnswer.Name, 400);

            
            if (!_context.IsAttached(pollAnswer))
                _context.PollAnswers.Attach(pollAnswer);

            _context.SaveChanges();

            if (this.CacheEnabled)
            {
                _cacheManager.RemoveByPattern(POLLS_PATTERN_KEY);
                _cacheManager.RemoveByPattern(POLLANSWERS_PATTERN_KEY);
            }
        }

        /// <summary>
        /// Creates a poll voting record
        /// </summary>
        /// <param name="pollAnswerId">The poll answer identifier</param>
        /// <param name="customerId">Customer identifer</param>
        public void CreatePollVotingRecord(int pollAnswerId, int customerId)
        {
            var pollAnswer = GetPollAnswerById(pollAnswerId);
            if (pollAnswer == null)
                return;

            //delete previous vote
            
            var oldPvr = (from pvr in _context.PollVotingRecords
                          where pvr.PollAnswerId == pollAnswerId &&
                          pvr.CustomerId == customerId
                          select pvr).FirstOrDefault();
            if (oldPvr != null)
            {
                _context.DeleteObject(oldPvr);
            }
            _context.SaveChanges();

            //insert new vote
            var newPvr = _context.PollVotingRecords.CreateObject();
            newPvr.PollAnswerId = pollAnswerId;
            newPvr.CustomerId = customerId;

            _context.PollVotingRecords.AddObject(newPvr);
            _context.SaveChanges();

            //new vote records
            int totalVotingRecords = (from pvr in _context.PollVotingRecords
                                      where pvr.PollAnswerId == pollAnswerId
                                      select pvr).Count();

            pollAnswer.Count = totalVotingRecords;
            UpdatePollAnswer(pollAnswer);

            if (this.CacheEnabled)
            {
                _cacheManager.RemoveByPattern(POLLS_PATTERN_KEY);
                _cacheManager.RemoveByPattern(POLLANSWERS_PATTERN_KEY);
            }
        }

        #endregion

        #region Properties
        /// <summary>
        /// Gets a value indicating whether cache is enabled
        /// </summary>
        public bool CacheEnabled
        {
            get
            {
                return IoC.Resolve<ISettingManager>().GetSettingValueBoolean("Cache.PollManager.CacheEnabled");
            }
        }
        #endregion
    }
}
