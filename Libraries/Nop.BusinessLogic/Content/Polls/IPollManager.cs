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
using NopSolutions.NopCommerce.BusinessLogic.IoC;

namespace NopSolutions.NopCommerce.BusinessLogic.Content.Polls
{
    /// <summary>
    /// Poll manager interface
    /// </summary>
    public partial interface IPollManager
    {
        /// <summary>
        /// Gets a poll
        /// </summary>
        /// <param name="pollId">The poll identifier</param>
        /// <returns>Poll</returns>
        Poll GetPollById(int pollId);

        /// <summary>
        /// Gets a poll
        /// </summary>
        /// <param name="systemKeyword">The poll system keyword</param>
        /// <returns>Poll</returns>
        Poll GetPollBySystemKeyword(string systemKeyword);

        /// <summary>
        /// Gets all polls
        /// </summary>
        /// <param name="languageId">Language identifier. 0 if you want to get all news</param>
        /// <returns>Poll collection</returns>
        List<Poll> GetAllPolls(int languageId);

        /// <summary>
        /// Gets poll collection
        /// </summary>
        /// <param name="languageId">Language identifier. 0 if you want to get all polls</param>
        /// <param name="pollCount">Poll count to load. 0 if you want to get all polls</param>
        /// <returns>Poll collection</returns>
        List<Poll> GetPolls(int languageId, int pollCount);

        /// <summary>
        /// Gets poll collection
        /// </summary>
        /// <param name="languageId">Language identifier. 0 if you want to get all polls</param>
        /// <param name="pollCount">Poll count to load. 0 if you want to get all polls</param>
        /// <param name="loadShownOnHomePageOnly">Retrieve only shown on home page polls</param>
        /// <returns>Poll collection</returns>
        List<Poll> GetPolls(int languageId, int pollCount, bool loadShownOnHomePageOnly);

        /// <summary>
        /// Deletes a poll
        /// </summary>
        /// <param name="pollId">The poll identifier</param>
        void DeletePoll(int pollId);
        
        /// <summary>
        /// Inserts a poll
        /// </summary>
        /// <param name="poll">Poll</param>
        void InsertPoll(Poll poll);

        /// <summary>
        /// Updates the poll
        /// </summary>
        /// <param name="poll">Poll</param>
        void UpdatePoll(Poll poll);

        /// <summary>
        /// Is voting record already exists
        /// </summary>
        /// <param name="pollId">Poll identifier</param>
        /// <param name="customerId">Customer identifier</param>
        /// <returns>Poll</returns>
        bool PollVotingRecordExists(int pollId, int customerId);

        /// <summary>
        /// Gets a poll answer
        /// </summary>
        /// <param name="pollAnswerId">Poll answer identifier</param>
        /// <returns>Poll answer</returns>
        PollAnswer GetPollAnswerById(int pollAnswerId);

        /// <summary>
        /// Gets a poll answers by poll identifier
        /// </summary>
        /// <param name="pollId">Poll identifier</param>
        /// <returns>Poll answer collection</returns>
        List<PollAnswer> GetPollAnswersByPollId(int pollId);

        /// <summary>
        /// Deletes a poll answer
        /// </summary>
        /// <param name="pollAnswerId">Poll answer identifier</param>
        void DeletePollAnswer(int pollAnswerId);

        /// <summary>
        /// Inserts a poll answer
        /// </summary>
        /// <param name="pollAnswer">Poll answer</param>
        void InsertPollAnswer(PollAnswer pollAnswer);

        /// <summary>
        /// Updates the poll answer
        /// </summary>
        /// <param name="pollAnswer">Poll answer</param>
        void UpdatePollAnswer(PollAnswer pollAnswer);

        /// <summary>
        /// Creates a poll voting record
        /// </summary>
        /// <param name="pollAnswerId">The poll answer identifier</param>
        /// <param name="customerId">Customer identifer</param>
        void CreatePollVotingRecord(int pollAnswerId, int customerId);
    }
}
