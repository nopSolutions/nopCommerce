using System.Collections.Generic;

namespace Nop.Services.Caching.CachingDefaults
{
    public interface ICasheKeyFactory
    {
        /// <summary>
        /// Create the customer role Ids cache key
        /// </summary>
        /// <param name="keyObjects">Parameters to create cache key</param>
        /// <returns></returns>
        string GetCustomerRoleIdsCacheKey(params object[] keyObjects);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="keyObjects"></param>
        /// <returns></returns>
        string GetCustomerRolesCacheKey(params object[] keyObjects);
       
        /// <summary>
        /// 
        /// </summary>
        /// <param name="keyObjects"></param>
        /// <returns></returns>
        string GetAddressesByCustomerIdCacheKey(params object[] keyObjects);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="keyObjects"></param>
        /// <returns></returns>
        string GetCustomerAddressCacheKey(params object[] keyObjects);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="keyObjects"></param>
        /// <returns></returns>
        string GetShoppingCartCacheKey(params object[] keyObjects);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        string GetIdsHash(IEnumerable<int> ids);
    }
}