using System.Linq;
using Nop.Core.Domain;

namespace Nop.Services.Caching.CachingDefaults
{
    public partial class CasheKeyFactory : ICasheKeyFactory
    {
        private readonly CachingSettings _cachingSettings;

        public CasheKeyFactory(CachingSettings cachingSettings)
        {
            _cachingSettings = cachingSettings;
        }

        protected virtual string CreateCacheKey(string keyFormater, params object[] keyObjects)
        {
            return keyObjects.Any() ? string.Format(keyFormater, keyObjects) : keyFormater;
        }

        public virtual string GetCustomerRoleIdsCacheKey(params object[] keyObjects)
        {
            if (!_cachingSettings.CachingCustomerRolesEnabled)
                return null;

            return CreateCacheKey(NopCustomerServiceCachingDefaults.CustomerRoleIdsCacheKey, keyObjects);
        }

        public virtual string GetCustomerRolesCacheKey(params object[] keyObjects)
        {
            if (!_cachingSettings.CachingCustomerRolesEnabled)
                return null;

            return CreateCacheKey(NopCustomerServiceCachingDefaults.CustomerRolesCacheKey, keyObjects);
        }

        public virtual string GetIsInCustomerRoleCacheKey(params object[] keyObjects)
        {
            if (!_cachingSettings.CachingCustomerRolesEnabled)
                return null;

            return CreateCacheKey(NopCustomerServiceCachingDefaults.IsInCustomerRoleCacheKey, keyObjects);
        }

        public virtual string GetAddressesByCustomerIdCacheKey(params object[] keyObjects)
        {
            if (!_cachingSettings.CachingCustomerAddressEnabled)
                return null;

            return CreateCacheKey(NopCustomerServiceCachingDefaults.CustomerAddressesByCustomerIdCacheKey, keyObjects);
        }

        public virtual string GetCustomerAddressCacheKey(params object[] keyObjects)
        {
            if (!_cachingSettings.CachingCustomerAddressEnabled)
                return null;

            return CreateCacheKey(NopCustomerServiceCachingDefaults.CustomerAddressCacheKeyCacheKey, keyObjects);
        }



        
    }
}
