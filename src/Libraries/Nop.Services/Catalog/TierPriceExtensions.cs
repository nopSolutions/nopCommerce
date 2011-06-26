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
        public static IList<TierPrice> FilterForCustomer(this IList<TierPrice> source,
            Customer customer)
        {
            var result = new List<TierPrice>();
            foreach (var tierPrice in source)
            {
                //check customer role requirement
                if (tierPrice.CustomerRole != null)
                {
                    if (customer == null)
                        continue;

                    var customerRoles = customer.CustomerRoles.Where(cr => cr.Active).ToList();
                    if (customerRoles.Count == 0)
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
    }
}
