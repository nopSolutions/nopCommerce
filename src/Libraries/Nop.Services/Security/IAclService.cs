using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Seo;

namespace Nop.Services.Seo
{
    /// <summary>
    /// ACL service inerface
    /// </summary>
    public partial interface IAclService
    {
        /// <summary>
        /// Deletes an ACL record
        /// </summary>
        /// <param name="aclRecord">ACL record</param>
        void DeleteAclRecord(AclRecord aclRecord);

        /// <summary>
        /// Gets an ACL record
        /// </summary>
        /// <param name="aclRecordId">ACL record identifier</param>
        /// <returns>ACL record</returns>
        AclRecord GetAclRecordById(int aclRecordId);
        
        /// <summary>
        /// Gets ACL records
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="entity">Entity</param>
        /// <returns>ACL records</returns>
        IList<AclRecord> GetAclRecords<T>(T entity) where T : BaseEntity, IAclSupported;

        /// <summary>
        /// Inserts an ACL record
        /// </summary>
        /// <param name="aclRecord">ACL record</param>
        void InsertAclRecord(AclRecord aclRecord);
        
        /// <summary>
        /// Inserts an ACL record
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="entity">Entity</param>
        void InsertAclRecord<T>(T entity, int customerRoleId) where T : BaseEntity, IAclSupported;

        /// <summary>
        /// Updates the ACL record
        /// </summary>
        /// <param name="aclRecord">ACL record</param>
        void UpdateAclRecord(AclRecord aclRecord);

        /// <summary>
        /// Find customer role identifiers with granted access
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="entity">Wntity</param>
        /// <returns>Customer role identifiers</returns>
        int[] GetCustomerRoleIdsWithAccess<T>(T entity) where T : BaseEntity, IAclSupported;

        /// <summary>
        /// Authorize ACL permission
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="entity">Wntity</param>
        /// <returns>true - authorized; otherwise, false</returns>
        bool Authorize<T>(T entity) where T : BaseEntity, IAclSupported;

        /// <summary>
        /// Authorize ACL permission
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="entity">Wntity</param>
        /// <param name="customer">Customer</param>
        /// <returns>true - authorized; otherwise, false</returns>
        bool Authorize<T>(T entity, Customer customer) where T : BaseEntity, IAclSupported;
    }
}