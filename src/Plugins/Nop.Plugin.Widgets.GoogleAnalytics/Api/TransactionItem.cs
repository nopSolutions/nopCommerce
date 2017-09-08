//Contributor: https://www.codeproject.com/Articles/493455/Server-side-Google-Analytics-Transactions

using System;
using System.Globalization;

namespace Nop.Plugin.Widgets.GoogleAnalytics.Api
{
    public class TransactionItem
    {
        private readonly string _utmt = "item";

        private string _utmtid;     //OrderId
        private string _utmipc;     //Product code
        private string _utmipn;     //Product name
        private string _utmipr;     //Product price (unit price)
        private string _utmiqt;     //Quantity
        private string _utmiva;     //Product category

        /// <summary>
        /// Create a new TransactionItem
        /// </summary>
        /// <param name="orderId">Order number</param>
        /// <param name="productName">Product name</param>
        /// <param name="productPrice">Unit price</param>
        /// <param name="quantity">Quantity</param>
        /// <param name="category">The product category</param>
        public TransactionItem(string orderId, string productCode, string productName, decimal productPrice, int quantity, string category)
        {
            var usCulture = new CultureInfo("en-US");

            _utmtid = Uri.EscapeDataString(orderId);
            _utmipc = Uri.EscapeDataString(productCode);
            _utmipn = Uri.EscapeDataString(productName);
            _utmipr = productPrice.ToString("0.00", usCulture);
            _utmiqt = quantity.ToString();
            _utmiva = Uri.EscapeDataString(category);
        }

        public string CreateParameterString()
        {
            return string.Format("utmt={0}&utmtid={1}&utmipc={2}&utmipn={3}&utmipr={4}&utmiqt={5}&utmiva={6}",
                                 _utmt,
                                 _utmtid,
                                 _utmipc,
                                 _utmipn,
                                 _utmipr,
                                 _utmiqt,
                                 _utmiva);
        }
    }
}
