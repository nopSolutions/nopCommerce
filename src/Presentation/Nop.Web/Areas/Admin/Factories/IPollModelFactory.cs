using Nop.Core.Domain.Polls;
using Nop.Web.Areas.Admin.Models.Polls;

namespace Nop.Web.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the poll model factory
    /// </summary>
    public partial interface IPollModelFactory
    {
        /// <summary>
        /// Prepare poll search model
        /// </summary>
        /// <param name="searchModel">Poll search model</param>
        /// <returns>Poll search model</returns>
        PollSearchModel PreparePollSearchModel(PollSearchModel searchModel);

        /// <summary>
        /// Prepare paged poll list model
        /// </summary>
        /// <param name="searchModel">Poll search model</param>
        /// <returns>Poll list model</returns>
        PollListModel PreparePollListModel(PollSearchModel searchModel);

        /// <summary>
        /// Prepare poll model
        /// </summary>
        /// <param name="model">Poll model</param>
        /// <param name="poll">Poll</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>Poll model</returns>
        PollModel PreparePollModel(PollModel model, Poll poll, bool excludeProperties = false);

        /// <summary>
        /// Prepare paged poll answer list model
        /// </summary>
        /// <param name="searchModel">Poll answer search model</param>
        /// <param name="poll">Poll</param>
        /// <returns>Poll answer list model</returns>
        PollAnswerListModel PreparePollAnswerListModel(PollAnswerSearchModel searchModel, Poll poll);
    }
}