//------------------------------------------------------------------------------
// The contents of this file are subject to the nopCommerce Public License Version 1.0 ("License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at  http://www.nopCommerce.com/License.aspx. 
// 
// Software distributed under the License is distributed on an "AS IS" basis, WITHOUT WARRANTY OF ANY KIND, either express or implied. 
// See the License for the specific language governing rights and limitations under the License.
// 
// The Original Code is nopCommerce.
// The Initial Developer of the Original Code is NopSolutions.
// All Rights Reserved.
// 
// Contributor(s): _______. 
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Nop.Core;
using Nop.Core.Domain.Customers;

namespace Nop.Services.Customers
{
    public static class CustomerExtentions
    {
        public static T GetAttribute<T>(this Customer customer,
            string key)
        {
            if (customer == null)
                throw new ArgumentNullException("customer");

            if (customer.CustomerAttributes == null)
                customer.CustomerAttributes = new List<CustomerAttribute>();
            var customerAttribute = customer.CustomerAttributes.FirstOrDefault(ca => ca.Key.Equals(key, StringComparison.InvariantCultureIgnoreCase));
            if (customerAttribute == null)
                return default(T);

            if (!TypeDescriptor.GetConverter(typeof(T)).CanConvertFrom(typeof(string)))
                throw new NopException("Not supported customer attribute type");

            var attributeValue = (T)(TypeDescriptor.GetConverter(typeof(T)).ConvertFromInvariantString(customerAttribute.Value));
            return attributeValue;
        }
    }
}
