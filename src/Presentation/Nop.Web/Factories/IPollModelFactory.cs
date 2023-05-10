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
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the poll model
        /// </returns>
        Task<PollModel> PreparePollModelAsync(Poll poll, bool setAlreadyVotedProperty);

        /// <summary>
        /// Get the poll model by poll system keyword
        /// </summary>
        /// <param name="systemKeyword">Poll system keyword</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the poll model
        /// </returns>
        Task<PollModel> PreparePollModelBySystemNameAsync(string systemKeyword);

        /// <summary>
        /// Prepare the home page poll models
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the list of the poll model
        /// </returns>
        Task<List<PollModel>> PrepareHomepagePollModelsAsync();
    }
}
