//Contributor: https://www.codeproject.com/Articles/493455/Server-side-Google-Analytics-Transactions

using System;
using System.Collections.Generic;
using System.Globalization;

namespace Nop.Plugin.Widgets.GoogleAnalytics.Api
{
    public class Transaction
    {        
        private readonly string _utmt = "tran";

        private string _orderId;       //(utmtid)
        private string _utmtci;        //Billing city
        private string _utmtco;        //Billing country
        private string _utmtrg;        //Billing region
        private string _utmtst;        //Store name / affiliation
        private string _utmtsp;        //Shipping costs
        private string _utmtto;        //Order total
        private string _utmttx;        //Tax costs

        /// <summary>
        /// Create a new E-commerce Transaction
        /// </summary>
        /// <param name="orderId">Order identifier</param>
        /// <param name="billingCity">Billing city</param>
        /// <param name="country">Country</param>
        /// <param name="region">Region</param>
        /// <param name="storeName">Store name</param>
        /// <param name="shipping">Shipping</param>
        /// <param name="tax">Tax</param>
        /// <param name="orderTotal">Order total</param>
        public Transaction(string orderId, string billingCity, string country, string region, string storeName, decimal shipping, decimal tax, decimal orderTotal)
        {
            Items = new List<TransactionItem>();

            _orderId = Uri.EscapeDataString(orderId);

            var usCulture = new CultureInfo("en-US");
            _utmtci = Uri.EscapeDataString(billingCity);
            _utmtco = Uri.EscapeDataString(country);
            _utmtrg = Uri.EscapeDataString(region);
            _utmtst = Uri.EscapeDataString(storeName);
            _utmtsp = shipping.ToString("0.00", usCulture);
            _utmttx = tax.ToString("0.00", usCulture);
            _utmtto = orderTotal.ToString("0.00", usCulture);
        }
        
        public string CreateParameterString()
        {
            return string.Format("utmt={0}&utmtci={1}&utmtco={2}&utmtrg={3}&utmtid={4}&utmtst={5}&utmtsp={6}&utmtto={7}&utmttx={8}",
                                 _utmt,
                                 _utmtci,
                                 _utmtco,
                                 _utmtrg,
                                 _orderId,
                                 _utmtst,
                                 _utmtsp,
                                 _utmtto,
                                 _utmttx);       
        }

        public List<TransactionItem> Items { get; set; }
    }
}
