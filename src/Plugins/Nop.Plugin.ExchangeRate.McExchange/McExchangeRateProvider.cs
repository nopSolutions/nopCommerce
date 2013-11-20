using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Xml;
using Nop.Core.Plugins;
using Nop.Services.Directory;
using Nop.Services.Logging;

namespace Nop.Plugin.ExchangeRate.McExchange
{
    public class McExchangeRateProvider : BasePlugin, IExchangeRateProvider
    {
        private readonly ILogger _logger;

        public McExchangeRateProvider(ILogger logger)
        {
            this._logger = logger;
        }

        /// <summary>
        /// Gets currency live rates
        /// </summary>
        /// <param name="exchangeRateCurrencyCode">Exchange rate currency code</param>
        /// <returns>Exchange rates</returns>
        public IList<Core.Domain.Directory.ExchangeRate> GetCurrencyLiveRates(string exchangeRateCurrencyCode)
        {
            var exchangeRates = new List<Core.Domain.Directory.ExchangeRate>();
            string url = string.Format("http://themoneyconverter.com/rss-feed/{0}/rss.xml", exchangeRateCurrencyCode);

            var request = WebRequest.Create(url) as HttpWebRequest;
            using (var response = request.GetResponse())
            {
                var xmlDoc = new XmlDocument();
                xmlDoc.Load(response.GetResponseStream());
                var nsmgr = new XmlNamespaceManager(xmlDoc.NameTable);
                nsmgr.AddNamespace("atom", "http://www.w3.org/2005/Atom");
                nsmgr.AddNamespace("cf", "http://www.microsoft.com/schemas/rss/core/2005");
                nsmgr.AddNamespace("cfi", "http://www.microsoft.com/schemas/rss/core/2005/internal");
                
                foreach (XmlNode node in xmlDoc.DocumentElement.FirstChild.ChildNodes)
                {
                    if (node.Name == "item")
                    {
                        /*
                                <item>
                                  <title xmlns:cf="http://www.microsoft.com/schemas/rss/core/2005" cf:type="text">AED/NZD</title>
                                  <link>http://themoneyconverter.com/NZD/AED.aspx</link>
                                  <guid>http://themoneyconverter.com/NZD/AED.aspx</guid>
                                  <pubDate>Fri, 20 Feb 2009 08:01:41 GMT</pubDate>
                                  <atom:published xmlns:atom="http://www.w3.org/2005/Atom">2009-02-20T08:01:41Z</atom:published>
                                  <atom:updated xmlns:atom="http://www.w3.org/2005/Atom">2009-02-20T08:01:41Z</atom:updated>
                                  <description xmlns:cf="http://www.microsoft.com/schemas/rss/core/2005" cf:type="html">1 New Zealand Dollar = 1.84499 Arab Emirates Dirham</description>
                                  <category>Middle-East</category>
                                  <cfi:id>32</cfi:id>
                                  <cfi:effectiveId>1074571199</cfi:effectiveId>
                                  <cfi:read>true</cfi:read>
                                  <cfi:downloadurl>http://themoneyconverter.com/NZD/rss.xml</cfi:downloadurl>
                                  <cfi:lastdownloadtime>2009-02-20T08:05:27.168Z</cfi:lastdownloadtime>
                                </item>
                        */
                        try
                        {
                            var rate = new Core.Domain.Directory.ExchangeRate();
                            foreach (XmlNode detailNode in node.ChildNodes)
                            {
                                switch (detailNode.Name)
                                {
                                    case "title":
                                        rate.CurrencyCode = detailNode.InnerText.Substring(0, 3);
                                        break;

                                    case "pubDate":
                                        rate.UpdatedOn = DateTime.Parse(detailNode.InnerText, CultureInfo.InvariantCulture);
                                        break;

                                    case "description":
                                        string description = detailNode.InnerText;
                                        int x = description.IndexOf('=');
                                        int y = description.IndexOf(' ', x + 2);

                                        //          1         2         3         4
                                        //01234567890123456789012345678901234567890
                                        // 1 New Zealand Dollar = 0.78815 Australian Dollar
                                        // x = 21
                                        // y = 30
                                        string rateText = description.Substring(x + 1, y - x - 1).Trim();
                                        rate.Rate = decimal.Parse(rateText, CultureInfo.InvariantCulture);
                                        break;

                                    default:
                                        break;
                                }
                            }

                            // Update the Rate in the collection if its already in there
                            if (rate.CurrencyCode != null)
                            {
                                exchangeRates.Add(rate);
                            }
                        }
                        catch (Exception exc)
                        {
                            _logger.Warning(string.Format("Error parsing currency rates (MC): {0}", exc.Message), exc);
                        }
                    } // if node is an item
                } // foreach child node under <channel>
            }

            return exchangeRates;
        }
    }
}
