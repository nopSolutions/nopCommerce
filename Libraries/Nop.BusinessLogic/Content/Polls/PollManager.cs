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

namespace NopSolutions.NopCommerce.BusinessLogic.Content.Polls
{
    /// <summary>
    /// Poll manager
    /// </summary>
    public partial class PollManager
    {
        #region Constants
        private const string POLLS_BY_ID_KEY = "Nop.polls.id-{0}";
        private const string POLLANSWERS_BY_POLLID_KEY = "Nop.pollanswers.pollid-{0}";
        private const string POLLS_PATTERN_KEY = "Nop.polls.";
        private const string POLLANSWERS_PATTERN_KEY = "Nop.pollanswers.";
        #endregion
        
        #region Methods

        /// <summary>
        /// Gets a poll
        /// </summary>
        /// <param name="pollId">The poll identifier</param>
        /// <returns>Poll</returns>
        public static Poll GetPollById(int pollId)
        {
            if (pollId == 0)
                return null;

            string key = string.Format(POLLS_BY_ID_KEY, pollId);
            object obj2 = NopRequestCache.Get(key);
            if (PollManager.CacheEnabled && (obj2 != null))
            {
                return (Poll)obj2;
            }

            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from p in context.Polls
                        where p.PollId == pollId
                        select p;
            var poll = query.SingleOrDefault();

            if (PollManager.CacheEnabled)
            {
                NopRequestCache.Add(key, poll);
            }
            return poll;
        }

        /// <summary>
        /// Gets a poll
        /// </summary>
        /// <param name="systemKeyword">The poll system keyword</param>
        /// <returns>Poll</returns>
        public static Poll GetPollBySystemKeyword(string systemKeyword)
        {
            if (String.IsNullOrWhiteSpace(systemKeyword))
                return null;

            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from p in context.Polls
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
        public static List<Poll> GetAllPolls(int languageId)
        {
            return GetPolls(languageId, 0);
        }

        /// <summary>
        /// Gets poll collection
        /// </summary>
        /// <param name="languageId">Language identifier. 0 if you want to get all polls</param>
        /// <param name="pollCount">Poll count to load. 0 if you want to get all polls</param>
        /// <returns>Poll collection</returns>
        public static List<Poll> GetPolls(int languageId, int pollCount)
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
        public static List<Poll> GetPolls(int languageId, int pollCount, bool loadShownOnHomePageOnly)
        {
            bool showHidden = NopContext.Current.IsAdmin;

            var context = ObjectContextHelper.CurrentObjectContext;
            var query = (IQueryable<Poll>)context.Polls;
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
        public static void DeletePoll(int pollId)
        {
            var poll = GetPollById(pollId);
            if (poll == null)
                return;

            var context = ObjectContextHelper.CurrentObjectContext;
            if (!context.IsAttached(poll))
                context.Polls.Attach(poll);
            context.DeleteObject(poll);
            context.SaveChanges();
            
            if (PollManager.CacheEnabled)
            {
                NopRequestCache.RemoveByPattern(POLLS_PATTERN_KEY);
                NopRequestCache.RemoveByPattern(POLLANSWERS_PATTERN_KEY);
            }
        }
        
        /// <summary>
        /// Inserts a poll
        /// </summary>
        /// <param name="languageId">The language identifier</param>
        /// <param name="name">The name</param>
        /// <param name="systemKeyword">The system keyword</param>
        /// <param name="published">A value indicating whether the entity is published</param>
        /// <param name="showOnHomePage">A value indicating whether the entity should be shown on home page</param>
        /// <param name="displayOrder">The display order</param>
        /// <param name="startDate">The poll start date and time</param>
        /// <param name="endDate">The poll end date and time</param>
        /// <returns>Poll</returns>
        public static Poll InsertPoll(int languageId, string name, string systemKeyword,
            bool published, bool showOnHomePage, int displayOrder, 
            DateTime? startDate, DateTime? endDate)
        {
            name = CommonHelper.EnsureMaximumLength(name, 400);
            systemKeyword = CommonHelper.EnsureMaximumLength(systemKeyword, 400);
            systemKeyword = systemKeyword.Trim();

            var context = ObjectContextHelper.CurrentObjectContext;

            var poll = context.Polls.CreateObject();
            poll.LanguageId = languageId;
            poll.Name = name;
            poll.SystemKeyword = systemKeyword;
            poll.Published = published;
            poll.ShowOnHomePage = showOnHomePage;
            poll.DisplayOrder = displayOrder;
            poll.StartDate = startDate;
            poll.EndDate = endDate;

            context.Polls.AddObject(poll);
            context.SaveChanges();

            if (PollManager.CacheEnabled)
            {
                NopRequestCache.RemoveByPattern(POLLS_PATTERN_KEY);
                NopRequestCache.RemoveByPattern(POLLANSWERS_PATTERN_KEY);
            }

            return poll;
        }

        /// <summary>
        /// Updates the poll
        /// </summary>
        /// <param name="pollId">The poll identifier</param>
        /// <param name="languageId">The language identifier</param>
        /// <param name="name">The name</param>
        /// <param name="systemKeyword">The system keyword</param>
        /// <param name="published">A value indicating whether the entity is published</param>
        /// <param name="showOnHomePage">A value indicating whether the entity should be shown on home page</param>
        /// <param name="displayOrder">The display order</param>
        /// <param name="startDate">The poll start date and time</param>
        /// <param name="endDate">The poll end date and time</param>
        /// <returns>Poll</returns>
        public static Poll UpdatePoll(int pollId, int languageId, string name,
            string systemKeyword, bool published, bool showOnHomePage, int displayOrder, 
            DateTime? startDate, DateTime? endDate)
        {
            name = CommonHelper.EnsureMaximumLength(name, 400);
            systemKeyword = CommonHelper.EnsureMaximumLength(systemKeyword, 400);
            systemKeyword = systemKeyword.Trim();

            var poll = GetPollById(pollId);
            if (poll == null)
                return null;

            var context = ObjectContextHelper.CurrentObjectContext;
            if (!context.IsAttached(poll))
                context.Polls.Attach(poll);

            poll.LanguageId = languageId;
            poll.Name = name;
            poll.SystemKeyword = systemKeyword;
            poll.Published = published;
            poll.ShowOnHomePage = showOnHomePage;
            poll.DisplayOrder = displayOrder;
            poll.StartDate = startDate;
            poll.EndDate = endDate;

            context.SaveChanges();

            if (PollManager.CacheEnabled)
            {
                NopRequestCache.RemoveByPattern(POLLS_PATTERN_KEY);
                NopRequestCache.RemoveByPattern(POLLANSWERS_PATTERN_KEY);
            }

            return poll;
        }

        /// <summary>
        /// Is voting record already exists
        /// </summary>
        /// <param name="pollId">Poll identifier</param>
        /// <param name="customerId">Customer identifier</param>
        /// <returns>Poll</returns>
        public static bool PollVotingRecordExists(int pollId, int customerId)
        {
            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from pvr in context.PollVotingRecords
                        join pa in context.PollAnswers on pvr.PollAnswerId equals pa.PollAnswerId
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
        public static PollAnswer GetPollAnswerById(int pollAnswerId)
        {
            if (pollAnswerId == 0)
                return null;

            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from pa in context.PollAnswers
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
        public static List<PollAnswer> GetPollAnswersByPollId(int pollId)
        {
            string key = string.Format(POLLANSWERS_BY_POLLID_KEY, pollId);
            object obj2 = NopRequestCache.Get(key);
            if (PollManager.CacheEnabled && (obj2 != null))
            {
                return (List<PollAnswer>)obj2;
            }

            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from pa in context.PollAnswers
                        orderby pa.DisplayOrder
                        where pa.PollId == pollId
                        select pa;
            var pollAnswers = query.ToList();

            if (PollManager.CacheEnabled)
            {
                NopRequestCache.Add(key, pollAnswers);
            }
            return pollAnswers;
        }

        /// <summary>
        /// Deletes a poll answer
        /// </summary>
        /// <param name="pollAnswerId">Poll answer identifier</param>
        public static void DeletePollAnswer(int pollAnswerId)
        {
            var pollAnswer = GetPollAnswerById(pollAnswerId);
            if (pollAnswer == null)
                return;

            var context = ObjectContextHelper.CurrentObjectContext;
            if (!context.IsAttached(pollAnswer))
                context.PollAnswers.Attach(pollAnswer);
            context.DeleteObject(pollAnswer);
            context.SaveChanges();

            if (PollManager.CacheEnabled)
            {
                NopRequestCache.RemoveByPattern(POLLS_PATTERN_KEY);
                NopRequestCache.RemoveByPattern(POLLANSWERS_PATTERN_KEY);
            }
        }

        /// <summary>
        /// Inserts a poll answer
        /// </summary>
        /// <param name="pollId">The poll identifier</param>
        /// <param name="name">The poll answer name</param>
        /// <param name="count">The current count</param>
        /// <param name="displayOrder">The display order</param>
        /// <returns>Poll answer</returns>
        public static PollAnswer InsertPollAnswer(int pollId,
            string name, int count, int displayOrder)
        {
            name = CommonHelper.EnsureMaximumLength(name, 400);

            var context = ObjectContextHelper.CurrentObjectContext;
            var pollAnswer = context.PollAnswers.CreateObject();
            pollAnswer.PollId = pollId;
            pollAnswer.Name = name;
            pollAnswer.Count = count;
            pollAnswer.DisplayOrder = displayOrder;

            context.PollAnswers.AddObject(pollAnswer);
            context.SaveChanges();

            if (PollManager.CacheEnabled)
            {
                NopRequestCache.RemoveByPattern(POLLS_PATTERN_KEY);
                NopRequestCache.RemoveByPattern(POLLANSWERS_PATTERN_KEY);
            }

            return pollAnswer;
        }

        /// <summary>
        /// Updates the poll answer
        /// </summary>
        /// <param name="pollAnswerId">The poll answer identifier</param>
        /// <param name="pollId">The poll identifier</param>
        /// <param name="name">The poll answer name</param>
        /// <param name="count">The current count</param>
        /// <param name="displayOrder">The display order</param>
        /// <returns>Poll answer</returns>
        public static PollAnswer UpdatePollAnswer(int pollAnswerId,
            int pollId, string name, int count, int displayOrder)
        {
            name = CommonHelper.EnsureMaximumLength(name, 400);

            var pollAnswer = GetPollAnswerById(pollAnswerId);
            if (pollAnswer == null)
                return null;

            var context = ObjectContextHelper.CurrentObjectContext;
            if (!context.IsAttached(pollAnswer))
                context.PollAnswers.Attach(pollAnswer);

            pollAnswer.PollId = pollId;
            pollAnswer.Name = name;
            pollAnswer.Count = count;
            pollAnswer.DisplayOrder = displayOrder;
            context.SaveChanges();

            if (PollManager.CacheEnabled)
            {
                NopRequestCache.RemoveByPattern(POLLS_PATTERN_KEY);
                NopRequestCache.RemoveByPattern(POLLANSWERS_PATTERN_KEY);
            }

            return pollAnswer;
        }

        /// <summary>
        /// Creates a poll voting record
        /// </summary>
        /// <param name="pollAnswerId">The poll answer identifier</param>
        /// <param name="customerId">Customer identifer</param>
        public static void CreatePollVotingRecord(int pollAnswerId, int customerId)
        {
            var pollAnswer = GetPollAnswerById(pollAnswerId);
            if (pollAnswer == null)
                return;

            //delete previous vote
            var context = ObjectContextHelper.CurrentObjectContext;
            var oldPvr = (from pvr in context.PollVotingRecords
                          where pvr.PollAnswerId == pollAnswerId &&
                          pvr.CustomerId == customerId
                          select pvr).FirstOrDefault();
            if (oldPvr != null)
            {
                context.DeleteObject(oldPvr);
            }
            context.SaveChanges();

            //insert new vote
            var newPvr = context.PollVotingRecords.CreateObject();
            newPvr.PollAnswerId = pollAnswerId;
            newPvr.CustomerId = customerId;

            context.PollVotingRecords.AddObject(newPvr);
            context.SaveChanges();

            //new vote records
            int totalVotingRecords = (from pvr in context.PollVotingRecords
                                      where pvr.PollAnswerId == pollAnswerId
                                      select pvr).Count();

            pollAnswer = UpdatePollAnswer(pollAnswer.PollAnswerId,
                pollAnswer.PollId,
                pollAnswer.Name,
                totalVotingRecords,
                pollAnswer.DisplayOrder);

            if (PollManager.CacheEnabled)
            {
                NopRequestCache.RemoveByPattern(POLLS_PATTERN_KEY);
                NopRequestCache.RemoveByPattern(POLLANSWERS_PATTERN_KEY);
            }
        }

        #endregion

        #region Properties
        /// <summary>
        /// Gets a value indicating whether cache is enabled
        /// </summary>
        public static bool CacheEnabled
        {
            get
            {
                return SettingManager.GetSettingValueBoolean("Cache.PollManager.CacheEnabled");
            }
        }
        #endregion
    }
}
