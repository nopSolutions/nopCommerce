using Nop.Core.Configuration;

namespace Nop.Core.Domain
{
    public partial class CachingSettings: ISettings
    {
        public bool CachingCustomerRolesEnabled { get; set; } = true;
        public bool CachingCustomerAddressEnabled { get; set; } = true;
    }
}
