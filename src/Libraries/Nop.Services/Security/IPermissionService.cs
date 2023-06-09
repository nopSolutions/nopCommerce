using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Security;

namespace Nop.Services.Security
{
    /// <summary>
    /// Permission service interface
    /// </summary>
    public partial interface IPermissionService
    {
        /// <summary>
        /// Gets all permissions
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the permissions
        /// </returns>
        Task<IList<PermissionRecord>> GetAllPermissionRecordsAsync();

        /// <summary>
        /// Inserts a permission
        /// </summary>
        /// <param name="permission">Permission</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task InsertPermissionRecordAsync(PermissionRecord permission);

        /// <summary>
        /// Gets a permission record by identifier
        /// </summary>
        /// <param name="permission">Permission</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains a permission record
        /// </returns>
        Task<PermissionRecord> GetPermissionRecordByIdAsync(int permissionId);

        /// <summary>
        /// Updates the permission
        /// </summary>
        /// <param name="permission">Permission</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task UpdatePermissionRecordAsync(PermissionRecord permission);

        /// <summary>
        /// Deletes the permission
        /// </summary>
        /// <param name="permission">Permission</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task DeletePermissionRecordAsync(PermissionRecord permission);

        /// <summary>
        /// Install permissions
        /// </summary>
        /// <param name="permissionProvider">Permission provider</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task InstallPermissionsAsync(IPermissionProvider permissionProvider);

        /// <summary>
        /// Install permissions
        /// </summary>
        /// <param name="permissionProvider">Permission provider</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task UninstallPermissionsAsync(IPermissionProvider permissionProvider);

        /// <summary>
        /// Authorize permission
        /// </summary>
        /// <param name="permission">Permission record</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the rue - authorized; otherwise, false
        /// </returns>
        Task<bool> AuthorizeAsync(PermissionRecord permission);

        /// <summary>
        /// Authorize permission
        /// </summary>
        /// <param name="permission">Permission record</param>
        /// <param name="customer">Customer</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the rue - authorized; otherwise, false
        /// </returns>
        Task<bool> AuthorizeAsync(PermissionRecord permission, Customer customer);

        /// <summary>
        /// Authorize permission
        /// </summary>
        /// <param name="permissionRecordSystemName">Permission record system name</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the rue - authorized; otherwise, false
        /// </returns>
        Task<bool> AuthorizeAsync(string permissionRecordSystemName);

        /// <summary>
        /// Authorize permission
        /// </summary>
        /// <param name="permissionRecordSystemName">Permission record system name</param>
        /// <param name="customer">Customer</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the rue - authorized; otherwise, false
        /// </returns>
        Task<bool> AuthorizeAsync(string permissionRecordSystemName, Customer customer);

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

        /// <summary>
        /// Gets a permission record-customer role mapping
        /// </summary>
        /// <param name="permissionId">Permission identifier</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task<IList<PermissionRecordCustomerRoleMapping>> GetMappingByPermissionRecordIdAsync(int permissionId);

        /// <summary>
        /// Delete a permission record-customer role mapping
        /// </summary>
        /// <param name="permissionId">Permission identifier</param>
        /// <param name="customerRoleId">Customer role identifier</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task DeletePermissionRecordCustomerRoleMappingAsync(int permissionId, int customerRoleId);

        /// <summary>
        /// Inserts a permission record-customer role mapping
        /// </summary>
        /// <param name="permissionRecordCustomerRoleMapping">Permission record-customer role mapping</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task InsertPermissionRecordCustomerRoleMappingAsync(PermissionRecordCustomerRoleMapping permissionRecordCustomerRoleMapping);
    }
}