namespace Nop.Services.Caching.CachingDefaults
{
    /// <summary>
    /// Represents default values related to customer services
    /// </summary>
    public static partial class NopCustomerServiceCachingDefaults
    {
        #region Customer attributes

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        public static string CustomerAttributesAllCacheKey => "Nop.customerattribute.all";

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : customer attribute ID
        /// </remarks>
        public static string CustomerAttributesByIdCacheKey => "Nop.customerattribute.id-{0}";

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : customer attribute ID
        /// </remarks>
        public static string CustomerAttributeValuesAllCacheKey => "Nop.customerattributevalue.all-{0}";

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : customer attribute value ID
        /// </remarks>
        public static string CustomerAttributeValuesByIdCacheKey => "Nop.customerattributevalue.id-{0}";

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string CustomerAttributesPrefixCacheKey => "Nop.customerattribute.";

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string CustomerAttributeValuesPrefixCacheKey => "Nop.customerattributevalue.";

        #endregion

        #region Customer roles

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : show hidden records?
        /// </remarks>
        public static string CustomerRolesAllCacheKey => "Nop.customerrole.all-{0}";

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : system name
        /// </remarks>
        public static string CustomerRolesBySystemNameCacheKey => "Nop.customerrole.systemname-{0}";

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
        public static string CustomerRoleIdsCacheKey => "Nop.customer.customerrole.ids-{0}-{1}";

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : customer identifier
        /// {1} : show hidden
        /// </remarks>
        public static string CustomerRolesCacheKey => "Nop.customer.customerrole-{0}-{1}";
        
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
        public static string CustomerAddressesByCustomerIdCacheKey => "Nop.customer.addresses.by.id-{0}";

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : customer identifier
        /// {1} : address identifier
        /// </remarks>
        public static string CustomerAddressCacheKeyCacheKey => "Nop.customer.addresses.address-{0}-{1}";

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string CustomerAddressesPrefixCacheKey => "Nop.customer.addresses";

        #endregion

        /// <summary>
        /// Gets a key for caching current customer password lifetime
        /// </summary>
        /// <remarks>
        /// {0} : customer identifier
        /// </remarks>
        public static string CustomerPasswordLifetimeCacheKey => "Nop.customers.passwordlifetime-{0}";
    }
}