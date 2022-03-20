using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nop.Core.Caching;
using Nop.Core.Domain.Security;
using Nop.Data;

namespace Nop.Services.Security
{
    public partial class AuthorizationService : IAuthorizationService
    {
        #region Fields

        private readonly IRepository<PermissionRecord> _permissionRecordRepository;
        private readonly IRepository<PermissionRecordCustomerRoleMapping> _permissionRecordCustomerRoleMappingRepository;
        private readonly IStaticCacheManager _staticCacheManager;

        #endregion

        #region Ctor

        public AuthorizationService(IRepository<PermissionRecord> permissionRecordRepository,
            IRepository<PermissionRecordCustomerRoleMapping> permissionRecordCustomerRoleMappingRepository,
            IStaticCacheManager staticCacheManager)
        {
            _permissionRecordRepository = permissionRecordRepository;
            _permissionRecordCustomerRoleMappingRepository = permissionRecordCustomerRoleMappingRepository;
            _staticCacheManager = staticCacheManager;
        }

        #endregion

        /// <summary>
        /// Authorize permission
        /// </summary>
        /// <param name="permissionRecordSystemName">Permission record system name</param>
        /// <param name="customerRoleId">Customer role identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the rue - authorized; otherwise, false
        /// </returns>
        public virtual async Task<bool> AuthorizeAsync(string permissionRecordSystemName, int customerRoleId)
        {
            if (string.IsNullOrEmpty(permissionRecordSystemName))
                return false;

            var key = _staticCacheManager.PrepareKeyForDefaultCache(NopSecurityDefaults.PermissionAllowedCacheKey, permissionRecordSystemName, customerRoleId);

            return await _staticCacheManager.GetAsync(key, async () =>
            {
                var permissions = await GetPermissionRecordsByCustomerRoleIdAsync(customerRoleId);
                foreach (var permission in permissions)
                    if (permission.SystemName.Equals(permissionRecordSystemName, StringComparison.InvariantCultureIgnoreCase))
                        return true;

                return false;
            });
        }

        /// <summary>
        /// Get permission records by customer role identifier
        /// </summary>
        /// <param name="customerRoleId">Customer role identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the permissions
        /// </returns>
        protected virtual async Task<IList<PermissionRecord>> GetPermissionRecordsByCustomerRoleIdAsync(int customerRoleId)
        {
            var key = _staticCacheManager.PrepareKeyForDefaultCache(NopSecurityDefaults.PermissionRecordsAllCacheKey, customerRoleId);

            var query = from pr in _permissionRecordRepository.Table
                        join prcrm in _permissionRecordCustomerRoleMappingRepository.Table on pr.Id equals prcrm
                            .PermissionRecordId
                        where prcrm.CustomerRoleId == customerRoleId
                        orderby pr.Id
                        select pr;

            return await _staticCacheManager.GetAsync(key, async () => await query.ToListAsync());
        }
    }
}
