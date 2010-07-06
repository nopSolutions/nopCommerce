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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NopSolutions.NopCommerce.Common.Utils;
using System.Security.Cryptography;
using System.Collections.Specialized;

namespace NopSolutions.NopCommerce.Payment.Methods.ChronoPay
{
    public class HostedPaymentHelper
    {
        #region Methods
        public static string CalcRequestSign(NameValueCollection reqParams)
        {
            return CalcMd5Hash(String.Format("{0}-{1}-{2}", reqParams["product_id"], reqParams["product_price"], HostedPaymentSettings.SharedSecrect));
        }

        public static bool ValidateResponseSign(NameValueCollection rspParams)
        {
            string rspSign = rspParams["sign"];
            if(String.IsNullOrEmpty(rspSign))
            {
                return false;
            }
            return rspSign.Equals(CalcMd5Hash(String.Format("{0}{1}{2}{3}{4}", HostedPaymentSettings.SharedSecrect, rspParams["customer_id"], rspParams["transaction_id"], rspParams["transaction_type"], rspParams["total"])));
        }
        #endregion

        #region Utilities
        private static string CalcMd5Hash(string s)
        {
            using(MD5 cs = MD5.Create())
            {
                StringBuilder sb = new StringBuilder();
                byte[] hash;

                hash = cs.ComputeHash(Encoding.UTF8.GetBytes(s));
                sb.Length = 0;
                foreach(byte b in hash)
                {
                    sb.Append(b.ToString("x2"));
                }

                return sb.ToString();
            }
        }
        #endregion
    }
}
