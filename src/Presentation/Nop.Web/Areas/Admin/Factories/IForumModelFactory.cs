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
        /// <returns>Forum group search model</returns>
        ForumGroupSearchModel PrepareForumGroupSearchModel(ForumGroupSearchModel searchModel);

        /// <summary>
        /// Prepare paged forum group list model
        /// </summary>
        /// <param name="searchModel">Forum group search model</param>
        /// <returns>Forum group list model</returns>
        ForumGroupListModel PrepareForumGroupListModel(ForumGroupSearchModel searchModel);

        /// <summary>
        /// Prepare forum group model
        /// </summary>
        /// <param name="model">Forum group model</param>
        /// <param name="forumGroup">Forum group</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>Forum group model</returns>
        ForumGroupModel PrepareForumGroupModel(ForumGroupModel model, ForumGroup forumGroup, bool excludeProperties = false);

        /// <summary>
        /// Prepare paged forum list model
        /// </summary>
        /// <param name="searchModel">Forum search model</param>
        /// <param name="forumGroup">Forum group</param>
        /// <returns>Forum list model</returns>
        ForumListModel PrepareForumListModel(ForumSearchModel searchModel, ForumGroup forumGroup);

        /// <summary>
        /// Prepare forum model
        /// </summary>
        /// <param name="model">Forum model</param>
        /// <param name="forum">Forum</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>Forum model</returns>
        ForumModel PrepareForumModel(ForumModel model, Forum forum, bool excludeProperties = false);
    }
}