using System.Collections.Generic;

namespace Nop.Services.Caching.CachingDefaults
{
    public interface ICasheKeyFactory
    {
        string GetCustomerRoleIdsCacheKey(params object[] keyObjects);
        string GetCustomerRolesCacheKey(params object[] keyObjects);
        string GetIsInCustomerRoleCacheKey(params object[] keyObjects);
        string GetAddressesByCustomerIdCacheKey(params object[] keyObjects);
        string GetCustomerAddressCacheKey(params object[] keyObjects);
        string GetIdsHash(IEnumerable<int> ids);
    }
}