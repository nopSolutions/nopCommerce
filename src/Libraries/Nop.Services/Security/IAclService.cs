using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Security;

namespace Nop.Services.Security;

/// <summary>
/// ACL service interface
/// </summary>
public partial interface IAclService
{
    /// <summary>
    /// Apply ACL to the passed query
    /// </summary>
    /// <typeparam name="TEntity">Type of entity that supports the ACL</typeparam>
    /// <param name="query">Query to filter</param>
    /// <param name="customer">Customer</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the filtered query
    /// </returns>
    Task<IQueryable<TEntity>> ApplyAcl<TEntity>(IQueryable<TEntity> query, Customer customer) where TEntity : BaseEntity, IAclSupported;

    /// <summary>
    /// Apply ACL to the passed query
    /// </summary>
    /// <typeparam name="TEntity">Type of entity that supports the ACL</typeparam>
    /// <param name="query">Query to filter</param>
    /// <param name="customerRoleIds">Identifiers of customer's roles</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the filtered query
    /// </returns>
    Task<IQueryable<TEntity>> ApplyAcl<TEntity>(IQueryable<TEntity> query, int[] customerRoleIds) where TEntity : BaseEntity, IAclSupported;

    /// <summary>
    /// Deletes an ACL record
    /// </summary>
    /// <param name="aclRecord">ACL record</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task DeleteAclRecordAsync(AclRecord aclRecord);

    /// <summary>
    /// Gets ACL records
    /// </summary>
    /// <typeparam name="TEntity">Type of entity that supports the ACL</typeparam>
    /// <param name="entity">Entity</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the ACL records
    /// </returns>
    Task<IList<AclRecord>> GetAclRecordsAsync<TEntity>(TEntity entity) where TEntity : BaseEntity, IAclSupported;

    /// <summary>
    /// Inserts an ACL record
    /// </summary>
    /// <typeparam name="TEntity">Type of entity that supports the ACL</typeparam>
    /// <param name="entity">Entity</param>
    /// <param name="customerRoleId">Customer role id</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task InsertAclRecordAsync<TEntity>(TEntity entity, int customerRoleId) where TEntity : BaseEntity, IAclSupported;

    /// <summary>
    /// Find customer role identifiers with granted access
    /// </summary>
    /// <param name="entityId">Entity ID</param>
    /// <param name="entityName">Entity name</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the customer role identifiers
    /// </returns>
    Task<int[]> GetCustomerRoleIdsWithAccessAsync(int entityId, string entityName);

    /// <summary>
    /// Authorize ACL permission
    /// </summary>
    /// <typeparam name="TEntity">Type of entity that supports the ACL</typeparam>
    /// <param name="entity">Entity</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains true - authorized; otherwise, false
    /// </returns>
    Task<bool> AuthorizeAsync<TEntity>(TEntity entity) where TEntity : BaseEntity, IAclSupported;

    /// <summary>
    /// Authorize ACL permission
    /// </summary>
    /// <typeparam name="TEntity">Type of entity that supports the ACL</typeparam>
    /// <param name="entity">Entity</param>
    /// <param name="customer">Customer</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains true - authorized; otherwise, false
    /// </returns>
    Task<bool> AuthorizeAsync<TEntity>(TEntity entity, Customer customer) where TEntity : BaseEntity, IAclSupported;

    /// <summary>
    /// Authorize ACL permission
    /// </summary>
    /// <param name="entityTypeName">Type name of entity that supports the ACL</param>
    /// <param name="entityId">Entity ID</param>
    /// <param name="customer">Customer</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains true - authorized; otherwise, false
    /// </returns>
    Task<bool> AuthorizeAsync(string entityTypeName, int entityId, Customer customer);

    /// <summary>
    /// Authorize ACL permission
    /// </summary>
    /// <param name="customer">Customer</param>
    /// <param name="allowedCustomerRoleIds">List of allowed customer role IDs</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains true - authorized; otherwise, false
    /// </returns>
    Task<bool> AuthorizeAsync(Customer customer, IList<int> allowedCustomerRoleIds);

    /// <summary>
    /// Save ACL mapping
    /// </summary>
    /// <typeparam name="TEntity">Type of entity</typeparam>
    /// <param name="entity">Entity</param>
    /// <param name="selectedCustomerRoleIds">Customer roles for mapping</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task SaveAclAsync<TEntity>(TEntity entity, IList<int> selectedCustomerRoleIds) where TEntity : BaseEntity, IAclSupported;
}