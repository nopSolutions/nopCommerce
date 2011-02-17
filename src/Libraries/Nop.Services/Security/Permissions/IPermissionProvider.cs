
using System.Collections.Generic;
using Nop.Core.Domain.Security.Permissions;

namespace Nop.Services.Security.Permissions
{
    public interface IPermissionProvider
    {
        IEnumerable<PermissionRecord> GetPermissions();
        IEnumerable<DefaultPermissionRecord> GetDefaultPermissions();
    }
}
