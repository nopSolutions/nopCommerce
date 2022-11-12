<<<<<<< HEAD
﻿using System.Collections.Generic;
using Nop.Core.Domain.Security;

namespace Nop.Services.Security
{
    /// <summary>
    /// Permission provider
    /// </summary>
    public interface IPermissionProvider
    {
        /// <summary>
        /// Get permissions
        /// </summary>
        /// <returns>Permissions</returns>
        IEnumerable<PermissionRecord> GetPermissions();

        /// <summary>
        /// Get default permissions
        /// </summary>
        /// <returns>Default permissions</returns>
        HashSet<(string systemRoleName, PermissionRecord[] permissions)> GetDefaultPermissions();
    }
}
=======
﻿using System.Collections.Generic;
using Nop.Core.Domain.Security;

namespace Nop.Services.Security
{
    /// <summary>
    /// Permission provider
    /// </summary>
    public interface IPermissionProvider
    {
        /// <summary>
        /// Get permissions
        /// </summary>
        /// <returns>Permissions</returns>
        IEnumerable<PermissionRecord> GetPermissions();

        /// <summary>
        /// Get default permissions
        /// </summary>
        /// <returns>Default permissions</returns>
        HashSet<(string systemRoleName, PermissionRecord[] permissions)> GetDefaultPermissions();
    }
}
>>>>>>> 174426a8e1a9c69225a65c26a93d9aa871080855
