using System.Collections.Generic;
using Nop.Core.Domain.Security;

namespace Nop.Services.Security
{
    /// <summary>
    /// 定义权限提供者要实现的接口
    /// Permission provider
    /// </summary>
    public interface IPermissionProvider
    {
        /// <summary>
        /// 获得所有权限
        /// Get permissions
        /// </summary>
        /// <returns>Permissions</returns>
        IEnumerable<PermissionRecord> GetPermissions();

        /// <summary>
        /// 获得默认权限
        /// Get default permissions
        /// </summary>
        /// <returns>Default permissions</returns>
        IEnumerable<DefaultPermissionRecord> GetDefaultPermissions();
    }
}
