using Nop.Core.Domain.Weixin;
using Nop.Web.Areas.Admin.Models.Weixin;

namespace Nop.Web.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the Weixin model factory
    /// </summary>
    public partial interface IWeixinModelFactory
    {
        /// <summary>
        /// Prepare User model
        /// </summary>
        /// <param name="model">User model</param>
        /// <param name="product">User</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>Product model</returns>
        UserModel PrepareUserModel(UserModel model, WUser user, bool excludeProperties = false);

        /// <summary>
        /// Prepare User search model
        /// </summary>
        /// <param name="searchModel">User search model</param>
        /// <returns>User search model</returns>
        UserSearchModel PrepareUserSearchModel(UserSearchModel searchModel);

        /// <summary>
        /// Prepare paged User list model
        /// </summary>
        /// <param name="searchModel">User search model</param>
        /// <returns>User list model</returns>
        UserListModel PrepareUserListModel(UserSearchModel searchModel);

    }
}