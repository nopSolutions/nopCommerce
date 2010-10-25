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
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.BusinessLogic.IoC;

namespace NopSolutions.NopCommerce.BusinessLogic.Payment
{
    /// <summary>
    /// Credit card type manager interface
    /// </summary>
    public partial interface ICreditCardTypeManager
    {
        /// <summary>
        /// Gets a credit card type
        /// </summary>
        /// <param name="creditCardTypeId">Credit card type identifier</param>
        /// <returns>Credit card type</returns>
        CreditCardType GetCreditCardTypeById(int creditCardTypeId);

        /// <summary>
        /// Marks a credit card type as deleted
        /// </summary>
        /// <param name="creditCardTypeId">Credit card type identifier</param>
        void MarkCreditCardTypeAsDeleted(int creditCardTypeId);

        /// <summary>
        /// Gets all credit card types
        /// </summary>
        /// <returns>Credit card type collection</returns>
        List<CreditCardType> GetAllCreditCardTypes();

        /// <summary>
        /// Inserts a credit card type
        /// </summary>
        /// <param name="creditCardType">Credit card type</param>
        void InsertCreditCardType(CreditCardType creditCardType);

        /// <summary>
        /// Updates the credit card type
        /// </summary>
        /// <param name="creditCardType">Credit card type</param>
        void UpdateCreditCardType(CreditCardType creditCardType);
    }
}
