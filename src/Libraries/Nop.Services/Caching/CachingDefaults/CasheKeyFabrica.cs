using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nop.Core.Caching;
using Nop.Core.Domain;
using Nop.Services.Security;

namespace Nop.Services.Caching.CachingDefaults
{
    public partial class CasheKeyFactory : ICasheKeyFactory
    {
        private readonly CachingSettings _cachingSettings;
        private readonly IEncryptionService _encryptionService;

        public CasheKeyFactory(CachingSettings cachingSettings, IEncryptionService encryptionService)
        {
            _cachingSettings = cachingSettings;
            _encryptionService = encryptionService;
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

        public virtual string GetShoppingCartCacheKey(params object[] keyObjects)
        {
            if (!_cachingSettings.CachingShoppingCartEnabled)
                return null;

            return CreateCacheKey(NopOrderCachingDefaults.ShoppingCartCacheKey, keyObjects);
        }

        public virtual string GetIdsHash(IEnumerable<int> ids)
        {
            return _encryptionService.CreateHash(Encoding.UTF8.GetBytes(string.Join(", ", ids.OrderBy(id => id))),
                NopCustomerServiceDefaults.DefaultHashedPasswordFormat);
        }
    }
}
