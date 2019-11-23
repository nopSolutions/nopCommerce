namespace Nop.Services.Customers
{
    /// <summary>
    /// Represents default values related to customer services
    /// </summary>
    public static partial class NopCustomerServiceDefaults
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

        #endregion

        /// <summary>
        /// Gets a key for caching current customer password lifetime
        /// </summary>
        /// <remarks>
        /// {0} : customer identifier
        /// </remarks>
        public static string CustomerPasswordLifetimeCacheKey => "Nop.customers.passwordlifetime-{0}";

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
    }
}