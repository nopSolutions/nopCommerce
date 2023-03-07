using Nop.Web.Areas.Admin.Models.Security;

namespace Nop.Web.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the security model factory
    /// </summary>
    public partial interface ISecurityModelFactory
    {
        /// <summary>
        /// Prepare permission mapping model
        /// </summary>
        /// <param name="model">Permission mapping model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the permission mapping model
        /// </returns>
        Task<PermissionMappingModel> PreparePermissionMappingModelAsync(PermissionMappingModel model);
    }
}