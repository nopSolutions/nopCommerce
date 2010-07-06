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
// Contributor(s): 
//------------------------------------------------------------------------------

using System;

namespace NopSolutions.NopCommerce.Payment.Methods.Dibs
{
    /// <summary>
    /// Dibs utilities
    /// </summary>
    public class DibsHelper
    {
        #region Methods
        /// <summary>
        /// Gets the dibs currency number by currency code
        /// </summary>
        /// <param name="currencyCode">Currency code</param>
        /// <returns>Dibs currency number</returns>
        public static int GetCurrencyNumberByCode(string currencyCode)
        {
            if(String.IsNullOrEmpty(currencyCode))
            {
                throw new ArgumentNullException();
            }
            if(currencyCode.Equals("SEK"))
            {
                return 752;
            }
            else if(currencyCode.Equals("DKK"))
            {
                return 208;
            }
            else
            {
                throw new ArgumentException("Unkown currency code");
            }
        }

        public static string GetCurrencyCodeByNumber(int currencyNumber)
        {
            switch(currencyNumber)
            {
                case 208:
                    return "DKK";
                case 752:
                    return "SEK";
                default:
                    throw new ArgumentException("Unkown currency number");
            }
        }

        public static string GetLanguageCodeByLanguageCulture(string languageCulture)
        {
            if(String.IsNullOrEmpty(languageCulture))
            {
                throw new ArgumentNullException();
            }
            if(languageCulture.Equals("sv-SE"))
            {
                return "sv";
            }
            else if(languageCulture.Equals("da-DK"))
            {
                return "da";
            }
            else
            {
                return "en";
            }
        }
        #endregion
    }
}
