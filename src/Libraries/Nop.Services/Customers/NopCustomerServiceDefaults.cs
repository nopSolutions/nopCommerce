using Nop.Core.Caching;

namespace Nop.Services.Customers
{
    /// <summary>
    /// Represents default values related to customer services
    /// </summary>
    public static partial class NopCustomerServicesDefaults
    {
        /// <summary>
        /// Gets a password salt key size
        /// </summary>
        public static int PasswordSaltKeySize => 5;
        
        /// <summary>
        /// Gets a max username length
        /// </summary>
        public static int CustomerUsernameLength => 100;

        /// <summary>
        /// Gets a default hash format for customer password
        /// </summary>
        public static string DefaultHashedPasswordFormat => "SHA512";

        /// <summary>
        /// Gets default prefix for customer
        /// </summary>
        public static string CustomerAttributePrefix => "customer_attribute_";

        #region Caching defaults

        #region Customer attributes

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        public static CacheKey CustomerAttributesAllCacheKey => new CacheKey("Nop.customerattribute.all");

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : customer attribute ID
        /// </remarks>
        public static CacheKey CustomerAttributeValuesAllCacheKey => new CacheKey("Nop.customerattributevalue.all-{0}");

        #endregion

        #region Customer roles

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : show hidden records?
        /// </remarks>
        public static CacheKey CustomerRolesAllCacheKey => new CacheKey("Nop.customerrole.all-{0}", CustomerRolesPrefixCacheKey);

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : system name
        /// </remarks>
        public static CacheKey CustomerRolesBySystemNameCacheKey => new CacheKey("Nop.customerrole.systemname-{0}", CustomerRolesPrefixCacheKey);

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string CustomerRolesPrefixCacheKey => "Nop.customerrole.";

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : customer identifier
        /// {1} : show hidden
        /// </remarks>
        public static CacheKey CustomerRoleIdsCacheKey => new CacheKey("Nop.customer.customerrole.ids-{0}-{1}", CustomerCustomerRolesPrefixCacheKey);

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : customer identifier
        /// {1} : show hidden
        /// </remarks>
        public static CacheKey CustomerRolesCacheKey => new CacheKey("Nop.customer.customerrole-{0}-{1}", CustomerCustomerRolesPrefixCacheKey);

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string CustomerCustomerRolesPrefixCacheKey => "Nop.customer.customerrole";

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : customer identifier
        /// </remarks>
        public static CacheKey CustomerAddressesByCustomerIdCacheKey => new CacheKey("Nop.customer.addresses.by.id-{0}", CustomerAddressesPrefixCacheKey);

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : customer identifier
        /// {1} : address identifier
        /// </remarks>
        public static CacheKey CustomerAddressCacheKeyCacheKey => new CacheKey("Nop.customer.addresses.address-{0}-{1}", CustomerAddressesPrefixCacheKey);

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string CustomerAddressesPrefixCacheKey => "Nop.customer.addresses";

        #endregion

        #region Customer password

        /// <summary>
        /// Gets a key for caching current customer password lifetime
        /// </summary>
        /// <remarks>
        /// {0} : customer identifier
        /// </remarks>
        public static CacheKey CustomerPasswordLifetimeCacheKey => new CacheKey("Nop.customers.passwordlifetime-{0}");

        #endregion

        #endregion

    }
}