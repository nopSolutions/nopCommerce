//Contributor: https://www.codeproject.com/Articles/493455/Server-side-Google-Analytics-Transactions

using System;
using System.Net.Http;
using Nop.Core;

namespace Nop.Plugin.Widgets.GoogleAnalytics.Api
{
    public class GoogleRequest
    {
        #region Fields

        private const string BASE_URL = "http://www.google-analytics.com/__utm.gif?";

        private const string ANALYTICS_VERSION = "5.3.7";
        private const string BROWSER_JAVA_ENABLED = "0";

        //Required parameters but not necessary for us, so just post a default
        private const string SCREEN_RESOLUTION = "1680x1050";
        private const string SCREEN_COLOR_DEPTH = "32-bit";
        private const string FLASH_VERSION = "11.5%20r31";

        //Internal request counter. Max requests = 500 per session
        private int _requestCount;
        private int? _domainHash;

        #endregion
        
        #region Utilities

        private async void FireRequest(string url)
        {
            if (_requestCount < 500)
            {
                _requestCount++;

                var httpClient = new HttpClient();
                try
                {
                    // we don't need the response so this is the end of the request
                    var response = (await httpClient.GetAsync(url)).EnsureSuccessStatusCode();
                }
                catch (Exception)
                {
                    //eat the error 
                }

            }
        }

        private string CreateParameterString()
        {
            return string.Format("utmwv={0}&utms={1}&utmn={2}&utmhn={3}&utmsr{4}&utmvp={5}&utmsc={6}&utmul={7}&utmje={8}&utmfl={9}&utmhid={10}&utmr={11}&utmp={12}&utmac={13}&utmcc={14}",
                ANALYTICS_VERSION,
                _requestCount,
                CommonHelper.GenerateRandomInteger(),
                HostName,
                SCREEN_RESOLUTION,
                SCREEN_RESOLUTION,
                SCREEN_COLOR_DEPTH,
                Culture,
                BROWSER_JAVA_ENABLED,
                FLASH_VERSION,
                CommonHelper.GenerateRandomInteger(),
                "-",
                PageTitle,
                AccountCode,
                GetUtmcCookieString());
        }

        private int DomainHash
        {
            get
            {
                if (!_domainHash.HasValue)
                {
                    if (HostName != null)
                    {
                        int a;
                        int c;
                        int h;
                        char chrCharacter;
                        int intCharacter;

                        a = 0;
                        for (h = HostName.Length - 1; h >= 0; h--)
                        {
                            chrCharacter = char.Parse(HostName.Substring(h, 1));
                            intCharacter = (int)chrCharacter;
                            a = (a << 6 & 268435455) + intCharacter + (intCharacter << 14);
                            c = a & 266338304;
                            a = c != 0 ? a ^ c >> 21 : a;
                        }

                        _domainHash = a;
                    }
                    _domainHash = 0;
                }

                return _domainHash.Value;
            }
        }

        private string _UtmcCookieString = null;
        //The cookie collection string
        private string GetUtmcCookieString()
        {
            if (_UtmcCookieString == null)
            {
                //create the unix timestamp
                var span = (DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, 0).ToLocalTime());
                var timeStampCurrent = (int)span.TotalSeconds;

                //fake the utma
                var utma = string.Format("{0}.{1}.{2}.{3}.{4}.{5}",
                                            DomainHash,
                                            CommonHelper.GenerateRandomInteger(),
                                            timeStampCurrent,
                                            timeStampCurrent,
                                            timeStampCurrent,
                                            "2");
                var utmz = string.Format("{0}.{1}.{2}.{3}.utmcsr={4}|utmccn={5}|utmcmd={6}",
                                                DomainHash,
                                                timeStampCurrent,
                                                "1",
                                                "1",
                                                "(direct)",
                                                "(direct)",
                                               "(none)");


                _UtmcCookieString = Uri.EscapeDataString(string.Format("__utma={0};+__utmz={1};", utma, utmz));
            }

            return _UtmcCookieString;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Send the request to the Google servers
        /// </summary>
        /// <param name="transaction">A corresponding transaction</param>
        public void SendRequest(Transaction transaction)
        {
            var requestUrl = BASE_URL + CreateParameterString() + "&" + transaction.CreateParameterString();
            FireRequest(requestUrl);

                foreach (var transItem in transaction.Items)
                {
                    FireRequest(BASE_URL + CreateParameterString() + "&" + transItem.CreateParameterString());
                }
        }

        #endregion

        #region Properties

        /// <summary>                           
        /// Your Google tracking code (e.g. UA-12345678-1)
        /// </summary>        
        public string AccountCode { get; set; }

        /// <summary>
        /// The language of the customer (e.g. en-US)
        /// </summary>
        public string Culture { get; set; }

        /// <summary>
        /// The hostname of the website making the request (e.g. www.google.com)
        /// </summary>        
        public string HostName { get; set; }

        /// <summary>
        /// The title of the page making the request
        /// </summary>
        public string PageTitle { get; set; }

        #endregion
    }
}
