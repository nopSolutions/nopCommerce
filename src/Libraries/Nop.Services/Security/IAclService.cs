using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq.Expressions;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Security;

namespace Nop.Services.Security
{
    /// <summary>
    /// ACL service interface
    /// </summary>
    public partial interface IAclService
    {
        /// <summary>
        /// Get an expression predicate to apply a store mapping
        /// </summary>
        /// <param name="customerRoleIds">Identifiers of customer's roles</param>
        /// <typeparam name="TEntity">Type of entity with supported store mapping</typeparam>
        /// <returns>Lambda expression</returns>
        Expression<Func<TEntity, bool>> ApplyAcl<TEntity>(int[] customerRoleIds) where TEntity : BaseEntity, IAclSupported;

        /// <summary>
        /// Deletes an ACL record
        /// </summary>
        /// <param name="aclRecord">ACL record</param>
        Task DeleteAclRecordAsync(AclRecord aclRecord);

        //TODO: may be deleted
        /// <summary>
        /// Gets an ACL record
        /// </summary>
        /// <param name="aclRecordId">ACL record identifier</param>
        /// <returns>ACL record</returns>
        Task<AclRecord> GetAclRecordByIdAsync(int aclRecordId);

        /// <summary>
        /// Gets ACL records
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="entity">Entity</param>
        /// <returns>ACL records</returns>
        Task<IList<AclRecord>> GetAclRecordsAsync<T>(T entity) where T : BaseEntity, IAclSupported;

        //TODO: may be deleted from interface
        /// <summary>
        /// Inserts an ACL record
        /// </summary>
        /// <param name="aclRecord">ACL record</param>
        Task InsertAclRecordAsync(AclRecord aclRecord);

        /// <summary>
        /// Inserts an ACL record
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="customerRoleId">Customer role id</param>
        /// <param name="entity">Entity</param>
        Task InsertAclRecordAsync<T>(T entity, int customerRoleId) where T : BaseEntity, IAclSupported;

        //TODO: may be deleted
        /// <summary>
        /// Get a value indicating whether any ACL records exist for entity type are related to customer roles
        /// </summary>
        /// <param name="customerRoleIds">Customer's role identifiers</param>
        /// <typeparam name="T">Entity type</typeparam>
        /// <returns>True if exist; otherwise false</returns>
        bool IsEntityAclMappingExist<T>(int[] customerRoleIds) where T : BaseEntity, IAclSupported;

        /// <summary>
        /// Updates the ACL record
        /// </summary>
        /// <param name="aclRecord">ACL record</param>
        Task UpdateAclRecordAsync(AclRecord aclRecord);

        /// <summary>
        /// Find customer role identifiers with granted access
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="entity">Entity</param>
        /// <returns>Customer role identifiers</returns>
        Task<int[]> GetCustomerRoleIdsWithAccessAsync<T>(T entity) where T : BaseEntity, IAclSupported;

        /// <summary>
        /// Authorize ACL permission
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="entity">Entity</param>
        /// <returns>true - authorized; otherwise, false</returns>
        Task<bool> AuthorizeAsync<T>(T entity) where T : BaseEntity, IAclSupported;

        /// <summary>
        /// Authorize ACL permission
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="entity">Entity</param>
        /// <param name="customer">Customer</param>
        /// <returns>true - authorized; otherwise, false</returns>
        Task<bool> AuthorizeAsync<T>(T entity, Customer customer) where T : BaseEntity, IAclSupported;
    }
}