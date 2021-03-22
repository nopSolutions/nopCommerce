using System.Threading.Tasks;
using Nop.Core.Domain.Forums;
using Nop.Web.Areas.Admin.Models.Forums;

namespace Nop.Web.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the forum model factory
    /// </summary>
    public partial interface IForumModelFactory
    {
        /// <summary>
        /// Prepare forum group search model
        /// </summary>
        /// <param name="searchModel">Forum group search model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the forum group search model
        /// </returns>
        Task<ForumGroupSearchModel> PrepareForumGroupSearchModelAsync(ForumGroupSearchModel searchModel);

        /// <summary>
        /// Prepare paged forum group list model
        /// </summary>
        /// <param name="searchModel">Forum group search model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the forum group list model
        /// </returns>
        Task<ForumGroupListModel> PrepareForumGroupListModelAsync(ForumGroupSearchModel searchModel);

        /// <summary>
        /// Prepare forum group model
        /// </summary>
        /// <param name="model">Forum group model</param>
        /// <param name="forumGroup">Forum group</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the forum group model
        /// </returns>
        Task<ForumGroupModel> PrepareForumGroupModelAsync(ForumGroupModel model, ForumGroup forumGroup, bool excludeProperties = false);

        /// <summary>
        /// Prepare paged forum list model
        /// </summary>
        /// <param name="searchModel">Forum search model</param>
        /// <param name="forumGroup">Forum group</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the forum list model
        /// </returns>
        Task<ForumListModel> PrepareForumListModelAsync(ForumSearchModel searchModel, ForumGroup forumGroup);

        /// <summary>
        /// Prepare forum model
        /// </summary>
        /// <param name="model">Forum model</param>
        /// <param name="forum">Forum</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the forum model
        /// </returns>
        Task<ForumModel> PrepareForumModelAsync(ForumModel model, Forum forum, bool excludeProperties = false);
    }
}