using System;
using System.Collections.Generic;
using System.Linq;

namespace Nop.Core.Domain.Stores
{
    public static class StoreExtensions
    {
        /// <summary>
        /// Parse comma-separated Hosts
        /// </summary>
        /// <param name="store">Store</param>
        /// <returns>Comma-separated hosts</returns>
        public static string[] ParseHostValues(this Store store)
        {
            if (store == null)
                throw new ArgumentNullException("store");

            var parsedValues = new List<string>();
            if (!String.IsNullOrEmpty(store.Hosts))
            {
                string[] hosts = store.Hosts.Split(new [] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string host in hosts)
                {
                    var tmp = host.Trim();
                    if (!String.IsNullOrEmpty(tmp))
                        parsedValues.Add(tmp);
                }
            }
            return parsedValues.ToArray();
        }

        /// <summary>
        /// Indicates whether a store contains a specified host
        /// </summary>
        /// <param name="store">Store</param>
        /// <param name="host">Host</param>
        /// <returns>true - contains, false - no</returns>
        public static bool ContainsHostValue(this Store store, string host)
        {
            if (store == null)
                throw new ArgumentNullException("store");

            if (String.IsNullOrEmpty(host))
                return false;

            var contains = store.ParseHostValues()
                                .FirstOrDefault(x => x.Equals(host, StringComparison.InvariantCultureIgnoreCase)) != null;
            return contains;
        }
    }
}
