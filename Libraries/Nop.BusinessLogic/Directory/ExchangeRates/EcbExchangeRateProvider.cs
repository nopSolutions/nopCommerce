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
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;
using NopSolutions.NopCommerce.BusinessLogic.Caching;
using NopSolutions.NopCommerce.BusinessLogic.Directory;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.Data;
using NopSolutions.NopCommerce.BusinessLogic.Profile;
using NopSolutions.NopCommerce.Common;
using NopSolutions.NopCommerce.Common.Utils;

namespace NopSolutions.NopCommerce.BusinessLogic.Directory.ExchangeRates
{
    /// <summary>
    /// ECB (European central bank) exchange rate provider interface
    /// </summary>
    public partial class EcbExchangeRateProvider : IExchangeRateProvider 
    {
        #region Ctor
        /// <summary>
        /// Ctor
        /// </summary>
        public EcbExchangeRateProvider()
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets currency live rates
        /// </summary>
        /// <param name="exchangeRateCurrencyCode">Exchange rate currency code</param>
        /// <returns>Exchange rates</returns>
        public List<ExchangeRate> GetCurrencyLiveRates(string exchangeRateCurrencyCode)
        {
            if (String.IsNullOrEmpty(exchangeRateCurrencyCode) ||
                exchangeRateCurrencyCode.ToLower() != "eur")
                throw new NopException("You can use ECB (European central bank) exchange rate provider only when exchange rate currency code is set to EURO");

            var exchangeRates = new List<ExchangeRate>();

            HttpWebRequest request = WebRequest.Create("http://www.ecb.int/stats/eurofxref/eurofxref-daily.xml") as HttpWebRequest;
            using (WebResponse response = request.GetResponse())
            {
                XmlDocument document = new XmlDocument();
                document.Load(response.GetResponseStream());
                XmlNamespaceManager nsmgr = new XmlNamespaceManager(document.NameTable);
                nsmgr.AddNamespace("ns", "http://www.ecb.int/vocabulary/2002-08-01/eurofxref");
                nsmgr.AddNamespace("gesmes", "http://www.gesmes.org/xml/2002-08-01");
                
                XmlNode node = document.SelectSingleNode("gesmes:Envelope/ns:Cube/ns:Cube", nsmgr);
                var updateDate = DateTime.ParseExact(node.Attributes["time"].Value, "yyyy-MM-dd", null);

                NumberFormatInfo provider = new NumberFormatInfo();
                provider.NumberDecimalSeparator = ".";
                provider.NumberGroupSeparator = "";
                foreach (XmlNode node2 in node.ChildNodes)
                {
                    exchangeRates.Add(new ExchangeRate()
                    {
                        CurrencyCode = node2.Attributes["currency"].Value,
                        Rate = decimal.Parse(node2.Attributes["rate"].Value, provider),
                        UpdatedOn = updateDate
                    }
                    );
                }
            }
            return exchangeRates;
        }

        #endregion

        #region Properties
        /// <summary>
        /// Gets exchange rate provider name
        /// </summary>
        public string Name
        {
            get
            {
                return "ECB";
            }
        }
        #endregion
    }
}