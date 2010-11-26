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
using System.Collections.Specialized;
using System.Security.Cryptography;
using System.Text;

namespace NopSolutions.NopCommerce.Payment.Methods.CyberSource
{
    public class HostedPaymentHelper
    {
        #region Properties
        internal static string OrderPageTimestamp
        {
            get
            {
                return (DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds.ToString().Split('.')[0];
            }
        }
        #endregion

        #region Methods
        public static string CalcRequestSign(NameValueCollection reqParams)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(reqParams["merchantID"]);
            sb.Append(reqParams["amount"]);
            sb.Append(reqParams["currency"]);
            sb.Append(reqParams["orderPage_timestamp"]);
            sb.Append(reqParams["orderPage_transactionType"]);

            return CalcHMACSHA1Hash(sb.ToString()).Replace("\n", "");
        }

        public static bool ValidateResponseSign(NameValueCollection rspParams)
        {
            string transactionSignature = null;
            string[] signedFieldsArr = null;

            try
            {
                transactionSignature = rspParams["transactionSignature"];
                signedFieldsArr = rspParams["signedFields"].Split(',');
            }
            catch (Exception)
            {
                return false;
            }
            
            StringBuilder sb = new StringBuilder();
            for(int i = 0; i < signedFieldsArr.Length; i++)
            {
                sb.Append(rspParams[signedFieldsArr[i]]);
            }

            string s = CalcHMACSHA1Hash(sb.ToString());

            return transactionSignature.Equals(s);
        }
        #endregion

        #region Utilities
        private static string CalcHMACSHA1Hash(string s)
        {
            using(HMACSHA1 cs = new HMACSHA1(Encoding.UTF8.GetBytes(HostedPaymentSettings.PublicKey)))
            {
                return Convert.ToBase64String(cs.ComputeHash(Encoding.UTF8.GetBytes(s)));
            }
        }
        #endregion
    }
}
