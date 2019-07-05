using System.Collections.Generic;
using Nop.Core.Domain.Polls;
using Nop.Web.Models.Polls;

namespace Nop.Web.Factories
{
    /// <summary>
    /// Represents the interface of the poll model factory
    /// </summary>
    public partial interface IPollModelFactory
    {
        /// <summary>
        /// Prepare the poll model
        /// </summary>
        /// <param name="poll">Poll</param>
        /// <param name="setAlreadyVotedProperty">Whether to load a value indicating that customer already voted for this poll</param>
        /// <returns>Poll model</returns>
        PollModel PreparePollModel(Poll poll, bool setAlreadyVotedProperty);

        /// <summary>
        /// Get the poll model by poll system keyword
        /// </summary>
        /// <param name="systemKeyword">Poll system keyword</param>
        /// <returns>Poll model</returns>
        PollModel PreparePollModelBySystemName(string systemKeyword);

        /// <summary>
        /// Prepare the home page poll models
        /// </summary>
        /// <returns>List of the poll model</returns>
        List<PollModel> PrepareHomepagePollModels();
    }
}
