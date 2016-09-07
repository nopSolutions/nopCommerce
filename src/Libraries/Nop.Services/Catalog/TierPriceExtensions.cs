using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;

namespace Nop.Services.Catalog
{
    /// <summary>
    /// Extensions
    /// </summary>
    public static class TierPriceExtensions
    {
        /// <summary>
        /// Filter tier prices by a store
        /// </summary>
        /// <param name="source">Tier prices</param>
        /// <param name="storeId">Store identifier</param>
        /// <returns>Filtered tier prices</returns>
        public static IList<TierPrice> FilterByStore(this IList<TierPrice> source,
            int storeId)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            var result = new List<TierPrice>();
            foreach (var tierPrice in source)
            {
                //check store requirement
                if (tierPrice.StoreId > 0 && tierPrice.StoreId != storeId)
                    continue;

                result.Add(tierPrice);
            }

            return result;
        }

        /// <summary>
        /// Filter tier prices for a customer
        /// </summary>
        /// <param name="source">Tier prices</param>
        /// <param name="customer">Customer</param>
        /// <returns>Filtered tier prices</returns>
        public static IList<TierPrice> FilterForCustomer(this IList<TierPrice> source,
            Customer customer)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            var result = new List<TierPrice>();
            foreach (var tierPrice in source)
            {
                //check customer role requirement
                if (tierPrice.CustomerRole != null)
                {
                    if (customer == null)
                        continue;

                    var customerRoles = customer.CustomerRoles.Where(cr => cr.Active).ToList();
                    if (!customerRoles.Any())
                        continue;

                    bool roleIsFound = false;
                    foreach (var customerRole in customerRoles)
                        if (customerRole == tierPrice.CustomerRole)
                            roleIsFound = true;

                    if (!roleIsFound)
                        continue;

                }

                result.Add(tierPrice);
            }

            return result;
        }

        /// <summary>
        /// Remove duplicated quantities (leave only a tier price with minimum price)
        /// </summary>
        /// <param name="source">Tier prices</param>
        /// <returns>Filtered tier prices</returns>
        public static IList<TierPrice> RemoveDuplicatedQuantities(this IList<TierPrice> source)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            
            //find duplicates
            var query = from tierPrice in source
                        group tierPrice by tierPrice.Quantity into g
                        where g.Count() > 1
                        select new { Quantity = g.Key, TierPrices = g.ToList() };
            foreach (var item in query)
            {
                //find a tier price record with minimum price (we'll not remove it)
                var minTierPrice = item.TierPrices.Aggregate((tp1, tp2) => (tp1.Price < tp2.Price ? tp1 : tp2));
                //remove all other records
                item.TierPrices.Remove(minTierPrice);
                item.TierPrices.ForEach(x=> source.Remove(x));
            }

            return source;
        }
    }
}
