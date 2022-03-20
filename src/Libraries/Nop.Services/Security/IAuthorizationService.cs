using System.Threading.Tasks;

namespace Nop.Services.Security
{
    /// <summary>
    /// Authorization service interface
    /// </summary>
    public partial interface IAuthorizationService
    {
        /// <summary>
        /// Authorize permission
        /// </summary>
        /// <param name="permissionRecordSystemName">Permission record system name</param>
        /// <param name="customerRoleId">Customer role identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the rue - authorized; otherwise, false
        /// </returns>
        Task<bool> AuthorizeAsync(string permissionRecordSystemName, int customerRoleId);
    }
}
