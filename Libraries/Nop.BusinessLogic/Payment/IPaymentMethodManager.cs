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
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Text;
using NopSolutions.NopCommerce.BusinessLogic.Caching;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.Data;
using NopSolutions.NopCommerce.BusinessLogic.Directory;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.BusinessLogic.IoC;

namespace NopSolutions.NopCommerce.BusinessLogic.Payment
{
    /// <summary>
    /// Payment method manager
    /// </summary>
    public partial interface IPaymentMethodManager
    {
        /// <summary>
        /// Deletes a payment method
        /// </summary>
        /// <param name="paymentMethodId">Payment method identifier</param>
        void DeletePaymentMethod(int paymentMethodId);

        /// <summary>
        /// Gets a payment method
        /// </summary>
        /// <param name="paymentMethodId">Payment method identifier</param>
        /// <returns>Payment method</returns>
        PaymentMethod GetPaymentMethodById(int paymentMethodId);

        /// <summary>
        /// Gets a payment method
        /// </summary>
        /// <param name="systemKeyword">Payment method system keyword</param>
        /// <returns>Payment method</returns>
        PaymentMethod GetPaymentMethodBySystemKeyword(string systemKeyword);

        /// <summary>
        /// Gets all payment methods
        /// </summary>
        /// <returns>Payment method collection</returns>
        List<PaymentMethod> GetAllPaymentMethods();

        /// <summary>
        /// Gets all payment methods
        /// </summary>
        /// <param name="filterByCountryId">The country indentifier</param>
        /// <returns>Payment method collection</returns>
        List<PaymentMethod> GetAllPaymentMethods(int? filterByCountryId);

        /// <summary>
        /// Gets all payment methods
        /// </summary>
        /// <param name="filterByCountryId">The country indentifier</param>
        /// <param name="showHidden">A value indicating whether the not active payment methods should be load</param>
        /// <returns>Payment method collection</returns>
        List<PaymentMethod> GetAllPaymentMethods(int? filterByCountryId, bool showHidden);

        /// <summary>
        /// Inserts a payment method
        /// </summary>
        /// <param name="paymentMethod">Payment method</param>
        void InsertPaymentMethod(PaymentMethod paymentMethod);

        /// <summary>
        /// Updates the payment method
        /// </summary>
        /// <param name="paymentMethod">Payment method</param>
        void UpdatePaymentMethod(PaymentMethod paymentMethod);

        /// <summary>
        /// Creates the payment method country mapping
        /// </summary>
        /// <param name="paymentMethodId">The payment method identifier</param>
        /// <param name="countryId">The country identifier</param>
        void CreatePaymentMethodCountryMapping(int paymentMethodId, int countryId);

        /// <summary>
        /// Checking whether the payment method country mapping exists
        /// </summary>
        /// <param name="paymentMethodId">The payment method identifier</param>
        /// <param name="countryId">The country identifier</param>
        /// <returns>True if mapping exist, otherwise false</returns>
        bool DoesPaymentMethodCountryMappingExist(int paymentMethodId, int countryId);

        /// <summary>
        /// Deletes the payment method country mapping
        /// </summary>
        /// <param name="paymentMethodId">The payment method identifier</param>
        /// <param name="countryId">The country identifier</param>
        void DeletePaymentMethodCountryMapping(int paymentMethodId, int countryId);
    }
}
