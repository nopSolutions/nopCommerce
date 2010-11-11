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
using System.Text;
using NopSolutions.NopCommerce.BusinessLogic.Infrastructure;


namespace NopSolutions.NopCommerce.BusinessLogic.Directory
{
    /// <summary>
    /// Represents a currency
    /// </summary>
    public partial class Currency : BaseEntity
    {
        #region Ctor

        /// <summary>
        /// Creates a new instance of the Currency class
        /// </summary>
        public Currency()
        {
        }
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the currency identifier
        /// </summary>
        public int CurrencyId { get; set; }

        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the currency code
        /// </summary>
        public string CurrencyCode { get; set; }

        /// <summary>
        /// Gets or sets the rate
        /// </summary>
        public decimal Rate { get; set; }

        /// <summary>
        /// Gets or sets the display locale
        /// </summary>
        public string DisplayLocale { get; set; }

        /// <summary>
        /// Gets or sets the custom formatting
        /// </summary>
        public string CustomFormatting { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the entity is published
        /// </summary>
        public bool Published { get; set; }

        /// <summary>
        /// Gets or sets the display order
        /// </summary>
        public int DisplayOrder { get; set; }

        /// <summary>
        /// Gets or sets the date and time of instance creation
        /// </summary>
        public DateTime CreatedOn { get; set; }

        /// <summary>
        /// Gets or sets the date and time of instance update
        /// </summary>
        public DateTime UpdatedOn { get; set; }

        #endregion 
        
        #region Custom Properties

        /// <summary>
        /// Gets or a value indicating whether the currency is primary exchange rate currency
        /// </summary>
        public bool IsPrimaryExchangeRateCurrency
        {
            get
            {
                Currency activePrimaryExchangeRateCurrency = IoC.Resolve<ICurrencyService>().PrimaryExchangeRateCurrency;
                return ((activePrimaryExchangeRateCurrency != null && activePrimaryExchangeRateCurrency.CurrencyId == this.CurrencyId));
            }
        }

        /// <summary>
        /// Gets or a value indicating whether the currency is primary store currency
        /// </summary>
        public bool IsPrimaryStoreCurrency
        {
            get
            {
                Currency activePrimaryStoreCurrency = IoC.Resolve<ICurrencyService>().PrimaryStoreCurrency;
                return ((activePrimaryStoreCurrency != null && activePrimaryStoreCurrency.CurrencyId == this.CurrencyId));
            }
        }

        #endregion
    }

}
