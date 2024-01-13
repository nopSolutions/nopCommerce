using Nop.Core.Caching;

namespace Nop.Services.Customers;

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

    #region Customer

    /// <summary>
    /// Gets a key for caching
    /// </summary>
    /// <remarks>
    /// {0} : system name
    /// </remarks>
    public static CacheKey CustomerBySystemNameCacheKey => new("Nop.customer.bysystemname.{0}");

    /// <summary>
    /// Gets a key for caching
    /// </summary>
    /// <remarks>
    /// {0} : customer GUID
    /// </remarks>
    public static CacheKey CustomerByGuidCacheKey => new("Nop.customer.byguid.{0}");

    #endregion

    #region Customer roles
    
    /// <summary>
    /// Gets a key for caching
    /// </summary>
    /// <remarks>
    /// {0} : system name
    /// </remarks>
    public static CacheKey CustomerRolesBySystemNameCacheKey => new("Nop.customerrole.bysystemname.{0}", CustomerRolesBySystemNamePrefix);

    /// <summary>
    /// Gets a key pattern to clear cache
    /// </summary>
    public static string CustomerRolesBySystemNamePrefix => "Nop.customerrole.bysystemname.";

    /// <summary>
    /// Gets a key for caching
    /// </summary>
    /// <remarks>
    /// {0} : customer identifier
    /// </remarks>
    public static CacheKey CustomerRolesCacheKey => new("Nop.customer.customerrole.{0}", CustomerCustomerRolesPrefix);

    /// <summary>
    /// Gets a key pattern to clear cache
    /// </summary>
    public static string CustomerCustomerRolesPrefix => "Nop.customer.customerrole.";
    
    #endregion

    #region Addresses

    /// <summary>
    /// Gets a key for caching
    /// </summary>
    /// <remarks>
    /// {0} : customer identifier
    /// </remarks>
    public static CacheKey CustomerAddressesCacheKey => new("Nop.customer.addresses.{0}", CustomerAddressesPrefix);

    /// <summary>
    /// Gets a key for caching
    /// </summary>
    /// <remarks>
    /// {0} : customer identifier
    /// {1} : address identifier
    /// </remarks>
    public static CacheKey CustomerAddressCacheKey => new("Nop.customer.addresses.{0}-{1}", CustomerAddressesByCustomerPrefix, CustomerAddressesPrefix);

    /// <summary>
    /// Gets a key pattern to clear cache
    /// </summary>
    public static string CustomerAddressesPrefix => "Nop.customer.addresses.";

    /// <summary>
    /// Gets a key pattern to clear cache
    /// </summary>
    /// <remarks>
    /// {0} : customer identifier
    /// </remarks>
    public static string CustomerAddressesByCustomerPrefix => "Nop.customer.addresses.{0}";

    #endregion

    #region Customer password

    /// <summary>
    /// Gets a key for caching current customer password lifetime
    /// </summary>
    /// <remarks>
    /// {0} : customer identifier
    /// </remarks>
    public static CacheKey CustomerPasswordLifetimeCacheKey => new("Nop.customerpassword.lifetime.{0}");

    #endregion

    #endregion
}